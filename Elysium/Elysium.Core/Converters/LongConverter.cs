﻿namespace Elysium.Core.Converters
{
    public static class LongConverter
    {
        public static string EncodeLong(long i)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(i));
        }

        public static long DecodeLong(string s)
        {
            return BitConverter.ToInt64(Convert.FromBase64String(s));
        }
    }
}
