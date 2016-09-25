// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using StackExchange.Redis;

namespace Microsoft.Extensions.Configuration.Redis
{
    /// <summary>
    /// Represents a Redis database as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class RedisConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// The redis instance.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Determines whether the source will be loaded if the underlying configuration changes.
        /// </summary>
        public bool ReloadOnChange { get; set; }

        /// <summary>
        /// The factory of <see cref="IConnectionMultiplexer"/> for database access.
        /// </summary>
        public Func<IConnectionMultiplexer> ConnectionFactory { get; set; }

        /// <summary>
        /// Builds the <see cref="RedisConfigurationSource"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="RedisConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new RedisConfigurationProvider(ConnectionFactory, Key, ReloadOnChange);
        }
    }
}