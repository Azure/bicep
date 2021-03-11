// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    [TestClass]
    public class ParamKeyVaultSecretReferenceTests
    {

        [TestMethod]
        public void ValidKeyVaultSecretReference()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}


module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    mySecret: kv.getSecret('mySecret')
  }
}
"),
                ("secret.bicep", @"
@secure()
param mySecret string

output exposed string = mySecret
"));

            diags.Should().BeEmpty();
            template!.Should().NotBeNull();
            var parameterToken = template!.SelectToken("$.resources[?(@.name == 'secret')].properties.parameters.mySecret")!;
            using (new AssertionScope())
            {
                parameterToken.SelectToken("$.value")!.Should().BeNull();
                parameterToken.SelectToken("$.reference.keyVault.id")!.Should().DeepEqual("[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]");
                parameterToken.SelectToken("$.reference.secretName")!.Should().DeepEqual("mySecret");
                parameterToken.SelectToken("$.reference.secretVersion")!.Should().BeNull();
            }
        }

        [TestMethod]
        public void ValidKeyVaultSecretReferenceWithSecretVersion()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    mySecret: kv.getSecret('mySecret','secretversionguid')
  }
}
"),
                ("secret.bicep", @"
@secure()
param mySecret string = 'defaultSecret'

output exposed string = mySecret
"));

            diags.Should().BeEmpty();
            template!.Should().NotBeNull();
            var parameterToken = template!.SelectToken("$.resources[?(@.name == 'secret')].properties.parameters.mySecret")!;
            using (new AssertionScope())
            {
                parameterToken.SelectToken("$.value")!.Should().BeNull();
                parameterToken.SelectToken("$.reference.keyVault.id")!.Should().DeepEqual("[resourceId('Microsoft.KeyVault/vaults', 'testkeyvault')]");
                parameterToken.SelectToken("$.reference.secretName")!.Should().DeepEqual("mySecret");
                parameterToken.SelectToken("$.reference.secretVersion")!.Should().DeepEqual("secretversionguid");
            }
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInOutput()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

output exposed string = kv.getSecret('mySecret','secretversionguid')
"));

            diags.Should().NotBeEmpty();
            template!.Should().BeNull();
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInVariable()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

var secret = kv.getSecret('mySecret','secretversionguid')
"));

            diags.Should().NotBeEmpty();
            template!.Should().BeNull();
        }


        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInNonSecretParam()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    notSecret: kv.getSecret('mySecret','secretversionguid')
  }
}
"),
                ("secret.bicep", @"
param notSecret string
"));

            diags.Should().NotBeEmpty();
            template!.Should().BeNull();
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInSecureParamInterpolation()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    secret: 'aSecret${kv.getSecret('mySecret','secretversionguid')}'
  }
}
"),
                ("secret.bicep", @"
@secure
param secret string
"));

            diags.Should().NotBeEmpty();
            template!.Should().BeNull();
        }
    }
}
