namespace DynaCore.Web
{
    public static class DynaCoreAppBuilderExtensions
    {
        public static DynaCoreWebhostBuilder UseWebApi(this DynaCoreAppBuilder builder, string applicationName, string[] args = null)
        {
            return new DynaCoreWebhostBuilder(builder, applicationName, args);
        }
    }
}