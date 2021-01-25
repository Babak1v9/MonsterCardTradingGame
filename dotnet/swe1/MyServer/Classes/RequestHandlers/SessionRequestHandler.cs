using System;
using System.IO;
using System.Text.Json;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.Data;

namespace MyServer.Classes.RequestHandlers {
    class SessionRequestHandler : IMyRequestHandler {

        private Request _request;
        private Response _response;
        private UserDatabaseController _userDatabaseController = new UserDatabaseController();

        public Request Request => _request;

        public Response Response {
            get => _response;
            set { _response = value; }
        }
        
        public SessionRequestHandler(Request request) {
            _request = request;
            _response = new Response { ContentType = "application/Json" };
        }

        public void ExecuteTask() {
            switch (_request.Method) {
                
                case "POST":
                    try {
                        var userToBeLoggedIn = JsonSerializer.Deserialize<User>(_request.ContentString);
                        var returnedUser = _userDatabaseController.GetByUserName(userToBeLoggedIn.Username);

                        var sha256 =_userDatabaseController.CalcSHA256(userToBeLoggedIn.Password, returnedUser.Salt);

                        if (sha256 == returnedUser.Password) {
                            _response.StatusCode = 200;
                            _response.SetContent("User login successful.");

                        } else {
                            _response.StatusCode = 401;
                            _response.SetContent("User login failed.");

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
                    _response.invalidURL();
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
