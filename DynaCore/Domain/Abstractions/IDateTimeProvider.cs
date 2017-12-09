using System;

namespace DynaCore.Domain.Abstractions
{
    public interface IDateTimeProvider
    {
        DateTimeKind Kind { get; }

        DateTime Now { get; }
    }
}