using System;
using System.Collections.Generic;
using System.IO;
using MTCGserver.Interfaces;
using MTCGserver.Classes.DB;

namespace MTCGserver.Classes.RequestHandlers {
    class ScoreRequests : IMyRequests {

        private Request _request;
        private Response _response;
        private UserDatabaseController userDBController = new UserDatabaseController();
        public Request Request => _request;
        public Response Response {
            get => _response;
            set { _response = value; }
        }
        public ScoreRequests(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "GET":
                    try {
                        bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                        if (!Authentication) {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        string scoreboard = "";
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
                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.SendException(e);
                    }break;

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
