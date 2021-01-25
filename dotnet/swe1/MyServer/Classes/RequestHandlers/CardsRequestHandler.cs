using System;
using System.IO;
using System.Text.Json;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.DB_Stuff;

namespace MyServer.Classes.RequestHandlers {
    class CardsRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private string _responseJson;
        private DeckDataBaseController _deckDatabaseController = new DeckDataBaseController();
        private UserDatabaseController _userDatabaseController = new UserDatabaseController();

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }

        public CardsRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "GET":
                    try {

                        string token = _request.Headers["authorization"];

                        var tokenWithoutBasic = token.Remove(0, 6);
                        var tokenExists = _userDatabaseController.VerifyUserToken(tokenWithoutBasic);

                        if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                            _response.StatusCode = 401;
                            _response.SetContent("Unauthorized");
                            return;
                        }

                        var user = _userDatabaseController.GetByToken(token);
                        var userId = user.Id;

                        user.Deck = _deckDatabaseController.ShowAllCards(userId);

                        if (user == null) {
                            _response.StatusCode = 404;
                            _response.SetContent("User not found");
                        } else {
                            _responseJson = JsonSerializer.Serialize(user.Deck);
                            _response.StatusCode = 200;
                            _response.SetContent(_responseJson);
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
