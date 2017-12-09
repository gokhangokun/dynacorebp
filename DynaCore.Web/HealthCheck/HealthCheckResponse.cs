using System.Collections.Generic;
using DynaCore.Domain.Responses;

namespace DynaCore.Web.HealthCheck
{
    public class HealthCheckResponse : BaseResponse
    {
        public string DockerImageName { get; set; }
        public List<HealthCheckResult> Results { get; set; }
    }
}