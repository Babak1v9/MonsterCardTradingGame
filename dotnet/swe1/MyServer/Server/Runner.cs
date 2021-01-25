using System;
using MTCGserver;

namespace ServerRunner {

    public class Runner {

        static void Main(string[] args) {

            Console.CancelKeyPress += delegate {
                Console.WriteLine("Goodbye!");
            };

            Server.Listen();

        }
    }
}
