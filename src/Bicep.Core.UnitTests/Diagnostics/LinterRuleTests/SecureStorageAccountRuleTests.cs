// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter.Rules;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class SecureStorageAccountRuleTests : LinterRuleTestsBase
{
    private static void CompileAndTest(string text, params string[] expectedProperties)
    {
        var options = new Options(ConfigurationPatch: configuration => TestConfigurationBuilder
            .Create(configuration)
            .WithAnalyzer(SecureStorageAccountRule.Code, DiagnosticLevel.Warning)
            .Build());

        AssertLinterRuleDiagnostics(SecureStorageAccountRule.Code, text, diagnostics =>
        {
            var rule = new SecureStorageAccountRule();
            var expectedMessages = expectedProperties
                .Select(property => rule.GetMessage(property, GetRecommendedValue(property)))
                .ToArray();

            diagnostics.Select(diagnostic => diagnostic.Message).Should().BeEquivalentTo(expectedMessages);
        }, options);
    }

    [TestMethod]
    public void Secure_storage_account_properties_should_not_produce_diagnostics()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'storageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}");
    }

    [TestMethod]
    public void Tls13_should_not_produce_a_diagnostic()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'storageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_3'
  }
}");
    }

    [TestMethod]
    public void Tls11_should_produce_a_diagnostic()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'storageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: false
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_1'
  }
}", "minimumTlsVersion");
    }

    [TestMethod]
    public void Missing_storage_account_properties_should_produce_diagnostics()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'storageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}",
            "allowBlobPublicAccess",
            "supportsHttpsTrafficOnly",
            "minimumTlsVersion");
    }

    [TestMethod]
    public void Unsafe_literal_storage_account_properties_should_produce_diagnostics()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'storageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: true
    supportsHttpsTrafficOnly: false
    minimumTlsVersion: 'TLS1_0'
  }
}",
            "allowBlobPublicAccess",
            "supportsHttpsTrafficOnly",
            "minimumTlsVersion");
    }

    [TestMethod]
    public void Existing_storage_accounts_should_not_produce_diagnostics()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: 'storageaccount'
}");
    }

    [TestMethod]
    public void Other_resources_should_not_produce_diagnostics()
    {
        CompileAndTest(@"
resource storage 'Microsoft.Web/sites@2023-01-01' = {
  name: 'site'
  location: resourceGroup().location
  properties: {
    allowBlobPublicAccess: true
    supportsHttpsTrafficOnly: false
    minimumTlsVersion: 'TLS1_0'
  }
}");
    }

    [TestMethod]
    public void Expressions_should_not_produce_diagnostics()
    {
        CompileAndTest(@"
param allowBlobPublicAccess bool
param supportsHttpsTrafficOnly bool
param minimumTlsVersion string

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: 'storageaccount'
  location: resourceGroup().location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    allowBlobPublicAccess: allowBlobPublicAccess
    supportsHttpsTrafficOnly: supportsHttpsTrafficOnly
    minimumTlsVersion: minimumTlsVersion
  }
}");
    }

    private static string GetRecommendedValue(string propertyName) => propertyName switch
    {
        "allowBlobPublicAccess" => "false",
        "supportsHttpsTrafficOnly" => "true",
        "minimumTlsVersion" => "'TLS1_2'",
        _ => throw new ArgumentOutOfRangeException(nameof(propertyName)),
    };
}
