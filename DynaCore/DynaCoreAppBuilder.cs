using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using DynaCore.Domain.Abstractions;
using DynaCore.Domain.Objects.DateTimeProviders;

namespace DynaCore
{
    public class DynaCoreAppBuilder : IDynaCoreAppBuilder
    {
        private static DynaCoreAppBuilder _appBuilder;
        public static DynaCoreAppBuilder Instance => _appBuilder ?? (_appBuilder = new DynaCoreAppBuilder());

        private readonly List<Action> _beforeBuildTasks = new List<Action>();
        private readonly List<Action> _afterBuildTasks = new List<Action>();
        private IConfiguration _configuration;

        public InMemoryDataStore DataStore = new InMemoryDataStore();
        public IServiceCollection Services = new ServiceCollection();

        internal DynaCoreAppBuilder()
        {
            InitializeConfiguration();
            SetGlobalJsonSerializerSettings();
            InitializeLogger();
            InitializeDateTimeProvider();
        }

        public DynaCoreApp Build()
        {
            foreach (var task in _beforeBuildTasks)
            {
                task.Invoke();
            }

            BuildServiceProvider();
            DynaCoreApp.Instance = new DynaCoreApp(DataStore);

            foreach (var task in _afterBuildTasks)
            {
                task.Invoke();
            }

            return DynaCoreApp.Instance;
        }

        public void BeforeBuild(Action action)
        {
            _beforeBuildTasks.Add(action);
        }

        public void AfterBuild(Action action)
        {
            _afterBuildTasks.Add(action);
        }

        public DynaCoreAppBuilder UseLocalTimes()
        {
            DataStore.Delete(Constants.DateTimeProvider);
            DataStore.Set(Constants.DateTimeProvider, new LocalDateTimeProvider());

            return this;
        }

        public DynaCoreAppBuilder UseUtcTimes()
        {
            DataStore.Delete(Constants.DateTimeProvider);
            DataStore.Set(Constants.DateTimeProvider, new UtcDateTimeProvider());

            return this;
        }

        public DynaCoreAppBuilder ConfigureServices(Action<IServiceCollection, IConfiguration> action)
        {
            action.Invoke(Services, _configuration);

            return this;
        }

        private void SetGlobalJsonSerializerSettings()
        {
            IDateTimeProvider dateTimeProvider = DataStore.Get<IDateTimeProvider>(Constants.DateTimeProvider);

            if (dateTimeProvider != null && dateTimeProvider.Kind == DateTimeKind.Local)
            {
                DynaCoreApp.JsonSerializerSettings = GetJsonSerializerSettings(DateTimeZoneHandling.Local);
            }
            else if (dateTimeProvider != null && dateTimeProvider.Kind == DateTimeKind.Utc)
            {
                DynaCoreApp.JsonSerializerSettings = GetJsonSerializerSettings(DateTimeZoneHandling.Utc);
            }
            else
            {
                DynaCoreApp.JsonSerializerSettings = GetJsonSerializerSettings();
            }
        }

        private JsonSerializerSettings GetJsonSerializerSettings(DateTimeZoneHandling dateTimeZoneHandling = DateTimeZoneHandling.Utc)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = dateTimeZoneHandling
            };

            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            return settings;
        }

        private void BuildServiceProvider()
        {
            if (DataStore.Get(Constants.ServiceProvider) == null)
            {
                ServiceProvider serviceProvider = Services.BuildServiceProvider();
                DataStore.Set(Constants.ServiceProvider, serviceProvider);
            }
        }

        private void InitializeLogger()
        {
            IConfigureOptions<LoggerFilterOptions> options = new ConfigureOptions<LoggerFilterOptions>(filterOptions =>
            {
                filterOptions.MinLevel = LogLevel.Trace;
            });

            var loggerFactory = new LoggerFactory();
            DataStore.Set(Constants.LoggerFactory, loggerFactory);
            loggerFactory.AddConsole();

            Services.AddOptions();
            Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));
            Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            Services.TryAddEnumerable(ServiceDescriptor.Singleton(options));
        }

        private void InitializeConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.ci.json", true, true)
                .AddJsonFile("appsettings.test.json", true, true)
                .AddJsonFile("appsettings.staging.json", true, true)
                .AddJsonFile("appsettings.production.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            DataStore.Set(Constants.Configuration, _configuration);
        }

        private void InitializeDateTimeProvider()
        {
            DataStore.Set(Constants.DateTimeProvider, new UtcDateTimeProvider());
        }
    }
}