using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace VisualStudioRemoteOutputPlugin.Network
{
    public class NetHost
    {

        private HttpClient _client;
        private HttpClientHandler _httpHandler;

        public static NetHost Instance { get; private set; }

        public NetHost()
        {
            Instance = this;

            _httpHandler = new HttpClientHandler();

            _client = new HttpClient(_httpHandler);
            _client.Timeout = TimeSpan.FromSeconds(1);

            //NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            //foreach (NetworkInterface adapter in adapters)
            //{
            //    IPInterfaceProperties properties = adapter.GetIPProperties();
            //    Debug.WriteLine(adapter.Description);
            //    Debug.WriteLine("  DNS suffix .............................. : {0}",
            //        properties.DnsSuffix);
            //    Debug.WriteLine("  DNS enabled ............................. : {0}",
            //        properties.IsDnsEnabled);
            //    Debug.WriteLine("  Dynamically configured DNS .............. : {0}",
            //        properties.IsDynamicDnsEnabled);
            //}
            //var s = "s";
            ////var port = 15000;

            //var target = IPAddress.Parse("192.168.0.11");
            //var ep = new IPEndPoint(target, 15001);
            //var target = IPAddress.Parse("192.168.0.11");
            //var ep = new IPEndPoint(target, 15002);
            //_client = new UdpClient(ep);

            //var c = new WebClient();

            //var result = c.DownloadString("http://www.theage.com.au");
            //var sdd = result;
            //_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //Socket.Select();

            //  _socket.Bind(ep);
            //_socket.Connect(ep);
        }

        public async void Send(string output)
        {
            using (var message = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5050"))
            {
                var content = new StringContent(output, Encoding.UTF8, "text/plain");
                message.Content = content;

                using (var result = await _client.SendAsync(message))
                {
                    var r = result.IsSuccessStatusCode;                    
                }
            }          
        }
    }
}
