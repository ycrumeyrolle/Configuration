// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration.Test;
using StackExchange.Redis;

namespace Microsoft.Extensions.Configuration.Redis.Test
{
    public class RedisConfigurationTest
    {
        [Fact]
        public void LoadsDataFromRedis()
        {
            var connection = new Mock<IConnectionMultiplexer>(MockBehavior.Strict);
            var database = new Mock<IDatabase>(MockBehavior.Strict);

            connection.Setup(c => c.GetDatabase(-1, null)).Returns(database.Object);
            database
                .Setup(c => c.HashGetAll(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns(new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {"Key1", "Value1"},
                    {"Key2", "Value2"}
                }.ToRedisHashset());

            // Act
            var provider = new RedisConfigurationProvider(() => connection.Object, "key", false);
            provider.Load();

            // Assert
            connection.VerifyAll();
            database.VerifyAll();

            Assert.Equal("Value1", provider.Get("Key1"));
            Assert.Equal("Value2", provider.Get("Key2"));
        }

        [Fact]
        public void SupportsReload()
        {
            var connection = new Mock<IConnectionMultiplexer>(MockBehavior.Strict);
            var database = new Mock<IDatabase>(MockBehavior.Strict);

            connection.Setup(c => c.GetDatabase(-1, null)).Returns(database.Object);
            var values = new Dictionary<string, string>()
            {
                {"Key1", "Value1"}
            };

            database
                .Setup(c => c.HashGetAll(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns(() => values.ToRedisHashset());

            // Act 
            var provider = new RedisConfigurationProvider(() => connection.Object, "key", false);
            provider.Load();

            // Assert
            connection.VerifyAll();
            database.VerifyAll();
            Assert.Equal("Value1", provider.Get("Key1"));

            values = new Dictionary<string, string>()
            {
                {"Key1", "Value2"}
            };
            provider.Load();
            Assert.Equal("Value2", provider.Get("Key1"));
        }

        [Fact]
        public void SupportsConfigurationChange()
        {
            var connection = new Mock<IConnectionMultiplexer>(MockBehavior.Strict);
            var database = new Mock<IDatabase>(MockBehavior.Strict);
            var subscriber = new Mock<ISubscriber>(MockBehavior.Strict);

            connection.Setup(c => c.GetDatabase(-1, null)).Returns(database.Object);
            connection.Setup(c => c.GetSubscriber(null)).Returns(subscriber.Object);

            var values = new Dictionary<string, string>()
            {
                {"Key1", "Value1"}
            };

            database
                .SetupGet(d => d.Database)
                .Returns(1);
            database
                .Setup(d => d.HashGetAll(It.IsAny<RedisKey>(), CommandFlags.None))
                .Returns(() => values.ToRedisHashset());

            Action<RedisChannel, RedisValue> reload = null;
            subscriber
                .Setup(s => s.Subscribe(It.IsAny<RedisChannel>(), It.IsAny<Action<RedisChannel, RedisValue>>(), It.IsAny<CommandFlags>()))
                .Callback<RedisChannel, Action<RedisChannel, RedisValue>, CommandFlags>((c, a, f) => reload = a);

            RedisConfigurationProvider provider = new RedisConfigurationProvider(() => connection.Object, "key", reloadOnChange: true);
            // Act 
            provider.Load();

            // Assert
            connection.VerifyAll();
            database.VerifyAll();
            subscriber.VerifyAll();

            Assert.Equal("Value1", provider.Get("Key1"));

            values = new Dictionary<string, string>()
            {
                {"Key1", "Value2"}
            };

            reload(default(RedisChannel), default(RedisValue));

            Assert.Equal("Value2", provider.Get("Key1"));
        }
    }
}
