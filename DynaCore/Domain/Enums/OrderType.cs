﻿using System.Runtime.Serialization;

namespace DynaCore.Domain.Enums
{
    public enum OrderType
    {
        [EnumMember(Value = "asc")]
        Asc,

        [EnumMember(Value = "desc")]
        Desc
    }
}