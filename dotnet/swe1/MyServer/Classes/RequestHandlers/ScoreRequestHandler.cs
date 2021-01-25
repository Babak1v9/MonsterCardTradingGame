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
        private UserDatabaseController userDBController = new UserDatabaseController();
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

                    bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                    if (!Authentication) {
                        _response.UnauthenticatedUser();
                        return;
                    }
             
                    string scoreboard ="";
                    List<int> userStats = new List<int>();
                    List<User> Users = new List<User>();
                    Users = userDBController.GetUsers();

                    foreach (User user in Users) {
                        if (user.Username != "admin") {
                            userStats = userDBController.ShowUserStats(user.Token);
                            scoreboard += Environment.NewLine + "Name: " + user.Name + " Elo: " + userStats[0] + " Games Played: " + userStats[1] + " Wins: " + userStats[2] + Environment.NewLine;
                        }
                    }
                    _response.StatusCode = 400;
                    _response.SetContent(scoreboard);
                    break;

                default:
                    _response.InvalidURL();
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
