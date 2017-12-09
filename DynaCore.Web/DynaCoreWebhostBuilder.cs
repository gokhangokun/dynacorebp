using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DynaCore.Web
{
    public class DynaCoreWebhostBuilder
    {
        private readonly string[] _args;
        private readonly string _applicationName;
        private readonly DynaCoreAppBuilder _DynaCoreAppBuilder;

        private bool _withIisIntegration = false;

        public DynaCoreWebhostBuilder(DynaCoreAppBuilder DynaCoreAppBuilder, string applicationName, string[] args)
        {
            _DynaCoreAppBuilder = DynaCoreAppBuilder;
            _applicationName = applicationName;
            _args = args;

            _DynaCoreAppBuilder.DataStore.Set(Constants.IsShuttingDown, false);
            _DynaCoreAppBuilder.DataStore.Set(Constants.UseSwagger, false);
        }

        public DynaCoreWebhostBuilder WithUrl(string rootUrl)
        {
            if (!String.IsNullOrEmpty(rootUrl))
            {
                _DynaCoreAppBuilder.DataStore.Set(Constants.ApiRootUrl, rootUrl.TrimEnd('/'));
            }

            return this;
        }

        public DynaCoreWebhostBuilder WithSwagger()
        {
            _DynaCoreAppBuilder.DataStore.Set(Constants.UseSwagger, true);

            return this;
        }

        public DynaCoreWebhostBuilder WithIisIntegration()
        {
            _withIisIntegration = true;
            return this;
        }

        public void Build()
        {
            _DynaCoreAppBuilder.DataStore.Set(Constants.ApplicationName, _applicationName);

            _DynaCoreAppBuilder.BeforeBuild(() =>
            {
                IWebHost webhost = CreateDefaultBuilder(_args)
                    .UseStartup<Startup>()
                    .Build();

                _DynaCoreAppBuilder.DataStore.Set(Constants.WebHost, webhost);
            });

            DynaCoreApp app = _DynaCoreAppBuilder.Build();

            ServiceProvider serviceProvider = app.DataStore.Get<ServiceProvider>(DynaCore.Constants.ServiceProvider);
            ILogger<DynaCoreWebhostBuilder> logger = serviceProvider.GetService<ILogger<DynaCoreWebhostBuilder>>();
            logger.LogInformation($"{_applicationName} started.");

            app.DataStore.Get<IWebHost>(Constants.WebHost).Run();
        }

        public IWebHostBuilder CreateDefaultBuilder(string[] args)
        {
            var webHostBuilder = new WebHostBuilder();

            webHostBuilder.UseKestrel();
            webHostBuilder.UseContentRoot(Directory.GetCurrentDirectory());
            webHostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                IHostingEnvironment hostingEnvironment = hostingContext.HostingEnvironment;
                config.AddJsonFile("appsettings.json", true, true).AddJsonFile(string.Format("appsettings.{0}.json", (object)hostingEnvironment.EnvironmentName), true, true);
                if (hostingEnvironment.IsDevelopment())
                {
                    Assembly assembly = Assembly.Load(new AssemblyName(hostingEnvironment.ApplicationName));
                    if (assembly != null)
                        config.AddUserSecrets(assembly, true);
                }
                config.AddEnvironmentVariables();
                if (args == null)
                    return;
                config.AddCommandLine(args);
            });
            webHostBuilder.ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
            });

            if (_withIisIntegration)
            {
                webHostBuilder.UseIISIntegration();
            }

            webHostBuilder.UseDefaultServiceProvider((context, options) => options.ValidateScopes = context.HostingEnvironment.IsDevelopment());

            return webHostBuilder;
        }
    }
}