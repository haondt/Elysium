using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.ActivityPub.Extensions
{
    public static class DateTimeExtensions
    {
        public static string AsXsdString(this DateTime dateTime)
        {
            return dateTime.ToString("o");
        }
    }
}
