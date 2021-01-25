using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.DB_Stuff;

namespace MyServer.Classes.RequestHandlers {
    class TransactionsRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private DeckDataBaseController deckDBController = new DeckDataBaseController();
        private UserDatabaseController userDBController = new UserDatabaseController();

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

                        bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                        if (!Authentication) {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        if (_request.Url.Segments[1] == "packages") {

                            string token = _request.Headers["authorization"];
                            var tokenWithoutBasic = token.Remove(0, 6);

                            var user = userDBController.GetByToken(token);
                            int userCoins = userDBController.GetCoins(tokenWithoutBasic);

                            if (userCoins >= 5) {

                                var userId = user.Id;
                                bool packageAcquired = deckDBController.acquirePackage(userId);
                                userDBController.PackageAcquired(tokenWithoutBasic);

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
                            _response.SetContent("Invalid URL, add /packages after transactions.");
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
                    _response.InvalidURL();
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
