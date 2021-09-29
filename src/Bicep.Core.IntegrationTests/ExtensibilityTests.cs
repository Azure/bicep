// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExtensibilityTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private CompilationHelper.CompilationHelperContext EnabledImportsContext
            => new CompilationHelper.CompilationHelperContext(Features: BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true));

        [TestMethod]
        public void Storage_import_bad_config_is_blocked()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import stg from storage {
  madeUpProperty: 'asdf'
}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"connectionString\"."),
                ("BCP037", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"configuration\". Permissible properties include \"connectionString\"."),
            });
        }

        [TestMethod]
        public void Storage_import_basic_test()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import stg from storage {
  connectionString: 'asdf'
}

resource container 'AzureStorage/containers@2020-01-01' = {
  name: 'myblob'
}

resource blob 'AzureStorage/blobs@2020-01-01' = {
  name: 'myblob'
  containerName: container.name
  base64Content: base64('sadfasdfd')
}
");
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Storage_import_end_to_end_test()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, 
                ("main.bicep", @"
param accountName string

resource stgAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: toLower(accountName)
  location: resourceGroup().location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}

var connectionString = 'DefaultEndpointsProtocol=https;AccountName=${stgAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${stgAccount.listKeys().keys[0].value}'

module website './website.bicep' = {
  name: 'website'
  params: {
    connectionString: connectionString
  }
}
"),
                ("website.bicep", @"
@secure()
param connectionString string

import stg from storage {
  connectionString: connectionString
}

resource container 'AzureStorage/containers@2020-01-01' = {
  name: 'bicep'
}

resource blob 'AzureStorage/blobs@2020-01-01' = {
  name: 'blob.txt'
  containerName: container.name
  base64Content: base64(loadTextContent('blob.txt'))
}
"),
                ("blob.txt", @"
Hello from Bicep!"));

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}