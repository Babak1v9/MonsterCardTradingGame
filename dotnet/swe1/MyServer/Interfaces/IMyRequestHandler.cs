using _Server.Classes;
using System.IO;

namespace _Server.Interfaces {
    public interface IMyRequestHandler {

        Request Request { get; }

        Response Response { get; set; }

        void ExecuteTask();

        void SendResponse(Stream stream);

    }
}
