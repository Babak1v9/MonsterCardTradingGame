using System;
using System.IO;
using System.Text.Json;
using MTCGserver.Interfaces;
using MTCGserver.Classes.Other;
using MTCGserver.Classes.HTTP;
using MTCGserver.Classes.DB;

namespace MTCGserver.Classes.Requests {
    class SessionRequestHandler : IRequestHandler {

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
                        var test = returnedUser.Username;
                        var test2 = returnedUser.Password;
                        var test3 = returnedUser.Salt;
                        Console.WriteLine("fetchedUser " + test + " " + test2 + " " + test3);

                        var sha256 =_userDatabaseController.CalcSHA256(userToBeLoggedIn.Password, returnedUser.Salt);
                        Console.WriteLine("new hash: " + sha256);
                        Console.WriteLine("old hash: " + returnedUser.Password);

                        if (sha256 == returnedUser.Password) {
                            Console.WriteLine("User login successful.");
                            _response.StatusCode = 200;
                            _response.SetContent("User login successful.");

                        } else {

                            Console.WriteLine("Password or User incorrect.");
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
                    Console.WriteLine("Invalid Http Method");
                    break;
            }
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
