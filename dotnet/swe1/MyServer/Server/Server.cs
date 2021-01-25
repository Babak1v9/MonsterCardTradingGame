using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MTCGserver.Classes;
using MTCGserver.Classes.RequestHandlers;
using MTCGserver.Interfaces;

namespace MTCGserver {

    public class Server {
        public static void Listen() {

            TcpListener listener = new TcpListener(IPAddress.Any, 10001);
            listener.Start();

            //endless loop
            while (true) {
                Console.WriteLine("Server listening on: " + listener.LocalEndpoint + Environment.NewLine);
                Console.WriteLine("Waiting for connection..." + Environment.NewLine);
  
                Socket client = listener.AcceptSocket();
                Console.WriteLine("New Connection incoming..." + Environment.NewLine);
                var connection = new Thread(() => HandleRequest(client));
                connection.Start();
            }
        }

        private static void HandleRequest(Socket clientSocket) {

            Console.WriteLine("Connection accepted" + Environment.NewLine);

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
            var response = new Response { ContentType = "text/plain", StatusCode = 200 };
            
            var firstSegment = request.Url.Segments[0];

            IMyRequests requestHandler = firstSegment switch {
                "users" => new UserRequests(request),
                "packages" => new PackageRequests(request),
                "cards" => new CardsRequests(request),
                "deck" => new DeckRequests(request),
                "score" => new ScoreRequests(request),
                "sessions" => new SessionRequests(request),
                "stats" => new StatsRequests(request),
                "tradings" => new TradingRequests(request),
                "transactions" => new TransactionsRequests(request),
                "battles" => new BattleRequests(request),
                _ => new UnknownRequests(request)

            };

            requestHandler.ExecuteTask();
            requestHandler.SendResponse(stream);

            clientSocket.Close();
        }
    }
}