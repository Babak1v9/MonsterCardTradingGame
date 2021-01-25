using MTCGserver.Classes;
using System.IO;

namespace MTCGserver.Interfaces {
    public interface IMyRequests {

        Request Request { get; }

        Response Response { get; set; }

        void ExecuteTask();

        void SendResponse(Stream stream);

    }
}
