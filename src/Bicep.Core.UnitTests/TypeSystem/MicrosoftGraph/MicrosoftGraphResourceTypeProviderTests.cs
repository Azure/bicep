// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem.MicrosoftGraph
{
    [TestClass]
    public class MicrosoftGraphResourceTypeProviderTests
    {

        [TestMethod]
        public void MicrosoftGraphResourceTypeProvider_can_list_all_types_without_throwing()
        {
            var availableTypes = MicrosoftGraphNamespaceType.Create(MicrosoftGraphNamespaceType.Settings.BicepProviderName).ResourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var minExpectedTypes = 5;
            availableTypes.Should().HaveCountGreaterThanOrEqualTo(minExpectedTypes);

            // verify there aren't any duplicates
            availableTypes.Select(x => x.FormatName().ToLowerInvariant()).Should().OnlyHaveUniqueItems();
        }
    }
}
