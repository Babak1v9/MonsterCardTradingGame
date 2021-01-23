using System;
using System.IO;
using MTCGserver.Classes.HTTP;
using MTCGserver.Interfaces;

namespace MTCGserver.Classes.Requests {
    class UnknownRequestHandler : IRequestHandler {
        public UnknownRequestHandler(Request request) {
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
