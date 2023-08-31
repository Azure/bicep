// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Bicep.Core.UnitTests.TypeSystem.MicrosoftGraph
{
    [TestClass]
    public class MicrosoftGraphResourceTypeProviderTests
    {
        private static string MicrosoftGraphBuildInName = "microsoftGraph";

        private static NamespaceType GetMicrosoftGraphNamespaceType()
        {
            return BicepTestConstants.NamespaceProvider.TryGetNamespace(MicrosoftGraphBuildInName, MicrosoftGraphBuildInName, ResourceScope.ResourceGroup, BicepTestConstants.Features, BicepSourceFileKind.BicepFile, null)!;
        }

        [TestMethod]
        public void MicrosoftGraphResourceTypeProvider_can_list_all_types_without_throwing()
        {
            var resourceTypeProvider = GetMicrosoftGraphNamespaceType().ResourceTypeProvider;
            var availableTypes = resourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var minExpectedTypes = 5;
            availableTypes.Should().HaveCountGreaterThanOrEqualTo(minExpectedTypes);

            // verify there aren't any duplicates
            availableTypes.Select(x => x.FormatName().ToLowerInvariant()).Should().OnlyHaveUniqueItems();
        }
    }
}
