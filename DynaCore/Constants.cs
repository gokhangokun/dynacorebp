﻿namespace DynaCore
{
    public struct Constants
    {
        public struct MessageTypes
        {
            public const string Error = "error";
            public const string Info = "info";
            public const string Warning = "warning";
            public const string Success = "success";
        }

        public const string DateTimeProvider = "DateTimeProvider";
        public const string ConfigManager = "ConfigManager";
        public const string ServiceCollection = "Services";
        public const string ServiceProvider = "ServiceProvider";
        public const string LoggerFactory = "LoggerFactory";
        public const string Configuration = "Configuration";
    }
}