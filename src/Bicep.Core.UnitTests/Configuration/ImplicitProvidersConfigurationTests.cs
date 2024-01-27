// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration
{
    [TestClass]
    public class ImplicitProvidersTests
    {
        [DataTestMethod]
        [DataRow(new string[] { "provider1", "provider2" }, 2)]
        [DataRow(new string[] { "provider1" }, 1)]
        [DataRow(new string[] { }, 0)]
        public void ImplicitProvidersConfiguration_deserialization_happy_path_succeeds(string[] providers, int expectedCount)
        {
            var json = $$"""
            {
                "implicitProviders": [{{string.Join(", ", providers.Select(p => $"\"{p}\""))}}]
            }
            """;
            var element = JsonElementFactory.CreateElement(json);
            var configuration = ImplicitProvidersConfiguration.Bind(element.GetProperty(RootConfiguration.ImplicitProvidersConfigurationKey));

            Assert.AreEqual(expectedCount, configuration.GetImplicitProviderNames().Count());
        }
    }
}
