using Haondt.Identity.StorageKey;
using KristofferStrube.ActivityStreams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.Grains.Extensions
{
    public static class ActivityPubObjectExtensions
    {
        public static StorageKey<T> GetStorageKey<T>(this T objectOrLink) where T : ObjectOrLink
        {
            if (objectOrLink.Id == null)
                throw new InvalidOperationException($"objectOrLink has no id");
            return StorageKey<T>.Create(objectOrLink.Id);
        }
    }
}
