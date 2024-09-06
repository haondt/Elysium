using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public enum ActivityType
    {
        Unknown = 0,
        Create,
        Update,
        Delete,
        Follow,
        Add,
        Remove,
        Like,
        Block,
        Undo
    }
}
