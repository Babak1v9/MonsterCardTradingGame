using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.DB_Stuff;

namespace MyServer.Classes.RequestHandlers {
    class TransactionsRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private DeckDataBaseController _deckDatabaseController = new DeckDataBaseController();
        private UserDatabaseController _userDatabaseController = new UserDatabaseController();

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }

        public TransactionsRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "POST":
                    try {

                        if (_request.Url.Segments[1] == "packages") {

                            string token = _request.Headers["authorization"];

                            var tokenWithoutBasic = token.Remove(0, 6);
                            var tokenExists = _userDatabaseController.VerifyUserToken(tokenWithoutBasic);

                            if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                                _response.StatusCode = 401;
                                _response.SetContent("Unauthorized");
                                return;
                            }
                            var user = _userDatabaseController.GetByToken(token);
                            int userCoins = _userDatabaseController.GetCoins(tokenWithoutBasic);

                            if (userCoins >= 5) {

                                var userId = user.Id;
                                bool packageAcquired = _deckDatabaseController.acquirePackage(userId);
                                _userDatabaseController.PackageAcquired(tokenWithoutBasic);

                                if (packageAcquired == false) {
                                    _response.StatusCode = 400;
                                    _response.SetContent("No Packages available.");
                                    break;
                                } else {
                                    _response.StatusCode = 200;
                                    _response.SetContent("Package acquired");
                                }

                            } else {
                                _response.StatusCode = 400;
                                _response.SetContent("Error, not enough Coins.");
                            }
                        } else {
                            _response.StatusCode = 400;
                            _response.SetContent("Error: Wrong URL");
                        }
                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent("Invalid Request");
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
