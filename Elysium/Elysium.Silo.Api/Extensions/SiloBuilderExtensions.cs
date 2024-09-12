using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Silo.Extensions
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder AddElysiumStorageGrainStorage(this ISiloBuilder builder, string providerName)
        {
            builder.Services.AddElysiumStorageGrainStorage(providerName);
            return builder;
        }
    }
}
