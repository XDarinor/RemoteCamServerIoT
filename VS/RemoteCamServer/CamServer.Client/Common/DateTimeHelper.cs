using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMDev.CamServer.Client.Common
{
    public static class DateTimeHelper
    {
        #region Methods

        public static DateTime FromUnixTime(double value, EpochTimeMeasureUnits timeMeasureUnit = EpochTimeMeasureUnits.Seconds)
        {
            DateTime baseUnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            switch (timeMeasureUnit)
            {
                case EpochTimeMeasureUnits.Seconds:
                    baseUnixTime = baseUnixTime.AddSeconds(value);
                    break;
                case EpochTimeMeasureUnits.Milliseconds:
                    baseUnixTime = baseUnixTime.AddMilliseconds(value);
                    break;
                default:
                    throw new FormatException("timeMeasureUnit");
            }

            return baseUnixTime;
        }

        public static double ToUnixTime(this DateTime source)
        {
            TimeSpan sourceTimeSpan = source.ToUniversalTime() - new DateTime(1970, 1, 1);
            double secondsSinceEpoch = sourceTimeSpan.TotalSeconds;

            return secondsSinceEpoch;
        }

        #endregion
    }
}
