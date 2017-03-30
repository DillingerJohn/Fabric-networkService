using System;

namespace NetworkService.Core.Logging
{
    public interface ILogger
    {
        void Log(string level, string message);
        void Log(string level, string message, Exception ex);
    }
}
