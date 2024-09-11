using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Exceptions
{
    public class ProbabilityException : Exception
    {
        public ProbabilityException()
        {
        }

        public ProbabilityException(string? message) : base(message)
        {
        }

        public ProbabilityException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ProbabilityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
