// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using StackExchange.Redis;

namespace Microsoft.Extensions.Configuration.Redis.Test
{

    public static class DictionaryExtensions
    {
        public static HashEntry[] ToRedisHashset(this Dictionary<string, string> data)
        {
            return data
                .Select(kvp => new HashEntry(kvp.Key, kvp.Value))
                .ToArray();
        }
    }
}