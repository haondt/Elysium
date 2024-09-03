using Haondt.Identity.StorageKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Models
{
    public interface IStorageKeyIdModel<T>
    {
        public StorageKey<T> Id { get; set; }
    }
}
