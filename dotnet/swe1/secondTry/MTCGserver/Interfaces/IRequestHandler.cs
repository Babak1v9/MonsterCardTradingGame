using System.IO;
using MTCGserver.Classes.HTTP;

namespace MTCGserver.Interfaces {
    public interface IRequestHandler {

        Request Request { get; }

        Response Response { get; set; }

        void ExecuteTask();

        void SendResponse(Stream stream);

    }
}
