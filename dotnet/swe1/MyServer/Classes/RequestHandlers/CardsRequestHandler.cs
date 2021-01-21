using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using _Server.Classes;
using _Server.Interfaces;

namespace MyServer.Classes.RequestHandlers {
    class CardsRequestHandler : IMyRequestHandler {
        public CardsRequestHandler(Request request ) {
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
