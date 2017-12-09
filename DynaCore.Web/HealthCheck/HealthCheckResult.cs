﻿namespace DynaCore.Web.HealthCheck
{
    public class HealthCheckResult
    {
        public string Key { get; set; }

        public string Message { get; set; }

        public bool? Success { get; set; }

        public bool IsCtirical { get; set; }
    }
}