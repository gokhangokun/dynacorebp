using System;
using DynaCore.Domain.Abstractions;

namespace DynaCore.Domain.Objects.DateTimeProviders
{
    public class LocalDateTimeProvider : IDateTimeProvider
    {
        public DateTimeKind Kind => DateTimeKind.Local;
        public DateTime Now => DateTime.Now;
    }
}