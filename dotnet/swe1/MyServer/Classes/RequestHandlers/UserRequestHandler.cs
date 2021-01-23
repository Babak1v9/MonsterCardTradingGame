using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.Data;
using System.Text.Json;

namespace MyServer.Classes.RequestHandlers {
    class UserRequestHandler : IMyRequestHandler {

        private Request _request;

        private string _responseJson;
        private Response _response;
        private UserDatabaseController _userDatabaseController = new UserDatabaseController();

        public UserRequestHandler(Request request) {
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
                    if (_request.Url.Segments.Length == 1) {

                        var users = _userDatabaseController.GetUsers();

                        if (users == null) {
                            _response.StatusCode = 404;
                            _response.SetContent("users not found");
                        } else {
                            _responseJson = JsonSerializer.Serialize(users);
                            Console.WriteLine("responsejson>: " + _responseJson);
                            _response.StatusCode = 200;
                            _response.SetContent(_responseJson);
                        }

                    } else {

                        var user = _request.Url.Parameter.ContainsKey("deck") && _request.Url.Parameter["deck"].Equals("true")
                            ? _userDatabaseController.GetByUserName(_request.Url.Segments[1], true) : _userDatabaseController.GetByUserName(_request.Url.Segments[1]);

                        if (user == null) {
                            _response.StatusCode = 404;
                            _response.SetContent("Request user not found");
                        } else {
                            _responseJson = JsonSerializer.Serialize(user);
                            _response.StatusCode = 200;
                            _response.SetContent(_responseJson);
                        }
                    }
                    break;

                case "PUT":
                    // if needs to be changed
                    // check if token is in session handler, doesnt exist yet
                    if (!_request.Headers.ContainsKey("authorization") || _request.Headers["authorization"] != $"Basic {_request.Url.Segments[1]}-mtcgToken") {
                        _response.StatusCode = 401;
                        _response.SetContent("Unauthorized");
                        return;
                    }
                    try {
                        var userToUpdate = _request.Url.Segments[1];
                        var userUpdatesToDo = JsonSerializer.Deserialize<User>(_request.ContentString);
                        _userDatabaseController.Update(userUpdatesToDo, userToUpdate); //checks with props are not null and updates them
                        var userUpdatedResponse = _userDatabaseController.GetByToken(_request.Headers["authorization"]); //authorization not working yet?
                        _responseJson = JsonSerializer.Serialize(userUpdatedResponse);
                        _response.StatusCode = 200;
                        _response.SetContent(_responseJson);
                    }
                    catch (Exception) {
                        _response.StatusCode = 400;
                        _response.SetContent("Could not update");
                    }
                    break;

                case "POST":
                    try {
                        var userToCreate = JsonSerializer.Deserialize<User>(_request.ContentString);
                        _userDatabaseController.Insert(userToCreate.Username, userToCreate.Password);
                        _response.StatusCode = 200;
                        _response.SetContent("User created.");
                    }
                    catch (Exception e) {
                        Console.WriteLine("e message: " + e.Message);
                        Console.WriteLine(" e source: " + e.Source);
                        Console.WriteLine("e.stacktrace: " + e.StackTrace);
                        _response.StatusCode = 400;
                        _response.SetContent("User already exists or password incorrect");
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
