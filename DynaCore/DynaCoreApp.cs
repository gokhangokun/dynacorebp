using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DynaCore.Domain.Abstractions;

namespace DynaCore
{
    public class DynaCoreApp
    {
        public static JsonSerializerSettings JsonSerializerSettings { get; set; }
        public static DynaCoreApp Instance { get; set; }

        public InMemoryDataStore DataStore { get; set; }

        public IDateTimeProvider DateTimeProvider => DataStore.Get<IDateTimeProvider>(Constants.DateTimeProvider);
        public ILoggerFactory LoggerFactory => DataStore.Get<ILoggerFactory>(Constants.LoggerFactory);
        public IConfiguration Configuration => DataStore.Get<IConfiguration>(Constants.Configuration);
        public IServiceProvider ServiceProvider => DataStore.Get<IServiceProvider>(Constants.ServiceProvider);

        public DynaCoreApp(InMemoryDataStore dataStore)
        {
            DataStore = dataStore;
        }
    }
}