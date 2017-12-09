using System;
using Microsoft.Extensions.Logging;
using DynaCore;
using DynaCore.NLog;

namespace ConsoleApp
{
    class Program
    {
        static ILogger<Program> _logger;

        static void Main(string[] args)
        {
            DynaCoreAppBuilder.Instance
                .UseUtcTimes()
                .UseNLog()
                .Build();

            _logger = DynaCoreApp.Instance.LoggerFactory.CreateLogger<Program>();
            _logger.LogInformation("Log information");

            Console.Out.WriteLineAsync(DynaCoreApp.Instance.Configuration["key"]);
            Console.ReadKey();
        }
    }
}