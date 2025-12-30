using System;

namespace BlueToque.Utility
{
    public static class TimeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromUnixEpochTime(string seconds) =>
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(Convert.ToInt64(seconds));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static DateTime FromUnixEpochTime(int? seconds) => seconds == null
                ? DateTime.UtcNow
                : new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(seconds.Value);
    }
}
