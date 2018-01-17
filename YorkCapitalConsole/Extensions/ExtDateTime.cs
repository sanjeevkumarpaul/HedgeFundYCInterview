using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static partial class ExtDateTime
    {
        private static bool IsLocal(string zone)
        {
            return TimeZone.CurrentTimeZone.StandardName.EqualsIgnoreCase(zone);
        }


        public static DateTime ToLocal(this DateTime time)
        {
            var zone = TimeZone.CurrentTimeZone.StandardName;

            return zone.Empty() || IsLocal(zone) ? time : TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(time, TimeZoneInfo.Local),
                                                                                          TimeZoneInfo.FindSystemTimeZoneById(zone));
        }
    }
}
