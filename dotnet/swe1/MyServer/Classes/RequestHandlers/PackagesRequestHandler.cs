using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.DB_Stuff;
using Newtonsoft.Json;

namespace MyServer.Classes.RequestHandlers {
    class PackagesRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private DeckDataBaseController _deckDatabaseController = new DeckDataBaseController();

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }

        public PackagesRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "POST":
                    try {

                        if (!_request.Headers.ContainsKey("authorization") || _request.Headers["authorization"] != $"Basic admin-mtcgToken") {
                            _response.StatusCode = 401;
                            _response.SetContent("Unauthorized");
                            return;
                        }

                        var packageId = _deckDatabaseController.createPackage();

                        var cardJsons = _request.ContentString.Split("}, {");
                        cardJsons[0] = (cardJsons[0].Replace("[", "") + "}");

                        for (var i = 1; i < cardJsons.Length - 1; i++) {
                            cardJsons[i] = ("{" + cardJsons[i] + "}");
                        }

                        cardJsons[^1] = ("{" + cardJsons[^1].Replace("]", ""));

                       
                        var tempCardType = new { id = "", name = "", damage = "" };

                        for (var i = 0; i < cardJsons.Length; i++) {
                            var tempCard = JsonConvert.DeserializeAnonymousType(cardJsons[i], tempCardType);

                            var cardID = tempCard.id;
                            var cardName = tempCard.name;
                            var cardDamage = tempCard.damage;
                            string cardType;
                            string elementType;

                            if (cardName.Contains("Water")) {
                                elementType = "1";

                            } else if (cardName.Contains("Fire")) {
                                elementType = "2";
                            } else {
                                elementType = "0";
                            }

                            if (cardName.Contains("Spell")) {
                                cardType = "1";
                            } else {
                                cardType = "0";
                            }

                            _deckDatabaseController.createCard(cardID, cardName, cardDamage, cardType, elementType, packageId);
                        }

                        
                        _response.StatusCode = 200;
                        _response.SetContent("Package created");
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
                    _response.SetContent("Invalid Http Method");
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
