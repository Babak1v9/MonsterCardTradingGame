using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;

namespace MyServer.Classes.RequestHandlers {
    class UnknownRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;

        public UnknownRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }

        public void ExecuteTask() {
            _response.InvalidURL();
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
