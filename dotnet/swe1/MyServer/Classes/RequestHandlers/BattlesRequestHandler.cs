
using _Server.Interfaces;
using _Server.Classes;
using System.IO;

namespace MyServer.Classes.RequestHandlers {
    class BattlesRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private UserDatabaseController UserDBController = new UserDatabaseController();
        public Request Request => _request;

        public Response Response {
            get => _response;
            set => _response = value;
        }
        public BattlesRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "text/plain" };
        }

        public void ExecuteTask() {
            if (!_request.Headers.ContainsKey("authorization")) {
                _response.StatusCode = 400;
                _response.SetContent("unauthorized");
                return;
            }

            var fighter = UserDBController.GetByToken(_request.Headers["authorization"]);

            if (fighter == null) {
                _response.StatusCode = 400;
                _response.SetContent("No user could be found within the specified token");
                return;
            }

            var updatedPlayer = GameHandler.Instance.ConnectPlayerToBattle(fighter);
            //todo: update the player returned here, as he will have the log and the new elo -> stroe to db
            
            _response.StatusCode = 200;
            _response.SetContent("success");
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
