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
        [TestMethod]
        public void ImplicitProvidersConfiguration_Bind_Null()
        {
            var element = JsonElementFactory.CreateElement("""
            {
                "implicitProviders": []
            }
            """);
            var configuration = ImplicitProvidersConfiguration.Bind(element.GetProperty("implicitProviders"));

            Assert.AreEqual(0, configuration.GetImplicitProviderNames().Count());
        }
    }

}
