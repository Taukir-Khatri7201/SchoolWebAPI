using SchoolWebAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SchoolWebAPI.Utility
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