using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;

namespace MyServer.Classes.RequestHandlers {
    class TradingRequestHandler : IMyRequestHandler {

        public TradingRequestHandler(Request request) {
            return;
        }

        public Request Request => throw new NotImplementedException();

        public Response Response { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void ExecuteTask() {
            throw new NotImplementedException();
        }

        public void SendResponse(Stream stream) {
            throw new NotImplementedException();
        }
    }
}
