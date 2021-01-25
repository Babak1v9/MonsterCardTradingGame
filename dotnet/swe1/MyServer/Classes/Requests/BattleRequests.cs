using System.IO;
using System;
using MTCGserver.Classes.DB;
using MTCGserver.Interfaces;

namespace MTCGserver.Classes.RequestHandlers {
    class BattleRequests : IMyRequests {

        private Request _request;
        private Response _response;
        private UserDatabaseController userDBController = new UserDatabaseController();
        public Request Request => _request;

        public Response Response {
            get => _response;
            set => _response = value;
        }
        public BattleRequests(Request request) {
            _request = request;
            _response = new Response { ContentType = "text/plain" };
        }

        public void ExecuteTask() {

            switch (_request.Method) {
                case "POST":
                    try {

                        bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                        if (!Authentication) {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        string token = _request.Headers["authorization"];
                        var user = userDBController.GetByToken(token);

                        if (user == null) {
                            _response.StatusCode = 400;
                            _response.SetContent("User not found.");
                            return;
                        }

                        BattleHandler.Instance.ConnectUserToBattle(user);
                        string gameLog = BattleHandler.Instance.GameLog;


                        _response.StatusCode = 200;
                        _response.SetContent("Battle is over."+ Environment.NewLine+"--- BEGIN OF GAME LOG ---"+ Environment.NewLine + gameLog + Environment.NewLine +"--- END OF GAME LOG ---");
                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.SendException(e);
                    }
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
