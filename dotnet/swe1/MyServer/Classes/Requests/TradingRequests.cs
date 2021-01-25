using System;
using System.Collections.Generic;
using System.IO;
using MTCGserver.Classes;
using MTCGserver.Interfaces;
using MTCGserver.Classes.Battle_Logic;
using MTCGserver.Classes.DB;
using Newtonsoft.Json;

namespace MTCGserver.Classes.RequestHandlers {
    class TradingRequests : IMyRequests {

        private Request _request;
        private Response _response;
        private DeckDataBaseController deckDBController = new DeckDataBaseController();
        private UserDatabaseController userDBController = new UserDatabaseController();
        public Request Request => _request;
        public Response Response {
            get => _response;
            set { _response = value; }
        }
        public TradingRequests(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                case "GET":
                    try {

                        bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                        if (!Authentication) {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        string token = _request.Headers["authorization"];
                        string tradingDeals="";

                        List<Card> Cards = new List<Card>();
                        List<String> CardsInfo = new List<String>();

                        var user = userDBController.GetByToken(token);
                        (Cards, CardsInfo) = deckDBController.GetTradingDeals(user.Id);

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
                        _response.SetContent("Trading Deals: " + Environment.NewLine + tradingDeals);

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.SendException(e);
                    }
                    break;

                case "POST":
                    try {

                        bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                        if (!Authentication) {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        string token = _request.Headers["authorization"];
                        var currentUser = userDBController.GetByToken(token);

                        if (_request.Url.Segments.Length == 1) {

                            dynamic tempType = new { id = "", cardToTrade = "", Type = "", DamageRequirement = "" };
                            dynamic tradingDeal = JsonConvert.DeserializeAnonymousType(_request.ContentString, tempType);

                            var damageRequirement = String.IsNullOrEmpty(tradingDeal.DamageRequirement) ? "15" : tradingDeal.DamageRequirement;
                            bool success = deckDBController.CreateTradingDeal(tradingDeal.id, currentUser.Id, tradingDeal.cardToTrade, tradingDeal.Type, damageRequirement);
                            if (success) {
                                _response.StatusCode = 200;
                                _response.SetContent("Trade created successfully.");
                            } else {
                                _response.StatusCode = 400;
                                _response.SetContent("Error while creating trade, selected card is currently in Deck.");
                            }

                        } else {

                            string tmpOffer =_request.ContentString;
                            var length = tmpOffer.Length;
                            string offeredCard = tmpOffer.Substring(1, length - 2);

                            string tradingId = _request.Url.Segments[1];
                            var tradeSuccess = deckDBController.TradeCards(tradingId, offeredCard, currentUser.Id);

                            if (tradeSuccess) {

                                _response.StatusCode = 200;
                                _response.SetContent("Cards traded successfully.");
                            } else {

                                _response.StatusCode = 400;
                                _response.SetContent("Buyer and Seller identical or offered Card is currently in Deck.");
                            }

                            
                        }

                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.SendException(e);
                    }
                    break;

                case "DELETE":
                    try {

                        bool Authentication = userDBController.AuthenticateUser(_request.Headers);
                        if (!Authentication) {
                            _response.UnauthenticatedUser();
                            return;
                        }

                        if (_request.Url.Segments.Length == 1) {
                            _response.StatusCode = 400;
                            _response.SetContent("Parameter missing: Trading ID.");
                        }

                        string tradingId = _request.Url.Segments[1];

                        deckDBController.DeleteTradingDeal(tradingId);
                        _response.StatusCode = 200;
                        _response.SetContent("Trade deleted successfully.");

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
