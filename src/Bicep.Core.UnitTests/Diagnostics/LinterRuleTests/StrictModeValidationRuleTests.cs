// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Diagnostics.LinterRuleTests;

[TestClass]
public class StrictModeValidationRuleTests : LinterRuleTestsBase
{
    [TestMethod]
    public void Module_output_used_to_format_resource_name()
    {
        var result = CompilationHelper.Compile(
            new (string fileName, string fileContents)[] {
                ("stg.bicep", @"
param foo string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: foo
#disable-next-line no-hardcoded-location
  location: 'West US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

output foo string = stg.properties.accessTier                
                "),
                ("main.bicep", @"
module a 'stg.bicep' = {
  name: 'asdf'
  params: {
    foo: 'blah'
  }
}

module b 'stg.bicep' = {
  name: 'asdf2'
  params: {
    foo: a.outputs.foo
  }
}
                ")});

        result.Diagnostics.Should().ContainDiagnostic("strict-mode", DiagnosticLevel.Warning, "Module \"b\" uses parameter \"foo\" to generate the name or scope of a resource. Supply a value that is known at the start of the deployment to ensure full preflight or what-if validation.");
    }

    [TestMethod]
    public void Resource_property_used_to_format_resource_name()
    {
        var result = CompilationHelper.Compile(
            new (string fileName, string fileContents)[] {
                ("stg.bicep", @"
param foo string

resource stg 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: foo
#disable-next-line no-hardcoded-location
  location: 'West US'
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
}

output foo string = foo                
                "),
                ("main.bicep", @"
resource a 'Microsoft.Storage/storageAccounts@2022-09-01' existing = {
  name: 'blah'
}

module b 'stg.bicep' = {
  name: 'asdf2'
  params: {
    foo: a.properties.accessTier
  }
}
                ")});

        result.Diagnostics.Should().ContainDiagnostic("strict-mode", DiagnosticLevel.Warning, "Module \"b\" uses parameter \"foo\" to generate the name or scope of a resource. Supply a value that is known at the start of the deployment to ensure full preflight or what-if validation.");
    }
}
