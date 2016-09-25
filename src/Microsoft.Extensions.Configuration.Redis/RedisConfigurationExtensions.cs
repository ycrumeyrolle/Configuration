// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.Configuration.Redis;
using StackExchange.Redis;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Extension methods for adding <see cref="RedisConfigurationProvider"/>.
    /// </summary>
    public static class RedisConfigurationExtensions
    {
        /// <summary>
        /// Adds the Redis configuration provider to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configuration">The configuration used to connect to Redis.</param>
        /// <param name="key">The Redis key used for configuration.</param>
        /// <param name="reloadOnChange">Determines whether the source will be loaded if the underlying configuration changes. 
        /// Requires the Redis Keyspace Notifications feature enabled on the Redis server. 
        /// <code>CONFIG SET notify-keyspace-events Kh</code>. 
        /// See http://redis.io/topics/notifications for details. 
        /// </param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddRedis(this IConfigurationBuilder builder, string configuration, string key, bool reloadOnChange = false)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(configuration))
            {
                throw new ArgumentException(Resources.Error_InvalidConfiguration, nameof(configuration));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Resources.Error_InvalidKey, nameof(key));
            }

            return builder.AddRedis(() => ConnectionMultiplexer.Connect(configuration), key, reloadOnChange);
        }

        private static IConfigurationBuilder AddRedis(this IConfigurationBuilder builder, Func<IConnectionMultiplexer> connectionFactory, string key, bool reloadOnChange)
        {
            var source = new RedisConfigurationSource()
            {
                Key = key,
                ReloadOnChange = reloadOnChange,
                ConnectionFactory = connectionFactory
            };

            builder.Add(source);
            return builder;
        }
    }
}