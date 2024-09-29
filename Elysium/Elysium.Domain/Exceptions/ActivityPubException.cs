﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Domain.Exceptions
{
    [GenerateSerializer]
    public class ActivityPubException : Exception
    {
        public ActivityPubException() : base() { }
        public ActivityPubException(string message) : base(message) { }

        public ActivityPubException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
