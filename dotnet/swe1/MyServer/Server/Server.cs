using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using _Server.Classes;

namespace _MyServer.Util {

    public class Server {

        public static void Listen() {

            TcpListener listener = new TcpListener(IPAddress.Any, 4040);
            listener.Start();

            //endless loop
            while (true) {
                Console.WriteLine("Server listening on: " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for connection...");
                Console.WriteLine(Environment.NewLine);

                
                Socket client = listener.AcceptSocket();
                Console.WriteLine("New Connection incoming...");
                Console.WriteLine(Environment.NewLine);
                var connection = new Thread(() => HandleRequest(client));
                connection.Start();
            }
        }

        private static void HandleRequest(Socket clientSocket) {

            Console.WriteLine("Connection accepted");
            Console.WriteLine(Environment.NewLine);

            using var stream = new NetworkStream(clientSocket);
            using var memoryStream = new MemoryStream();

            int bytesRead;
            var readBuffer = new byte[1024];


            while ((bytesRead = stream.Read(readBuffer, 0, readBuffer.Length)) > 0) {
                memoryStream.Write(readBuffer, 0, bytesRead);
                if (!stream.DataAvailable) {
                    break;
                }
            }

            var readDataString = Encoding.UTF8.GetString(memoryStream.ToArray(), 0, (int)memoryStream.Length);
            var request = new Request(readDataString);
     
            Console.WriteLine($"Got Request with Method: {request.Method}");
            Console.WriteLine($"Raw Request: {readDataString}");

            var response = new Response { ContentType = "text/plain", StatusCode = 200 };

            response.AddHeader("Connection", "Close");
            response.SetContent($"Hi Mr. Client, this is Server{Environment.NewLine}" +
                                $"Your User Agent: {request.UserAgent}.{Environment.NewLine}" +
                                $"Your Header Count: {request.HeaderCount}");

            response.Send(stream);
            clientSocket.Close();
        }
    }
}