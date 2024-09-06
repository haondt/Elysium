using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Exceptions
{
    public class ActivityPubException : Exception
    {
        public ActivityPubException() : base() { }
        public ActivityPubException(string message) : base(message) { }
    }
}
