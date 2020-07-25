using System.Threading.Tasks;

namespace Serilog.Sinks.AspNetCore.SignalR.Interfaces
{
    /// <summary>
    /// The strongly types interface for SignalR Hubs to send over Serilog
    /// </summary>
    public interface IHub
    {
        /// <summary>
        /// Send a message as a string
        /// </summary>
        /// <param name="message">The message you want to send.</param>
        /// <returns>Task.</returns>
        Task SendLogAsString(string message);

        /// <summary>
        /// Send a message as an object
        /// </summary>
        /// <param name="messageObject">The message you want to send.</param>
        /// <returns>Task.</returns>
        Task SendLogAsObject(object messageObject);
    }
}