using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Cryptography.Services
{
    [GenerateSerializer]
    public class EncryptedDataParameters
    {
        [Id(0)]
        public required string IV { get; set; }
    }
}
