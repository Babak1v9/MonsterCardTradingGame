using _Server.Interfaces;
using _Server.Classes;
using System.IO;
using System;

namespace MyServer.Classes.RequestHandlers {
    class BattlesRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private UserDatabaseController userDBController = new UserDatabaseController();
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
            string token = _request.Headers["authorization"];

            var tokenWithoutBasic = token.Remove(0, 6);
            var tokenExists = userDBController.VerifyUserToken(tokenWithoutBasic);

            if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                _response.StatusCode = 401;
                _response.SetContent("Unauthorized");
                return;
            }

            var user = userDBController.GetByToken(token);

            if (user == null) {
                _response.StatusCode = 400;
                _response.SetContent(Environment.NewLine+ "No user found" + Environment.NewLine);
                return;
            }

            var updatedUser = GameHandler.Instance.ConnectUserToBattle(user);
            Console.WriteLine(updatedUser.Deck.Count);
            string gameLog = GameHandler.Instance.GameLog;
            // update count of games, elo
            
            _response.StatusCode = 200;
            _response.SetContent(Environment.NewLine + "Battle is over. Game Log:" + gameLog + Environment.NewLine);
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
