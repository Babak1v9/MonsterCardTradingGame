using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.Battle_Stuff;
using MyServer.Classes.DB_Stuff;
using Newtonsoft.Json;

namespace MyServer.Classes.RequestHandlers {
    class TradingRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private DeckDataBaseController deckDBController = new DeckDataBaseController();
        private UserDatabaseController userDBController = new UserDatabaseController();
        public Request Request => _request;
        public Response Response {
            get => _response;
            set { _response = value; }
        }
        public TradingRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "GET":
                    try {

                        string token = _request.Headers["authorization"];
                        var tokenWithoutBasic = token.Remove(0, 6);
                        var tokenExists = userDBController.VerifyUserToken(tokenWithoutBasic);

                        if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                            _response.StatusCode = 401;
                            _response.SetContent(Environment.NewLine + "Unauthorized" + Environment.NewLine);
                            return;
                        }

                        string tradingDeals="";
                        List<Card> Cards = new List<Card>();
                        List<String> CardsInfo = new List<String>();
                        (Cards, CardsInfo) = deckDBController.GetTradingDeals();
                        int counter = 0;

                        if (Cards != null && CardsInfo != null) {
 
                            for(int i = 0; i < Cards.Count; i++) {
                                var secondRequirement = "";
                                var typeRequirement = "";
                                var cardType = Cards[i].CardType == 0 ? "Monster" : "Spell";

                                var cardElement = Cards[i].ElementType switch {
                                    0 => "Normal",
                                    1 => "Water",
                                    2 => "Fire",
                                    _ => "Normal"
                                };

                                if (CardsInfo[counter + 2].Length == 1) {
                                    typeRequirement = CardsInfo[counter + 2] == "0" ? "Monster" : "Spell";
                                } else {
                                    typeRequirement = CardsInfo[counter + 2];
                                }

                                if (String.IsNullOrEmpty(CardsInfo[counter + 3].ToString())) {
                                    secondRequirement = CardsInfo[counter + 4].ToString();
                                
                                } else {
                                    secondRequirement = CardsInfo[counter+3] switch {
                                        "0" => "Normal",
                                        "1" => "Water",
                                        "2" => "Fire",
                                        _ => "Normal"
                                    };
                                }

                                tradingDeals += Environment.NewLine + "Trading_ID: " + CardsInfo[counter] + Environment.NewLine + " UserId: " + CardsInfo[counter+1] + Environment.NewLine + " Card: " + Cards[i].Name + Environment.NewLine + " Type: " + cardType + Environment.NewLine + " Element: " + cardElement + Environment.NewLine + " Damage: " + Cards[i].Damage + Environment.NewLine + " Type Requirement: " + typeRequirement + Environment.NewLine + " Element or Damage Requirement: " + secondRequirement + Environment.NewLine+ Environment.NewLine;

                                counter += 5;
                            }   
                        }
                        _response.StatusCode = 200;
                        _response.SetContent(Environment.NewLine + "Trading Deals: " + Environment.NewLine + tradingDeals + Environment.NewLine);

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent(Environment.NewLine + "Error while requesting trading deals." + Environment.NewLine);
                    }
                    break;

                case "POST":
                    try {

                        string token = _request.Headers["authorization"];
                        var tokenWithoutBasic = token.Remove(0, 6);
                        var tokenExists = userDBController.VerifyUserToken(tokenWithoutBasic);

                        if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                            _response.StatusCode = 401;
                            _response.SetContent(Environment.NewLine + "Unauthorized" + Environment.NewLine);
                            return;
                        }

                        var currentUser = userDBController.GetByToken(token);

                        dynamic tempType = new { id = "", cardToTrade = "", Type = "", DamageRequirement ="" };
                        dynamic tradingDeal = JsonConvert.DeserializeAnonymousType(_request.ContentString, tempType);

                        var damageRequirement = String.IsNullOrEmpty(tradingDeal.DamageRequirement) ? "15" : tradingDeal.DamageRequirement;
                        deckDBController.CreateTradingDeal(tradingDeal.id, currentUser.Id, tradingDeal.cardToTrade, tradingDeal.Type, damageRequirement);
                        _response.StatusCode = 200;
                        _response.SetContent(Environment.NewLine + "Trade created successfully." + Environment.NewLine);

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent(Environment.NewLine + "Insufficient Parameters for creating trading deal." + Environment.NewLine);
                    }
                    break;

                case "DELETE":
                    try {

                        string token = _request.Headers["authorization"];
                        var tokenWithoutBasic = token.Remove(0, 6);
                        var tokenExists = userDBController.VerifyUserToken(tokenWithoutBasic);

                        if (!_request.Headers.ContainsKey("authorization") || tokenExists != true) {
                            _response.StatusCode = 401;
                            _response.SetContent(Environment.NewLine + "Unable to delete trading deal." + Environment.NewLine);
                            return;
                        }

                        if (_request.Url.Segments.Length == 1) {

                            _response.StatusCode = 400;
                            _response.SetContent(Environment.NewLine + "Parameter missing: Trading ID." + Environment.NewLine);
                        }

                        string tradingId = _request.Url.Segments[1];

                        deckDBController.DeleteTradingDeal(tradingId);
                        _response.StatusCode = 200;
                        _response.SetContent(Environment.NewLine + "Trade deleted successfully." + Environment.NewLine);

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent(Environment.NewLine + "Invalid Trading ID entered." + Environment.NewLine);
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
