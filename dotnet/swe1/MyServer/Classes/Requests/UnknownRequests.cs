using System.IO;
using MTCGserver.Classes;
using MTCGserver.Interfaces;

namespace MTCGserver.Classes.RequestHandlers {
    class UnknownRequests : IMyRequests {

        private Request _request;
        private Response _response;

        public UnknownRequests(Request request) {
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
