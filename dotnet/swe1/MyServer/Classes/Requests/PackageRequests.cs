using System;
using System.IO;
using MTCGserver.Interfaces;
using MTCGserver.Classes.DB;
using Newtonsoft.Json;

namespace MTCGserver.Classes.RequestHandlers {
    class PackageRequests : IMyRequests {

        private Request _request;
        private Response _response;
        private DeckDataBaseController deckDBController = new DeckDataBaseController();

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }

        public PackageRequests(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "POST":
                    try {
                        if (!_request.Headers.ContainsKey("authorization") || _request.Headers["authorization"] != $"Basic admin-mtcgToken") {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        var packageId = deckDBController.createPackage();

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
                            deckDBController.createCard(cardID, cardName, cardDamage, cardType, elementType, packageId);
                        }
                        
                        _response.StatusCode = 200;
                        _response.SetContent("Package created" + Environment.NewLine);
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
