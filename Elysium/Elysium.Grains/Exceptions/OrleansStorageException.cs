using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Exceptions
{
    public class OrleansStorageException : Exception
    {
        public OrleansStorageException()
        {
        }

        public OrleansStorageException(string? message) : base(message)
        {
        }

        public OrleansStorageException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

    }
}
