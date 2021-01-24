using System;
using System.Collections.Generic;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;

namespace MyServer.Classes.RequestHandlers {
    class StatsRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private UserDatabaseController _userDatabaseController = new UserDatabaseController();
        public Request Request => _request;
        public Response Response {
            get => _response;
            set { _response = value; }
        }
        public StatsRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }
        public void ExecuteTask() {
            switch (_request.Method) {
                case "GET":

                    string token = _request.Headers["authorization"];
                    var tokenWithoutBasic = token.Remove(0, 6);
                    var tokenExists = _userDatabaseController.VerifyUserToken(tokenWithoutBasic);

                    if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                        _response.StatusCode = 401;
                        _response.SetContent(Environment.NewLine + "Unauthorized" + Environment.NewLine);
                        return;
                    }
                    List<int> userStats = new List<int>();
                    userStats = _userDatabaseController.ShowUserStats(tokenWithoutBasic);
                    _response.StatusCode = 400;
                    _response.SetContent(Environment.NewLine + "Elo: " + userStats[0] + " Games Played: " + userStats[1] + " Wins: " + userStats[2] + Environment.NewLine);

                    break;

                default:
                    _response.StatusCode = 400;
                    _response.SetContent(Environment.NewLine + "Invalid HTTP Method" + Environment.NewLine);
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
