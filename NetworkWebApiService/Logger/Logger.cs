using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
//using log4net.Config;
using NetworkService.Core.Logging;


namespace NetworkService.Logging
{
    internal class Logger : ILogger
    {
        private static readonly Lazy<Logger> LazyLogger = new Lazy<Logger>(() => new Logger());
        //Here is the once-per-class call to initialize the log object
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static ILogger Instanse
        {
            get {
                return LazyLogger.Value;
            }
        }
        public Logger()
        { }

        public void Log(string level, string message)
        {
            try
            {
                switch (level)
                {
                    case "Info":
                        logger.Info(message);
                        break;
                    case "Warn":
                        logger.Warn(message);
                        break;
                    case "Error":
                        logger.Error(message);
                        break;
                    case "Fatal":
                        logger.Fatal(message);
                        break;
                    case "Debug":
                        logger.Debug(message);
                        break;
                    default:
                        logger.Info(message);
                        break;
                }
            }
            catch {
                throw new NotImplementedException();
            }
        }

        public void Log(string level, string message, Exception ex)
        {
            try
            {
                switch (level)
                {
                    case "Info":
                        logger.Info(message, ex);
                        break;
                    case "Warn":
                        logger.Warn(message, ex);
                        break;
                    case "Error":
                        logger.Error(message, ex);
                        break;
                    case "Fatal":
                        logger.Fatal(message, ex);
                        break;
                    case "Debug":
                        logger.Debug(message, ex);
                        break;
                    default:
                        logger.Info(message, ex);
                        break;
                }
            }
            catch
            {
                throw new NotImplementedException();
            }
        }
    }
}
