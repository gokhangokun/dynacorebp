using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using DynaCore.Web.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace DynaCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public InMemoryDataStore DataStore => DynaCoreAppBuilder.Instance.DataStore;

        public void ConfigureServices(IServiceCollection services)
        {
            foreach (ServiceDescriptor descriptor in DynaCoreAppBuilder.Instance.Services)
            {
                services.Add(descriptor);
            }

            services.AddMvc()
                .AddApplicationPart(Assembly.GetEntryAssembly())
                .AddMvcOptions(o =>
                {
                    o.InputFormatters.RemoveType<XmlDataContractSerializerInputFormatter>();
                    o.InputFormatters.RemoveType<XmlSerializerInputFormatter>();

                    o.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
                    o.OutputFormatters.RemoveType<StreamOutputFormatter>();
                    o.OutputFormatters.RemoveType<StringOutputFormatter>();
                    o.OutputFormatters.RemoveType<XmlDataContractSerializerOutputFormatter>();
                    o.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();

                    o.Filters.Add<GlobalExceptionFilter>();
                })
                .AddJsonOptions(o =>
                {
                    o.SerializerSettings.ContractResolver = DynaCoreApp.JsonSerializerSettings.ContractResolver;
                    o.SerializerSettings.Formatting = DynaCoreApp.JsonSerializerSettings.Formatting;
                    o.SerializerSettings.NullValueHandling = DynaCoreApp.JsonSerializerSettings.NullValueHandling;
                    o.SerializerSettings.DefaultValueHandling = DynaCoreApp.JsonSerializerSettings.DefaultValueHandling;
                    o.SerializerSettings.ReferenceLoopHandling = DynaCoreApp.JsonSerializerSettings.ReferenceLoopHandling;
                    o.SerializerSettings.DateTimeZoneHandling = DynaCoreApp.JsonSerializerSettings.DateTimeZoneHandling;
                    o.SerializerSettings.Converters.Clear();

                    foreach (JsonConverter converter in DynaCoreApp.JsonSerializerSettings.Converters)
                    {
                        o.SerializerSettings.Converters.Add(converter);
                    }
                });

            if (DataStore.Get<bool>(Constants.UseSwagger))
            {
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info { Title = DataStore.Get<string>(Constants.ApplicationName), Version = "v1" });
                    c.DescribeAllEnumsAsStrings();
                    c.DescribeStringEnumsInCamelCase();
                    c.IgnoreObsoleteActions();
                    c.IgnoreObsoleteProperties();
                });
            }

            DynaCoreAppBuilder.Instance.Services = services;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            DynaCoreAppBuilder.Instance.DataStore.Set(DynaCore.Constants.ServiceProvider, app.ApplicationServices);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc(r =>
            {
                if (DataStore.Get<bool>(Constants.UseSwagger))
                {
                    r.MapRoute(name: "Default",
                        template: "",
                        defaults: new {controller = "Help", action = "Index"});
                }
            });
            
            if (DataStore.Get<bool>(Constants.UseSwagger))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.InjectOnCompleteJavaScript("");
                });
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            DynaCoreApp.Instance.DataStore.Set(Constants.IsShuttingDown, true);
            Thread.Sleep(15000);
        }
    }
}