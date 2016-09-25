// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace RedisSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("settings.json");
            var config = builder.Build();

            builder.AddRedis(config["ConnectionString"], config["ConfigurationKey"], reloadOnChange: true);
            config = builder.Build();
            ChangeToken.OnChange(config.GetReloadToken, ConfigurationChanged, config);

            Console.WriteLine("Initial configuration :");
            foreach (var item in config.AsEnumerable())
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }

            Console.ReadKey();
        }

        private static void ConfigurationChanged(IConfigurationRoot configuration)
        {
            Console.WriteLine();
            Console.WriteLine("Configuration has changed :");
            foreach (var item in configuration.AsEnumerable())
            {
                Console.WriteLine($"{item.Key} : {item.Value}");
            }
        }
    }
}
