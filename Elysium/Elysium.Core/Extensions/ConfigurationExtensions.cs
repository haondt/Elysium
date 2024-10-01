using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetRequiredSection<T>(this IConfiguration configuration)
        {
            return configuration.GetRequiredSection(typeof(T).Name).Get<T>() ?? throw new ArgumentNullException(typeof(T).Name);
        }
    }
}
