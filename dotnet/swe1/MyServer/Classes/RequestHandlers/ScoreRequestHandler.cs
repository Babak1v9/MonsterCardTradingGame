using System;
using System.Collections.Generic;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.Data;

namespace MyServer.Classes.RequestHandlers {
    class ScoreRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private UserDatabaseController _userDatabaseController = new UserDatabaseController();
        public Request Request => _request;
        public Response Response {
            get => _response;
            set { _response = value; }
        }
        public ScoreRequestHandler(Request request) {
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

                    string scoreboard ="";
                    List<int> userStats = new List<int>();
                    List<User> Users = new List<User>();
                    Users = _userDatabaseController.GetUsers();


                    /*for (int i = 0; i == Users.Count; i++) {
                        userStats = _userDatabaseController.ShowUserStats(Users[i].Token);
                    }*/

                    foreach (User user in Users) {
                        if (user.Username != "admin") {
                            userStats = _userDatabaseController.ShowUserStats(user.Token);
                            scoreboard += Environment.NewLine + "Name: " + user.Name + " Elo: " + userStats[0] + " Games Played: " + userStats[1] + " Wins: " + userStats[2] + Environment.NewLine;
                        }
                    }
                    _response.StatusCode = 400;
                    _response.SetContent(scoreboard);
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
