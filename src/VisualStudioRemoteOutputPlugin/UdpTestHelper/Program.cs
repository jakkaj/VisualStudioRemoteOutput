using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpTestHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            _udp();
            Console.ReadLine();
        }

        public static async void _udp()
        {
            var client = new UdpClient();

            var message = "Jordan";
            var target = IPAddress.Parse("192.168.0.11");
            var ep = new IPEndPoint(target, 20000);
            var sendbuf = Encoding.ASCII.GetBytes(message);

            while (true)
            {
                client.Send(sendbuf, sendbuf.Length, ep);
                await Task.Delay(1000);

            }
        }

        public static async void _tcp()
        {
            //var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //var client = new UdpClient();

            var client = new TcpClient();

            while (true)
            {
                var message = "Jordan";
                var target = IPAddress.Parse("192.168.0.11");
                var ep = new IPEndPoint(target, 20000);
                var sendbuf = Encoding.ASCII.GetBytes(message);

                if (!client.Connected)
                {
                    Debug.WriteLine("Attempting to connect");
                    await client.ConnectAsync(target, 20000);
                    if (!client.Connected)
                    {
                        Debug.WriteLine("Could not connect");
                        await Task.Delay(1000);
                        continue;
                    }
                    Debug.WriteLine("Connected");
                }

                var sendBytes = Encoding.UTF8.GetBytes(message);

                var stream = client.GetStream();
                stream.Write(sendBytes, 0, message.Length);
                var data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);




                ////client.Send(sendbuf, sendbuf.Length, ep);
                //serverSocket.SendTo(sendbuf, ep);


                await Task.Delay(1000);


            }


        }
    }
}
