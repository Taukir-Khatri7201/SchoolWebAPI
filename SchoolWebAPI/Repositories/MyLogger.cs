using NLog;

namespace SchoolWebAPI.Repositories
{
    public class MyLogger : ILogger
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}