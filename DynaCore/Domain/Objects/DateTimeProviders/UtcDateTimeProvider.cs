using System;
using DynaCore.Domain.Abstractions;

namespace DynaCore.Domain.Objects.DateTimeProviders
{
    public class UtcDateTimeProvider : IDateTimeProvider
    {
        public DateTimeKind Kind => DateTimeKind.Utc;
        public DateTime Now => DateTime.UtcNow;
    }
}