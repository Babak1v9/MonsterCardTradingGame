using System;
using System.IO;
using _Server.Classes;
using _Server.Interfaces;
using MyServer.Classes.DB_Stuff;

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
            throw new NotImplementedException();
        }

        public void SendResponse(Stream stream) {
            _response.Send(stream);
        }
    }
}
