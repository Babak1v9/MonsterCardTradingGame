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

            switch (_request.Method) {
                case "POST":
                    try {
                        string token = _request.Headers["authorization"];

                        var tokenWithoutBasic = token.Remove(0, 6);
                        var tokenExists = userDBController.VerifyUserToken(tokenWithoutBasic);

                        if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                            _response.StatusCode = 401;
                            _response.SetContent(Environment.NewLine+"Unauthorized."+ Environment.NewLine);
                            return;
                        }

                        var user = userDBController.GetByToken(token);

                        if (user == null) {
                            _response.StatusCode = 400;
                            _response.SetContent(Environment.NewLine + "User not found." + Environment.NewLine);
                            return;
                        }

                        GameHandler.Instance.ConnectUserToBattle(user);
                        string gameLog = GameHandler.Instance.GameLog;


                        _response.StatusCode = 200;
                        _response.SetContent(Environment.NewLine + "Battle is over."+ Environment.NewLine+"--- BEGIN OF GAME LOG ---"+ Environment.NewLine + gameLog + Environment.NewLine +"--- END OF GAME LOG ---" + Environment.NewLine);
                    } catch (Exception e) {

                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent(Environment.NewLine+"Invalid Request"+ Environment.NewLine);
                    }
                    break;

                default:
                    _response.invalidURL();
                    break;
            }

        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
