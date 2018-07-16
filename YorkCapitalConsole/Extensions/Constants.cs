using System;

namespace Extensions
{
    public enum DataTypes
    {
        NUMERIC,
        DATE,
        STRING
    };

    internal static class AppConstant
    {
        internal static DateTime SQLDateTimeMin = DateTime.MinValue;
        internal static DateTime SQLDateTimeMax = DateTime.MaxValue;

        internal const string DateFormat = "MM/dd/yyyy";
    }
}
