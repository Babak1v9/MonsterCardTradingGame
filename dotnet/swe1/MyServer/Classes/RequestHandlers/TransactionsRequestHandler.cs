using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public void verifyUserToken() {

        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "POST":
                    try {
                        Console.WriteLine("test");

                        if (_request.Url.Segments[1] == "packages") {

                            string token = _request.Headers["authorization"];
                            Console.WriteLine(token);
                            var tokenExists = _userDatabaseController.verifyUserToken(token);

                            if (!_request.Headers.ContainsKey("authorization") || _request.Headers["authorization"] != $"Basic test-mtcgToken") {
                                _response.StatusCode = 401;
                                _response.SetContent("Unauthorized");
                                return;
                            }

                            _deckDatabaseController.acquirePackage("Basic kenboec-mtcgToken");

                            _response.StatusCode = 200;
                            _response.SetContent("Package acquired");
                        } else {
                            _response.StatusCode = 400;
                            _response.SetContent("Error: Wrong URL");
                        }
                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent("Request body invalid");
                    }
                    break;

                default:
                    _response.StatusCode = 400;
                    _response.SetContent("Invalid HTTP Method");
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
