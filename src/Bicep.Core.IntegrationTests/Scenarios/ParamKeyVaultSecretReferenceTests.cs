// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions.Execution;
using Bicep.Core.Diagnostics;
using System.Linq;

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
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

output exposed string = kv.getSecret('mySecret','secretversionguid')
"));

            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInVariable()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

var secret = kv.getSecret('mySecret','secretversionguid')
"));

            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }


        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInNonSecretParam()
        {
            var result = CompilationHelper.Compile(
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

            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInSecureParamInterpolation()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    testParam: '${kv.getSecret('mySecret','secretversionguid')}'
  }
}
"),
                ("secret.bicep", @"
@secure()
param testParam string
"));

            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInObjectParam()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    testParam: kv.getSecret('mySecret','secretversionguid')
  }
}
"),
                ("secret.bicep", @"
param testParam object
"));


            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceUsageInArrayParam()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}

module secret 'secret.bicep' = {
  name: 'secret'
  params: {
    testParam: [
      kv.getSecret('mySecret','secretversionguid')
    ]
  }
}
"),
                ("secret.bicep", @"
param testParam array
"));

            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }


        [TestMethod]
        public void ValidKeyVaultSecretReferenceInLoopedModule()
        {
            var (template, diags, _) = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}
var secrets = [
{
  name: 'secret01'
  version: 'versionA'
}
{
  name: 'secret02'
  version: 'versionB'
}
]
module secret 'secret.bicep' = [for (secret, i) in secrets : {
  name: 'secret'
  params: {
    mySecret: kv.getSecret('super-${secret.name}', secret.version)
  }
}]
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
                parameterToken.SelectToken("$.reference.secretName")!.Should().DeepEqual("[format('super-{0}', variables('secrets')[copyIndex()].name)]");
                parameterToken.SelectToken("$.reference.secretVersion")!.Should().DeepEqual("[variables('secrets')[copyIndex()].version]");
            }
        }

        [TestMethod]
        public void InvalidKeyVaultSecretReferenceInLoopedModule()
        {
            var result = CompilationHelper.Compile(
                ("main.bicep", @"
resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: 'testkeyvault'
}
var secrets = [
{
  name: 'secret01'
  version: 'versionA'
}
{
  name: 'secret02'
  version: 'versionB'
}
]
module secret 'secret.bicep' = [for (secret, i) in secrets : {
  name: 'secret-{i}'
  params: {
    mySecret: '${kv.getSecret(secret.name, secret.version)}'
  }
}]
"),
                ("secret.bicep", @"
@secure()
param mySecret string = 'defaultSecret'

output exposed string = mySecret
"));

            result.Should().NotGenerateATemplate();
            result.Should().OnlyContainDiagnostic("BCP179", DiagnosticLevel.Error, "Function \"getSecret\" is not valid at this location. It can only be used when directly assigning a value to a module parameter with a secure decorator.");
        }
    }
}

