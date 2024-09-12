﻿using Elysium.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class NoSignatureAuthor : IHttpMessageAuthor
    {
        public static NoSignatureAuthor Instance = new();
        public Task<LocalIri> GetIriAsync()
        {
            throw new NotSupportedException();
        }

        public Task<string> GetKeyIdAsync()
        {
            throw new NotSupportedException();
        }

        public Task<bool> IsInASigningMoodAsync() => Task.FromResult(false);

        public Task<string> SignAsync(string stringToSign)
        {
            throw new NotSupportedException();
        }
    }
}
