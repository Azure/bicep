// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration
{
    [TestClass]
    public class ImplicitExtensionsTests
    {
        [DataTestMethod]
        [DataRow(new string[] { "extension1", "extension2" }, 2)]
        [DataRow(new string[] { "extension1" }, 1)]
        [DataRow(new string[] { }, 0)]
        public void ImplicitExtensionsConfiguration_deserialization_happy_path_succeeds(string[] extensions, int expectedCount)
        {
            var json = $$"""
            {
                "implicitExtensions": [{{string.Join(", ", extensions.Select(p => $"\"{p}\""))}}]
            }
            """;
            var element = JsonElementFactory.CreateElement(json);
            var configuration = ImplicitExtensionsConfiguration.Bind(element.GetProperty(RootConfiguration.ImplicitExtensionsKey));

            Assert.AreEqual(expectedCount, configuration.GetImplicitExtensionNames().Count());
        }
    }
}
