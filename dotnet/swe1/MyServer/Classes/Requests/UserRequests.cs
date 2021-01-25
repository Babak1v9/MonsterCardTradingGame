using System;
using System.IO;
using MTCGserver.Interfaces;
using System.Text.Json;
using MTCGserver.Classes.DB;

namespace MTCGserver.Classes.RequestHandlers {
    class UserRequests : IMyRequests {

        private Request _request;
        private Response _response;
        private UserDatabaseController userDBController = new UserDatabaseController();
        private string _responseJson;

        public UserRequests(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
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
                        var tokenWithoutBasic = token.Remove(0, 6);
                        var user = userDBController.GetByToken(token);

                        if (_request.Url.Segments.Length == 1) {

                            var users = userDBController.GetUsers();
                            if (users == null) {
                                _response.StatusCode = 404;
                                _response.SetContent("users not found");
                            } else {
                                _responseJson = JsonSerializer.Serialize(users);
                                _response.StatusCode = 200;
                                _response.SetContent("Users: " + Environment.NewLine + _responseJson);
                            }
                        } else {

                            var requestedUser = userDBController.GetByUserName(_request.Url.Segments[1]);
                            if (requestedUser == null) {
                                _response.StatusCode = 404;
                                _response.SetContent("Requested User not found");
                                return;
                            }

                            var tokenRequestedUser = requestedUser.Token;
                            if (tokenRequestedUser == tokenWithoutBasic) {

                                _responseJson = JsonSerializer.Serialize(user);
                                _response.StatusCode = 200;
                                _response.SetContent("User: " + Environment.NewLine + _responseJson);
                            } else {
                                _response.StatusCode = 401;
                                _response.SetContent("Unauthorized");
                            }
                        }
                    } catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.SendException(e);
                    }
                    break;

                case "PUT":
                    
                    if (!_request.Headers.ContainsKey("authorization") || _request.Headers["authorization"] != $"Basic {_request.Url.Segments[1]}-mtcgToken") {
                        _response.StatusCode = 401;
                        _response.SetContent("Unauthorized");
                        return;
                    }
                    try {
                        var userToUpdate = _request.Url.Segments[1];
                        var userUpdatesToDo = JsonSerializer.Deserialize<User>(_request.ContentString);
                        userDBController.Update(userUpdatesToDo, userToUpdate);
                        var userUpdatedResponse = userDBController.GetByToken(_request.Headers["authorization"]); 
                        _responseJson = JsonSerializer.Serialize(userUpdatedResponse);
                        _response.StatusCode = 200;
                        _response.SetContent(Environment.NewLine + "Updated User: " + Environment.NewLine + _responseJson + Environment.NewLine);
                    }
                    catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.SendException(e);
                    }
                    break;

                case "POST":
                    try {

                        var userToCreate = JsonSerializer.Deserialize<User>(_request.ContentString);

                        var checkUser = userDBController.GetByUserName(userToCreate.Username);
                        if (checkUser != null) {

                            _response.StatusCode = 400;
                            _response.SetContent("User already exists.");
                            return;
                        }

                        userDBController.Insert(userToCreate.Username, userToCreate.Password);
                        _response.StatusCode = 200;
                        _response.SetContent("User created.");

                    }
                    catch (Exception e) {
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
