using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Core.Exceptions
{
    public class JsonNavigationException : Exception
    {
        public JsonNavigationException() : base () { }
        public JsonNavigationException(string message) : base(message) { }
    }
}
