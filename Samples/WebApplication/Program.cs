using DynaCore;
using DynaCore.NLog;
using DynaCore.Web;

namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DynaCoreAppBuilder.Instance
                .UseUtcTimes()
                .UseNLog()
                .UseWebApi("WebApplication")
                    .WithSwagger()
                    .Build();
        }
    }
}
