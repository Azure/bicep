// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Modules;
using Bicep.Core.Registry;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Registry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class PublicRegistryModuleMetadataProviderTests
    {
        [TestMethod]
        public void GetExponentialDelay_ZeroCount_ShouldGiveInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            PublicRegistryModuleMetadataProvider provider = new();
            var delay = provider.GetExponentialDelay(initial, 0, max);

            delay.Should().Be(initial);
        }

        [TestMethod]
        public void GetExponentialDelay_1Count_ShouldGiveDoubleInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            PublicRegistryModuleMetadataProvider provider = new();
            var delay = provider.GetExponentialDelay(initial, 1, max);

            delay.Should().Be(initial*2);
        }

        [TestMethod]
        public void GetExponentialDelay_2Count_ShouldGiveQuadrupleInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            PublicRegistryModuleMetadataProvider provider = new();
            var delay = provider.GetExponentialDelay(initial, 2, max);

            delay.Should().Be(initial * 4);
        }

        [TestMethod]
        public void GetExponentialDelay_AboveMaxCount_ShouldGiveMaxDelay()
        {
            TimeSpan initial = TimeSpan.FromSeconds(1);
            TimeSpan max = TimeSpan.FromDays(365);
            PublicRegistryModuleMetadataProvider provider = new();

            TimeSpan exponentiallyGrowingDelay = initial;
            int count = 0;
            while (exponentiallyGrowingDelay < max * 1000)
            {
                var delay = provider.GetExponentialDelay(initial, count, max);

                if (exponentiallyGrowingDelay < max)
                {
                    delay.Should().Be(exponentiallyGrowingDelay);
                }
                else
                {
                    delay.Should().Be(max);
                }

                delay.Should().BeLessThanOrEqualTo(max);

                ++count;
                exponentiallyGrowingDelay *= 2;
            }
        }
    }
}
