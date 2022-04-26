using DataAccess.Repositories;

namespace DataAccess.Utility
{
    public class LoggerHelper
    {
        private readonly ILogger logger;

        public LoggerHelper(ILogger logger)
        {
            this.logger = logger;
        }

        public ILogger GetLogger()
        {
            return logger;
        }
    }
}