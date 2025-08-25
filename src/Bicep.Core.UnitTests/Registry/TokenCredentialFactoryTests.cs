// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Reflection;
using Azure.Core;
using Azure.Identity;
using Bicep.Core.Configuration;
using Bicep.Core.AzureApi;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class TokenCredentialFactoryTests
    {
        private static readonly Uri exampleAuthorityUri = new("https://bicep.test.invalid");

        [DataTestMethod]
        [DataRow(CredentialType.Environment, null, typeof(EnvironmentCredential))]
        [DataRow(CredentialType.ManagedIdentity, null, typeof(ManagedIdentityCredential))]
        [DataRow(CredentialType.VisualStudio, null, typeof(VisualStudioCredential))]
        [DataRow(CredentialType.VisualStudioCode, null, typeof(VisualStudioCodeCredential))]
        [DataRow(CredentialType.AzureCLI, null, typeof(AzureCliCredential))]
        [DataRow(CredentialType.AzurePowerShell, null, typeof(AzurePowerShellCredential))]
        public void ShouldCreateExpectedSingleCredential(CredentialType credentialType, CredentialOptions? credentialOptions, Type expectedCredentialType)
        {
            var f = new TokenCredentialFactory();
            f.CreateSingle(credentialType, credentialOptions, exampleAuthorityUri).Should().BeOfType(expectedCredentialType);
        }

        [DataTestMethod]
        [DynamicData(nameof(CreateManagedIdentityOptionsData), DynamicDataSourceType.Method)]
        public void ShouldCreateExpectedSingleManagedIdentityCredential(CredentialOptions? credentialOptions)
        {
            var f = new TokenCredentialFactory();
            f.CreateSingle(CredentialType.ManagedIdentity, credentialOptions, exampleAuthorityUri).Should().BeOfType(typeof(ManagedIdentityCredential));
        }

        [TestMethod]
        public void EmptyListOfCredentialTypesShouldThrow()
        {
            var f = new TokenCredentialFactory();
            FluentActions.Invoking(() => f.CreateChain([], null, exampleAuthorityUri)).Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void ShouldCreateExpectedSingleItemChain()
        {
            var f = new TokenCredentialFactory();
            var credential = f.CreateChain(new[] { CredentialType.VisualStudioCode }, null, exampleAuthorityUri);
            AssertCredentialTypes(credential, typeof(VisualStudioCodeCredential));
        }

        [TestMethod]
        public void ShouldCreateExpectedMultiItemChain()
        {
            var f = new TokenCredentialFactory();
            var credential = f.CreateChain(new[] { CredentialType.AzureCLI, CredentialType.ManagedIdentity, CredentialType.VisualStudio }, null, exampleAuthorityUri);
            AssertCredentialTypes(credential, typeof(AzureCliCredential), typeof(ManagedIdentityCredential), typeof(VisualStudioCredential));
        }

        private static IEnumerable<object[]> CreateManagedIdentityOptionsData()
        {
            yield return CreateTestsCase(null);
            yield return CreateTestsCase(new ManagedIdentity(ManagedIdentityType.SystemAssigned, null, null));
            yield return CreateTestsCase(new ManagedIdentity(ManagedIdentityType.SystemAssigned, Guid.Empty.ToString(), null));
            yield return CreateTestsCase(new ManagedIdentity(ManagedIdentityType.SystemAssigned, null, $"/subscriptions/{Guid.Empty}/providers/resourceGroups/myRG/providers/Microsoft.Storage/storageAccounts/mySA"));

            static object[] CreateTestsCase(ManagedIdentity? managedIdentity) => [new CredentialOptions(managedIdentity)];
        }

        private static void AssertCredentialTypes(TokenCredential credential, params Type[] expectedTypes)
        {
            credential.Should().BeOfType<ChainedTokenCredential>();

            // no other way to find out how it was created
            // it's fragile but should be easy to fix after package upgrades
            var sourcesField = typeof(ChainedTokenCredential).GetField("_sources", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField)!;
            sourcesField.Should().NotBeNull();

            var rawValue = sourcesField.GetValue(credential)!;
            rawValue.Should().BeOfType<TokenCredential[]>();

            var actualTypes = (TokenCredential[])rawValue;
            actualTypes.Select(t => t.GetType()).Should().Equal(expectedTypes);
        }
    }
}
