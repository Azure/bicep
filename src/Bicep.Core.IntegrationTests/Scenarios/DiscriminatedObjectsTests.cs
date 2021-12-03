// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.Scenarios
{
    [TestClass]
    public class DiscriminatedObjectsTests
    {

        [TestMethod]
        // https://github.com/azure/bicep/issues/657
        public void Test_Issue657_discriminators()
        {
            var customTypes = new[] {
                new ResourceTypeComponents(
                    ResourceTypeReference.Parse("Rp.A/parent@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Rp.A/parent@2020-10-01",
                        ("name", LanguageConstants.String))),
                new ResourceTypeComponents(
                    ResourceTypeReference.Parse("Rp.A/parent/child@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateDiscriminatedObjectType(
                        "Rp.A/parent/child@2020-10-01",
                        "name",
                        TestTypeHelper.CreateObjectType(
                            "Val1Type",
                            ("name", new StringLiteralType("val1")),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal1", LanguageConstants.Bool)))),
                        TestTypeHelper.CreateObjectType(
                            "Val2Type",
                            ("name", new StringLiteralType("val2")),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnVal2", LanguageConstants.Bool)))))),
            };

            var result = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// top-level resource
resource test2 'Rp.A/parent/child@2020-10-01' = {
  name: 'test/test2'
  properties: {
    anythingGoesHere: true
  }
}

// 'existing' top-level resource
resource test3 'Rp.A/parent/child@2020-10-01' existing = {
  name: 'test/test3'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'val1'
  properties: {
    onlyOnVal1: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'val2'
}
"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var failedResult = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'notAValidVal'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'notAValidVal'
}
"));

            failedResult.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
            });
        }

        [TestMethod]
        // https://github.com/azure/bicep/issues/657
        public void Test_Issue657_enum()
        {
            var customTypes = new[] {
                new ResourceTypeComponents(
                    ResourceTypeReference.Parse("Rp.A/parent@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Rp.A/parent@2020-10-01",
                        ("name", LanguageConstants.String))),
                new ResourceTypeComponents(
                    ResourceTypeReference.Parse("Rp.A/parent/child@2020-10-01"),
                    ResourceScope.ResourceGroup,
                    TestTypeHelper.CreateObjectType(
                        "Rp.A/parent/child@2020-10-01",
                        ("name", TypeHelper.CreateTypeUnion(new StringLiteralType("val1"), new StringLiteralType("val2"))),
                            ("properties", TestTypeHelper.CreateObjectType(
                                "properties",
                                ("onlyOnEnum", LanguageConstants.Bool))))),
            };

            var result = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// top-level resource
resource test2 'Rp.A/parent/child@2020-10-01' = {
  name: 'test/test2'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' top-level resource
resource test3 'Rp.A/parent/child@2020-10-01' existing = {
  name: 'test/test3'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'val1'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'val2'
}
"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var failedResult = CompilationHelper.Compile(
                TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(customTypes),
                ("main.bicep", @"
resource test 'Rp.A/parent@2020-10-01' = {
  name: 'test'
}

// parent-property child resource
resource test4 'Rp.A/parent/child@2020-10-01' = {
  parent: test
  name: 'notAValidVal'
  properties: {
    onlyOnEnum: true
  }
}

// 'existing' parent-property child resource
resource test5 'Rp.A/parent/child@2020-10-01' existing = {
  parent: test
  name: 'notAValidVal'
}
"));

            failedResult.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"'val1' | 'val2'\" but the provided value is of type \"'notAValidVal'\"."),
            });
        }


        [TestMethod]
        // https://github.com/Azure/bicep/issues/2262
        public void Test_Issue2262()
        {
            // Wrong discriminated key: PartitionScheme.
            var result = CompilationHelper.Compile(@"
resource service 'Microsoft.ServiceFabric/clusters/applications/services@2020-12-01-preview' = {
  name: 'myCluster/myApp/myService'
  properties: {
    serviceKind: 'Stateful'
    partitionDescription: {
      PartitionScheme: 'Named'
      names: [
        'foo'
      ]
      count: 1
    }
  }
}
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP078", DiagnosticLevel.Warning, "The property \"partitionScheme\" requires a value of type \"'Named' | 'Singleton' | 'UniformInt64Range'\", but none was supplied."),
                ("BCP089", DiagnosticLevel.Warning, "The property \"PartitionScheme\" is not allowed on objects of type \"'Named' | 'Singleton' | 'UniformInt64Range'\". Did you mean \"partitionScheme\"?"),
            });

            var diagnosticWithCodeFix = result.Diagnostics.OfType<FixableDiagnostic>().Single();
            var codeFix = diagnosticWithCodeFix.Fixes.Single();
            var codeReplacement = codeFix.Replacements.Single();

            codeReplacement.Span.Should().Be(new TextSpan(212, 15));
            codeReplacement.Text.Should().Be("partitionScheme");
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/4668
        /// </summary>
        [TestMethod]
        public void Issue_4668_1()
        {
            var result = CompilationHelper.Compile(@"
@description('The language of the Deployment Script. AzurePowerShell or AzureCLI.')
@allowed([
  'AzureCLI'
  'AzurePowerShell'
])
param kind string = 'AzureCLI'

@description('The identity that will be used to execute the Deployment Script.')
param identity object

@description('The properties of the Deployment Script.')
param properties object

resource mainResource 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'testscript'
  location: 'westeurope'
  kind: kind
  identity: identity
  properties: properties
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP225", DiagnosticLevel.Warning, "The discriminator property \"kind\" value cannot be determined at compilation time. Type checking for this object is disabled.")
            }).And.GenerateATemplate();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/4668
        /// </summary>
        [TestMethod]
        public void Issue_4668_2()
        {
            var result = CompilationHelper.Compile(new CompilationHelper.CompilationHelperContext(NamespaceProvider: BuiltInTestTypes.Create()), @"
param properties object

resource mainResource 'Test.Rp/discriminatedPropertiesTests@2020-01-01' = {
  name: 'testresource'
  properties: properties
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/4668
        /// </summary>
        [TestMethod]
        public void Issue_4668_3()
        {
            var result = CompilationHelper.Compile(new CompilationHelper.CompilationHelperContext(NamespaceProvider: BuiltInTestTypes.Create()), @"
@allowed([
  'PropertiesA'
  'PropertiesB'
])
param propType string
param values object

resource mainResource 'Test.Rp/discriminatedPropertiesTests2@2020-01-01' = {
  name: 'testresource'
  properties: {
    propType: propType
    values: values
  }
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP225", DiagnosticLevel.Warning, "The discriminator property \"propType\" value cannot be determined at compilation time. Type checking for this object is disabled.")
            }).And.GenerateATemplate();
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/4668
        /// </summary>
        [TestMethod]
        public void Issue_4668_4()
        {
            var result = CompilationHelper.Compile(new CompilationHelper.CompilationHelperContext(NamespaceProvider: BuiltInTestTypes.Create()), @"
param propType string
param values object

resource mainResource 'Test.Rp/discriminatedPropertiesTests2@2020-01-01' = {
  name: 'testresource'
  properties: {
    propType: propType
    values: values
  }
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP225", DiagnosticLevel.Warning, "The discriminator property \"propType\" value cannot be determined at compilation time. Type checking for this object is disabled.")
            }).And.GenerateATemplate();
        }

    }
}
