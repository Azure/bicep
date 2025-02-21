// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class DecoratorTests
    {
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void ParameterDecorator_MissingDeclaration_ExpectedParameterDeclaration()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@secure()
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP290", DiagnosticLevel.Error, "Expected a parameter or type declaration after the decorator."),
                });
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_MissingDeclaration_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @waitUntil(x => x.ProvisionStatus == 'Succeeded', 'PT20S')
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP149", DiagnosticLevel.Error, "Expected a resource declaration after the decorator."),
                });
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_MissingParameters_ExpectedTwoParameters()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @waitUntil(x => x.ProvisionStatus == 'Succeeded')
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP149", DiagnosticLevel.Error, "Expected a resource declaration after the decorator."),
                    ("BCP071", DiagnosticLevel.Error, "Expected 2 arguments, but got 1."),

                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'])
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP149", DiagnosticLevel.Error, "Expected a resource declaration after the decorator."),
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_WithModuleDeclaration_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
                    @retryOn(['ResourceNotFound'])
                    module myModule 'module.bicep' = {
                      name: 'moduleb'
                      params: {
                        inputa: 'foo'
                        inputb: 'bar'
                      }
                    }
                    "
                ,
                [moduleUri] = @"
                    param inputa string
                    param inputb string
                    "
            };

            var compilation = services.BuildCompilation(files, mainUri);
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => !d.IsError());

            using (new AssertionScope())
            {
                diagnosticsByFile[mainUri].ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP128", DiagnosticLevel.Error, "Function \"retryOn\" cannot be used as a module decorator.")
                });
                success.Should().BeFalse();
            }
        }

        [TestMethod]
        public void RetryOnDecorator_WithRetryCountOptionalParameter_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 5)
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP149", DiagnosticLevel.Error, "Expected a resource declaration after the decorator."),
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_InvalidRetryCount()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 0)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 0) is too small to assign to a target for which the minimum allowable value is 1.")
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_NegativeRetryCount()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], -5)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to -5) is too small to assign to a target for which the minimum allowable value is 1."),
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_NonIntegerRetryCountValue()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 'randomString')
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"'randomString'\" is not assignable to parameter of type \"int\"."),
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_InvalidErrorMessageItemType()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound', 1010])
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().NotBeEmpty();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"string\" but the provided value is of type \"'ResourceNotFound' | 1010\".")
                });

            }
        }

        [TestMethod]
        public void RetryOnDecorator_ValidScenario()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound', 'ServerError'], 1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");

            var retryOnJObject = new JObject
            {
                ["exceptionCodes"] = new JArray("ResourceNotFound", "ServerError"),
                ["retryCount"] = 1
            };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources[0].options.retryOn", retryOnJObject);

            }
        }

        [TestMethod]
        public void ParameterDecorator_AttachedToOtherKindsOfDeclarations_CannotBeUsedAsDecoratorSpecificToTheDeclarations()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
@maxLength(1)
var foo = true

@allowed([
  true
])
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
    location: 'westus'
    name: 'myVNet'
    properties:{
        addressSpace: {
            addressPrefixes: [
                '10.0.0.0/20'
            ]
        }
    }
}

@secure()
module myModule 'module.bicep' = {
  name: 'moduleb'
  params: {
    inputa: 'foo'
    inputb: 'bar'
  }
}
",
                [moduleUri] = @"
param inputa string
param inputb string
",
            };

            var compilation = Services.BuildCompilation(files, mainUri);
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => !d.IsError());

            using (new AssertionScope())
            {
                diagnosticsByFile[mainUri].ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP126", DiagnosticLevel.Error, "Function \"maxLength\" cannot be used as a variable decorator."),
                    ("BCP127", DiagnosticLevel.Error, "Function \"allowed\" cannot be used as a resource decorator."),
                    ("BCP128", DiagnosticLevel.Error, "Function \"secure\" cannot be used as a module decorator."),
                });
                success.Should().BeFalse();
            }
        }

        [TestMethod]
        public void MetadataDecorator_AttachedToOutputDeclaration_CanBeUsed()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@metadata({
  some: 'sample-metadata'
})
output test bool = true
");
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("outputs.test.metadata.some", new JValue("sample-metadata"));
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
            }
        }

        [TestMethod]
        public void ConstraintDecorators_AttachedToOutputDeclaration_CanBeUsed()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@minValue(2)
@maxValue(3)
output test int = 2

@minLength(2)
@maxLength(3)
output stringTest string = 'foo'

@minLength(2)
@maxLength(3)
output arrayTest array = ['fizz', 'buzz']
");
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("outputs.test.minValue", new JValue(2));
                template.Should().HaveValueAtPath("outputs.test.maxValue", new JValue(3));
                template.Should().HaveValueAtPath("outputs.stringTest.minLength", new JValue(2));
                template.Should().HaveValueAtPath("outputs.stringTest.maxLength", new JValue(3));
                template.Should().HaveValueAtPath("outputs.arrayTest.minLength", new JValue(2));
                template.Should().HaveValueAtPath("outputs.arrayTest.maxLength", new JValue(3));
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
            }
        }

        [TestMethod]
        public void MetadataDecorator_AttachedToOutputDeclaration_IsMergedWithDescriptionDecorator()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@metadata({
  some: 'sample-metadata'
})
@description('this is some helpful text, which is compiled into in the metadata object')
output test bool = true
");
            using (new AssertionScope())
            {
                template.Should().HaveValueAtPath("outputs.test.metadata.some", new JValue("sample-metadata"));
                template.Should().HaveValueAtPath("outputs.test.metadata.description", new JValue("this is some helpful text, which is compiled into in the metadata object"));
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
            }
        }

        [TestMethod]
        public void NonDecoratorFunction_MissingDeclaration_CannotBeUsedAsDecorator()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
@concat()
@resourceId()
");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP152", DiagnosticLevel.Error, "Function \"concat\" cannot be used as a decorator."),
                    ("BCP132", DiagnosticLevel.Error, "Expected a declaration after the decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"resourceId\" cannot be used as a decorator.")
                });
            }
        }

        [TestMethod]
        public void NonDecoratorFunction_AttachedToDeclaration_CannotBeUsedAsDecorator()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
@resourceId()
param foo string

@concat()
var bar = true

@environment()
resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01' = {
    location: 'westus'
    name: 'myVNet'
    properties:{
        addressSpace: {
            addressPrefixes: [
                '10.0.0.0/20'
            ]
        }
    }
}

@union()
module myModule 'module.bicep' = {
  name: 'moduleb'
  params: {
    inputa: 'foo'
    inputb: 'bar'
  }
}

@guid()
output baz bool = false
",
                [moduleUri] = @"
param inputa string
param inputb string
",
            };

            var compilation = Services.BuildCompilation(files, mainUri);
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => !d.IsError());

            using (new AssertionScope())
            {
                diagnosticsByFile[mainUri].ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP152", DiagnosticLevel.Error, "Function \"resourceId\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"concat\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"environment\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"union\" cannot be used as a decorator."),
                    ("BCP152", DiagnosticLevel.Error, "Function \"guid\" cannot be used as a decorator."),
                });
                success.Should().BeFalse();
            }
        }

        /// <summary>
        /// https://github.com/Azure/bicep/issues/10970
        /// </summary>
        [TestMethod]
        public void DecoratorsOnNestedChildResource_CanBeUsed()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
var dbs = [
    'db1'
    'db2'
    'db3'
]
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'
  location: 'polandcentral'

  @batchSize(1)
  @description('Sql Databases')
  resource sqlDatabase 'databases' = [for db in dbs: {
    name: db
    location: 'polandcentral'
  }]

  @description('Primary Sql Database')
  resource primaryDb 'databases' = {
    name: 'primary'
    location: 'polandcentral'
  }
}");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources[0].copy.mode", "serial")
                    .And.HaveValueAtPath("$.resources[0].copy.batchSize", 1);
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources[0].metadata.description", "Sql Databases");
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources[1].metadata.description", "Primary Sql Database");
            }
        }

        [TestMethod]
        public void DecoratorDescriptionInResourceBody_ShouldPromptForDeclaration()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'
  location: 'polandcentral'

  @description('Primary Sql Database')
}
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP132", DiagnosticLevel.Error, "Expected a declaration after the decorator."),
                });
                template.Should().BeNull();
            }
        }

        [TestMethod]
        public void DecoratorBatchSizeInResourceBody_ShouldPromptForResourceDeclaration()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'
  location: 'polandcentral'

  @batchSize(1)
}
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP153", DiagnosticLevel.Error, "Expected a resource or module declaration after the decorator."),
                });
                template.Should().BeNull();
            }
        }

        [TestMethod]
        public void UnfinishedDecoratorInResourceBody_ShouldPromptForNamespaceOrDecoratorName()
        {
            var (template, diagnostics, _) = CompilationHelper.Compile(@"
resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: 'sql-server-name'
  location: 'polandcentral'

  @
}
");
            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
                {
                    ("BCP123", DiagnosticLevel.Error, "Expected a namespace or decorator name at this location."),
                });
                template.Should().BeNull();
            }
        }
    }
}
