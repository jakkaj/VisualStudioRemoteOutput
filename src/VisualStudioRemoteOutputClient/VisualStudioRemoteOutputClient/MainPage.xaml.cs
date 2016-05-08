using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VisualStudioRemoteOutputClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DatagramSocket _socket;

        public MainPage()
        {
            this.InitializeComponent();

            _test();
        }

        async void _test()
        {
            var l = NetworkInformation.GetHostNames().Where(localHostInfo => localHostInfo.IPInformation != null).ToList();
            var p = NetworkInformation.GetConnectionProfiles();

            _socket = new DatagramSocket();

          //  _socket.Control.DontFragment = true;
            //_socket.Control.MulticastOnly = false;
            _socket.MessageReceived += Socket_MessageReceived;
            //  _socket.Control.MulticastOnly = true;


            //await _socket.BindServiceNameAsync("15000");

            //var h = l.LastOrDefault();

           
            StartServer();
            await _socket.BindServiceNameAsync("15000");
        }


        private async void StartServer()
        {
            var port = "20000";
            var l = NetworkInformation.GetHostNames().Where(localHostInfo => localHostInfo.IPInformation != null).ToList();
            var h = l.LastOrDefault();
            var listener = new StreamSocketListener();
            await listener.BindEndpointAsync(h, port);
            //await listener.BindServiceNameAsync(port.ToString());
            Debug.WriteLine("Bound to port: " + port.ToString());
            listener.ConnectionReceived += async (s, e) =>
            {
                Debug.WriteLine("Got connection");
                using (IInputStream input = e.Socket.InputStream)
                {
                    var buffer = new Windows.Storage.Streams.Buffer(2);
                    await input.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.Partial);
                }

                using (IOutputStream output = e.Socket.OutputStream)
                {
                    using (Stream response = output.AsStreamForWrite())
                    {
                        response.Write(Encoding.ASCII.GetBytes("Hello, World!"), 0, 1);
                    }
                }
            };
        }

        private async void Socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            object outObj;
            if (CoreApplication.Properties.TryGetValue("remotePeer", out outObj))
            {
                EchoMessage((RemotePeer)outObj, eventArguments);
                return;
            }

            // We do not have an output stream yet so create one.
            try
            {
                IOutputStream outputStream = await _socket.GetOutputStreamAsync(
                    eventArguments.RemoteAddress,
                    eventArguments.RemotePort);

                // It might happen that the OnMessage was invoked more than once before the GetOutputStreamAsync call
                // completed. In this case we will end up with multiple streams - just keep one of them.
                RemotePeer peer;
                lock (this)
                {
                    if (CoreApplication.Properties.TryGetValue("remotePeer", out outObj))
                    {
                        peer = (RemotePeer)outObj;
                    }
                    else
                    {
                        peer = new RemotePeer(outputStream, eventArguments.RemoteAddress, eventArguments.RemotePort);
                        CoreApplication.Properties.Add("remotePeer", peer);
                    }
                }

                EchoMessage(peer, eventArguments);
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                NotifyUserFromAsyncThread("Connect failed with error: " + exception.Message);
            }
        }

        /// <summary>
        /// Echo the message back to the peer
        /// </summary>
        /// <param name="peer">The remote peer object</param>
        /// <param name="eventArguments">The received message event arguments</param>
        async void EchoMessage(RemotePeer peer, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            if (!peer.IsMatching(eventArguments.RemoteAddress, eventArguments.RemotePort))
            {
                // In the sample we are communicating with just one peer. To communicate with multiple peers, an
                // application should cache output streams (e.g., by using a hash map), because creating an output
                // stream for each received datagram is costly. Keep in mind though, that every cache requires logic
                // to remove old or unused elements; otherwise, the cache will turn into a memory leaking structure.
                NotifyUserFromAsyncThread(
                    String.Format(
                        "Got datagram from {0}:{1}, but already 'connected' to {2}",
                        eventArguments.RemoteAddress,
                        eventArguments.RemotePort,
                        peer));
            }

            try
            {
                await peer.OutputStream.WriteAsync(eventArguments.GetDataReader().DetachBuffer());
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                NotifyUserFromAsyncThread("Send failed with error: " + exception.Message);
            }
        }

        /// <summary>
        /// Notifies the user from a non-UI thread
        /// </summary>
        /// <param name="strMessage">The message</param>
        /// <param name="type">The type of notification</param>
        private void NotifyUserFromAsyncThread(string strMessage)
        {
            Debug.WriteLine("Notify from other: " + strMessage);
        }


    }

    class RemotePeer
    {
        IOutputStream outputStream;
        HostName hostName;
        String port;

        public RemotePeer(IOutputStream outputStream, HostName hostName, String port)
        {
            this.outputStream = outputStream;
            this.hostName = hostName;
            this.port = port;
        }

        public bool IsMatching(HostName hostName, String port)
        {
            return (this.hostName == hostName && this.port == port);
        }

        public IOutputStream OutputStream
        {
            get { return outputStream; }
        }

        public override String ToString()
        {
            return hostName + port;
        }
    }
}
