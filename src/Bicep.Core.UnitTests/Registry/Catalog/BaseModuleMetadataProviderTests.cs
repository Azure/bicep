// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bicep.Core.Registry.Catalog.Implementation;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace Bicep.Core.UnitTests.Registry.Catalog
{
    [TestClass]
    public class BaseModuleMetadataProviderTests
    {
        [TestMethod]
        public void GetExponentialDelay_ZeroCount_ShouldGiveInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            var delay = BaseModuleMetadataProvider.GetExponentialDelay(initial, 0, max);

            delay.Should().Be(initial);
        }

        [TestMethod]
        public void GetExponentialDelay_1Count_ShouldGiveDoubleInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            var delay = BaseModuleMetadataProvider.GetExponentialDelay(initial, 1, max);

            delay.Should().Be(initial * 2);
        }

        [TestMethod]
        public void GetExponentialDelay_2Count_ShouldGiveQuadrupleInitialDelay()
        {
            TimeSpan initial = TimeSpan.FromDays(2.5);
            TimeSpan max = TimeSpan.FromDays(10);
            var delay = BaseModuleMetadataProvider.GetExponentialDelay(initial, 2, max);

            delay.Should().Be(initial * 4);
        }

        [TestMethod]
        public void GetExponentialDelay_AboveMaxCount_ShouldGiveMaxDelay()
        {
            TimeSpan initial = TimeSpan.FromSeconds(1);
            TimeSpan max = TimeSpan.FromDays(365);

            TimeSpan exponentiallyGrowingDelay = initial;
            int count = 0;
            while (exponentiallyGrowingDelay < max * 1000)
            {
                var delay = BaseModuleMetadataProvider.GetExponentialDelay(initial, count, max);

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
