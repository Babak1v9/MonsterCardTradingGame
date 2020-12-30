using System;
using _MyServer.Util;

namespace _Server {

    public class Runner {

        static void Main(string[] args) {

            Console.CancelKeyPress += delegate {
                Console.WriteLine("Goodbye!");
            };

            Server.Listen();

        }
    }
}
