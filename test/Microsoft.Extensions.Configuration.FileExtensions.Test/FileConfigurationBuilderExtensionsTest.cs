// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace Microsoft.Extensions.Configuration.Json
{
    public class FileConfigurationBuilderExtensionsTest
    {
        [Fact]
        public void SetFileProvider_ThrowsIfBasePathIsNull()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => configurationBuilder.SetBasePath(basePath: null));
            Assert.Equal("basePath", ex.ParamName);
        }

        [Fact]
        public void SetFileProvider_CheckPropertiesValueOnBuilder()
        {
            var expectedBasePath = Directory.GetCurrentDirectory();
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.SetBasePath(expectedBasePath);
            var physicalProvider = configurationBuilder.GetFileProvider() as PhysicalFileProvider;
            Assert.NotNull(physicalProvider);
            Assert.Equal(EnsureTrailingSlash(expectedBasePath), physicalProvider.Root);
        }

        [Fact]
        public void GetFileProvider_ReturnPhysicalProviderWithBaseDirectoryIfNotSet()
        {
            // Arrange
            var configurationBuilder = new ConfigurationBuilder();

            // Act
            var physicalProvider = configurationBuilder.GetFileProvider() as PhysicalFileProvider;

            string expectedPath;

#if NETCOREAPP1_0
            expectedPath = AppContext.BaseDirectory;
#else
            expectedPath = Path.GetFullPath(AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") as string ?? 
                AppDomain.CurrentDomain.BaseDirectory);
#endif

            Assert.NotNull(physicalProvider);
            Assert.Equal(EnsureTrailingSlash(expectedPath), physicalProvider.Root);
        }

        private static string EnsureTrailingSlash(string path)
        {
            if (!string.IsNullOrEmpty(path) &&
                path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }
    }
}
