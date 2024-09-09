﻿using Elysium.Core.Models;
using Elysium.GrainInterfaces;
using Haondt.Identity.StorageKey;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elysium.GrainInterfaces.Services
{
    public class StorageKeyGrainFactory<T>(IGrainFactory grainFactory) : IGrainFactory<StorageKey<T>>
    {
        public TGrain GetGrain<TGrain>(StorageKey<T> identity) where TGrain : IGrain<StorageKey<T>>
        {
            return grainFactory.GetGrain<TGrain>(StorageKeyConvert.Serialize(identity));
        }

        public StorageKey<T> GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<StorageKey<T>>
        {
            return StorageKeyConvert.Deserialize<T>(grain.GetPrimaryKeyString());
        }
    }

    //public class StorageKeyGrainFactory(IGrainFactory grainFactory) : IGrainFactory<StorageKey>
    //{
    //    public TGrain GetGrain<TGrain>(StorageKey identity) where TGrain : IGrain<StorageKey>
    //    {

    //        return grainFactory.GetGrain<TGrain>(StorageKeyConvert.Serialize(identity));
    //    }

    //    public StorageKey GetIdentity<TGrain>(TGrain grain) where TGrain : IGrain<StorageKey>
    //    {
    //        return StorageKeyConvert.Deserialize(grain.GetPrimaryKeyString());
    //    }
    //}
}
