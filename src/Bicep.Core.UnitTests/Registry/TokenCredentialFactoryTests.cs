// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Core;
using Azure.Identity;
using Bicep.Core.Configuration;
using Bicep.Core.Registry.Auth;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;

namespace Bicep.Core.UnitTests.Registry
{
    [TestClass]
    public class TokenCredentialFactoryTests
    {
        private static readonly Uri exampleAuthorityUri = new("https://bicep.test.invalid");

        [DataRow(CredentialType.Environment, typeof(EnvironmentCredential))]
        [DataRow(CredentialType.ManagedIdentity, typeof(ManagedIdentityCredential))]
        [DataRow(CredentialType.VisualStudio, typeof(VisualStudioCredential))]
        [DataRow(CredentialType.VisualStudioCode, typeof(VisualStudioCodeCredential))]
        [DataRow(CredentialType.AzureCLI, typeof(AzureCliCredential))]
        [DataRow(CredentialType.AzurePowerShell, typeof(AzurePowerShellCredential))]
        [DataTestMethod]
        public void ShouldCreateExpectedSingleCredential(CredentialType credentialType, Type expectedCredentialType)
        {
            var f = new TokenCredentialFactory();
            f.CreateSingle(credentialType, exampleAuthorityUri).Should().BeOfType(expectedCredentialType);
        }

        public void EmptyListOfCredentialTypesShouldThrow()
        {
            var f = new TokenCredentialFactory();
            FluentActions.Invoking(() => f.CreateChain(Enumerable.Empty<CredentialType>(), exampleAuthorityUri)).Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void ShouldCreateExpectedSingleItemChain()
        {
            var f = new TokenCredentialFactory();
            var credential = f.CreateChain(new[] { CredentialType.VisualStudioCode }, exampleAuthorityUri);
            AssertCredentialTypes(credential, typeof(VisualStudioCodeCredential));
        }

        [TestMethod]
        public void ShouldCreateExpectedMultiItemChain()
        {
            var f = new TokenCredentialFactory();
            var credential = f.CreateChain(new[] { CredentialType.AzureCLI, CredentialType.ManagedIdentity, CredentialType.VisualStudio }, exampleAuthorityUri);
            AssertCredentialTypes(credential, typeof(AzureCliCredential), typeof(ManagedIdentityCredential), typeof(VisualStudioCredential));
        }

        private void AssertCredentialTypes(TokenCredential credential, params Type[] expectedTypes)
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
