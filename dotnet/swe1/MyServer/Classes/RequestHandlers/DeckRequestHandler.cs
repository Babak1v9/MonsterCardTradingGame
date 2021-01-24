using System;
using System.IO;
using System.Text.Json;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.DB_Stuff;
using Newtonsoft.Json;

namespace MyServer.Classes.RequestHandlers {
    class DeckRequestHandler : IMyRequestHandler {

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

        public DeckRequestHandler(Request request) {
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

                        if (_request.Url.Parameter.ContainsKey("format") && _request.Url.Parameter["format"].Equals("plain")) {
                            _response.ContentType = "text/plain";
                        }

                        var user = _userDatabaseController.GetByToken(token);
                        var userId = user.Id;

                        user.Deck = _deckDatabaseController.GetDeck(userId);

                        if (user == null) {
                            _response.StatusCode = 404;
                            _response.SetContent("User not found");
                        } else {
                            _responseJson = System.Text.Json.JsonSerializer.Serialize(user.Deck);
                            _response.StatusCode = 200;
                            _response.SetContent("User Deck:\n" + _responseJson);
                        }

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent("Invalid Request.");
                    }
                    break;
                case "PUT":
                    try {

                        string token = _request.Headers["authorization"];

                        var tokenWithoutBasic = token.Remove(0, 6);
                        var tokenExists = _userDatabaseController.VerifyUserToken(tokenWithoutBasic);

                        if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                            _response.StatusCode = 401;
                            _response.SetContent("Unauthorized.");
                            return;
                        }

                        var Cards = JsonConvert.DeserializeObject<string[]>(_request.ContentString);
                        Console.WriteLine(Cards.Length);
                        if (Cards.Length == 4) {

                            var user = _userDatabaseController.GetByToken(token);
                            var userId = user.Id;

                            var doesExist =_deckDatabaseController.checkUserDeck(userId);

                            if (doesExist != true) {
                                _deckDatabaseController.CardsToDeck(Cards, userId);
                                _response.StatusCode = 200;
                                _response.SetContent("Deck created.");
                            } else {

                                user.Deck = _deckDatabaseController.GetDeck(userId);

                                _responseJson = System.Text.Json.JsonSerializer.Serialize(user.Deck);
                                _response.StatusCode = 200;
                                _response.SetContent("User Deck already exists:\n" + _responseJson);
 
                            }
                        } else {
                            _response.StatusCode = 400;
                            _response.SetContent("Not enough Cards provided.");
                        }

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent("Invalid Request.");
                        
                    } 
                    break;

                default:
                    _response.StatusCode = 400;
                    _response.SetContent("Invalid HTTP Method.");
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
