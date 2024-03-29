﻿using System;
using System.IO;
using System.Text.Json;
using MTCGserver.Interfaces;
using MTCGserver.Classes.DB;

namespace MTCGserver.Classes.RequestHandlers {
    class CardsRequests : IMyRequests {

        private Request _request;
        private Response _response;
        private string _responseJson;
        private DeckDataBaseController deckDBController = new DeckDataBaseController();
        private UserDatabaseController userDBController = new UserDatabaseController();

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }

        public CardsRequests(Request request) {
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
                        var user = userDBController.GetByToken(token);
                        var userId = user.Id;

                        user.Deck = deckDBController.ShowAllCards(userId);

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
