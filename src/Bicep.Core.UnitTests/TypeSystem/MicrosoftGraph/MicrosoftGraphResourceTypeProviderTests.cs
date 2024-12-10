// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Providers.MicrosoftGraph;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem.MicrosoftGraph
{
    [TestClass]
    public class MicrosoftGraphResourceTypeProviderTests
    {
        private static ServiceBuilder Services => new();

        [TestMethod]
        public void MicrosoftGraphResourceTypeProvider_can_list_all_types_without_throwing()
        {
            var availableTypes = MicrosoftGraphNamespaceType.Create(MicrosoftGraphNamespaceType.Settings.BicepExtensionName).ResourceTypeProvider.GetAvailableTypes();

            // sanity check - we know there should be a lot of types available
            var minExpectedTypes = 5;
            availableTypes.Should().HaveCountGreaterThanOrEqualTo(minExpectedTypes);

            // verify there aren't any duplicates
            availableTypes.Select(x => x.FormatName().ToLowerInvariant()).Should().OnlyHaveUniqueItems();
        }

        [TestMethod]
        public void MsGraphResourceTypeProvider_should_warn_for_property_mismatch()
        {
            Compilation createCompilation(string program) => Services
                .WithMsGraphResourceTypeLoader(new MicrosoftGraphResourceTypeLoader())
                .BuildCompilation(program);

            var compilation = createCompilation(@"
resource app 'Microsoft.Graph/applications@beta' = {
  uniqueName: 'test'
  displayName: 'test'
  extraProp: 'extra'
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, "The property \"extraProp\" is not allowed on objects of type \"Microsoft.Graph/applications\". Permissible properties include \"api\", \"appRoles\", \"authenticationBehaviors\", \"defaultRedirectUri\", \"dependsOn\", \"description\", \"disabledByMicrosoftStatus\", \"groupMembershipClaims\", \"identifierUris\", \"info\", \"isDeviceOnlyAuthSupported\", \"isFallbackPublicClient\", \"keyCredentials\", \"logo\", \"notes\", \"optionalClaims\", \"parentalControlSettings\", \"passwordCredentials\", \"publicClient\", \"requestSignatureVerification\", \"requiredResourceAccess\", \"samlMetadataUrl\", \"serviceManagementReference\", \"servicePrincipalLockConfiguration\", \"signInAudience\", \"spa\", \"tags\", \"tokenEncryptionKeyId\", \"verifiedPublisher\", \"web\", \"windows\". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues.")
            });

            compilation = createCompilation(@"
resource app 'Microsoft.Graph/applications@beta' = {
  uniqueName: 'test'
  displayName: 'test'
  spa: {
    extraNestedProp: 'extra'
  }
}
");
            compilation.Should().HaveDiagnostics(new[] {
                ("BCP037", DiagnosticLevel.Warning, "The property \"extraNestedProp\" is not allowed on objects of type \"MicrosoftGraphSpaApplication\". Permissible properties include \"redirectUris\". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues.")
            });
        }
    }
}
