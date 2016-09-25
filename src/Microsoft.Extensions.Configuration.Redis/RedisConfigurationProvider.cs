// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using StackExchange.Redis;

namespace Microsoft.Extensions.Configuration.Redis
{
    /// <summary>
    /// A Redis based <see cref="ConfigurationProvider"/>.
    /// </summary>
    public class RedisConfigurationProvider : ConfigurationProvider
    {
        private readonly Func<IConnectionMultiplexer> _connectionFactory;
        private readonly string _key;

        private IConnectionMultiplexer _connection;
        private IDatabase _database;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public RedisConfigurationProvider(Func<IConnectionMultiplexer> connectionFactory, string key, bool reloadOnChange)
        {
            if (connectionFactory == null)
            {
                throw new ArgumentNullException(nameof(connectionFactory));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _connectionFactory = connectionFactory;
            _key = key;

            if (reloadOnChange)
            {
                Connect();

                var subscriber = _connection.GetSubscriber();
                var topic = $"__keyspace@{_database.Database}__:{_key}";
                subscriber.Subscribe(topic, (channel, value) =>
                {
                    Reload();
                });
            }
        }

        /// <summary>
        /// Loads the configuration data from the redis store.
        /// </summary>
        public override void Load()
        {
            Connect();

            var hash = _database.HashGetAll(_key);
            var data = new Dictionary<string, string>(hash.Length, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < hash.Length; i++)
            {
                data.Add(hash[i].Name, hash[i].Value);
            }

            Data = data;
        }

        private void Reload()
        {
            Load();
            OnReload();
        }

        private void Connect()
        {
            if (_connection == null)
            {
                _connection = _connectionFactory();
                _database = _connection.GetDatabase();
            }
        }
    }
}
