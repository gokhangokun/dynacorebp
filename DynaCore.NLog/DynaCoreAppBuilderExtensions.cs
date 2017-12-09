using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace DynaCore.NLog
{
    public static class DynaCoreAppBuilderExtensions
    {
        public static DynaCoreAppBuilder UseNLog(this DynaCoreAppBuilder builder)
        {
            builder.AfterBuild(() =>
            {
                IConfiguration configuration = builder.DataStore.Get<IConfiguration>(Constants.Configuration);

                if (!String.IsNullOrWhiteSpace(configuration["NLogConfig"]))
                {
                    File.WriteAllText("nlog.config", configuration["NLogConfig"], Encoding.UTF8);
                }

                ILoggerFactory loggerFactory = builder.DataStore.Get<ILoggerFactory>(Constants.LoggerFactory);
                loggerFactory.AddNLog();
            });

            return builder;
        }
    }
}