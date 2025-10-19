// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParameterTests
    {
        private ServiceBuilder ServicesWithResourceTyped => new ServiceBuilder().WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true));

        [NotNull]
        public TestContext? TestContext { get; set; }

        private ServiceBuilder ServicesWithExtensions => new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true))
            .WithConfigurationPatch(c => c.WithExtensions("""
            {
              "az": "builtin:",
              "kubernetes": "builtin:",
              "foo": "builtin:",
              "bar": "builtin:"
            }
            """))
            .WithNamespaceProvider(TestExtensionsNamespaceProvider.CreateWithDefaults());

        [TestMethod]
        public void Parameter_can_have_resource_type()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties:{
    accessTier: p.properties.accessTier
  }
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            var parameterSymbol = model.Root.ParameterDeclarations.Should().ContainSingle().Subject;
            var typeInfo = model.GetTypeInfo(parameterSymbol.DeclaringSyntax);
            typeInfo.Should().BeOfType<ResourceType>().Which.TypeReference.FormatName().Should().BeEquivalentTo("Microsoft.Storage/storageAccounts@2019-06-01");

            var reference = model.FindReferences(parameterSymbol).OfType<VariableAccessSyntax>().Should().ContainSingle().Subject;
            model.GetDeclaredType(reference).Should().NotBeNull();
            model.GetTypeInfo(reference).Should().NotBeNull();

            result.Template.Should().HaveValueAtPath("$.resources[0].properties.accessTier", "[reference(parameters('p'), '2019-06-01').accessTier]");
            result.Template.Should().HaveValueAtPath("$.parameters.p", new JObject()
            {
                ["type"] = new JValue("string"),
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                },
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_can_have_decorators()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
@description('cool')
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: 'test'
  location: 'eastus'
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties:{
    accessTier: p.properties.accessTier
  }
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.parameters.p", new JObject()
            {
                ["type"] = new JValue("string"),
                ["metadata"] = new JObject()
                {
                    ["resourceType"] = new JValue("Microsoft.Storage/storageAccounts@2019-06-01"),
                    ["description"] = new JValue("cool"),
                },
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_can_have_properties_evaluated()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

output id string = p.id
output name string = p.name
output type string = p.type
output apiVersion string = p.apiVersion
output accessTier string = p.properties.accessTier
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.id", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[parameters('p')]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.name", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[last(split(parameters('p'), '/'))]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.type", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("Microsoft.Storage/storageAccounts"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.apiVersion", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("2019-06-01"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.accessTier", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[reference(parameters('p'), '2019-06-01').accessTier]"),
            });
        }

        [TestMethod]
        public void Parameter_can_have_warnings_for_missing_type()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped, @"
param p resource 'Some.Fake/Type@2019-06-01'

output id string = p.id
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
            });
        }

        [TestMethod]
        public void Parameter_can_only_use_az_resource_type()
        {
            var result = CompilationHelper.Compile(ServicesWithExtensions, """
            extension bar with {
            connectionString: 'asdf'
            } as stg

            param container resource 'stg:container'
            output name string = container.name // silence unused params warning
            """);
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP227", DiagnosticLevel.Error, "The type \"container\" cannot be used as a parameter or output type. Resource types from extensions are currently not supported as parameters or outputs."),
                ("BCP062", DiagnosticLevel.Error, "The referenced declaration with name \"container\" is not valid."),
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_cannot_be_used_as_extension_scope()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

resource resource 'My.Rp/myResource@2020-01-01' = {
  scope: p
  name: 'resource'
}");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"My.Rp/myResource@2020-01-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                ("BCP229", DiagnosticLevel.Error, "The parameter \"p\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource."),
            });
        }

        [TestMethod]
        public void Parameter_with_resource_type_cannot_be_used_as_parent()
        {
            var result = CompilationHelper.Compile(ServicesWithResourceTyped, @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'

resource resource 'Microsoft.Storage/storageAccounts/tableServices@2020-06-01' = {
  parent: p
  name: 'child1'
  properties: {
    cors: {
      corsRules: []
    }
  }
}");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[]
            {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Microsoft.Storage/storageAccounts/tableServices@2020-06-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                ("BCP229", DiagnosticLevel.Error, "The parameter \"p\" cannot be used as a resource scope or parent. Resources passed as parameters cannot be used as a scope or parent of a resource."),
                ("BCP169", DiagnosticLevel.Error, "Expected resource name to contain 1 \"/\" character(s). The number of name segments must match the number of segments in the resource type."),
            });
        }

        [TestMethod]
        public void Parameter_with_string_interpolation()
        {
            var result = CompilationHelper.CompileParams(
              ("test.txt", @"Hello $NAME!"),
              ("parameters.bicepparam", @"
using 'main.bicep'

param foo = 'foo'
param bar = 'bar${foo}bar'
param baz = replace(loadTextContent('test.txt'), '$NAME', 'Anthony')
"),
              ("main.bicep", @"
param foo string
param bar string
param baz string

output baz string = '${foo}${bar}'
"));

            // Exclude the "No using declaration is present in this parameters file" diagnostic
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Parameters.Should().DeepEqual(JToken.Parse(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""value"": ""foo""
    },
    ""bar"": {
      ""value"": ""barfoobar""
    },
    ""baz"": {
      ""value"": ""Hello Anthony!""
    }
  }
}"));
        }

        [TestMethod]
        public void Non_deterministic_functions_are_blocked()
        {
            var result = CompilationHelper.CompileParams(
              ("parameters.bicepparam", @"
using 'main.bicep'

param foo = utcNow()
param bar = newGuid()
"),
              ("main.bicep", @"
param foo string
param bar string
"));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP065", DiagnosticLevel.Error, "Function \"utcNow\" is not valid at this location. It can only be used as a parameter default value."),
                ("BCP065", DiagnosticLevel.Error, "Function \"newGuid\" is not valid at this location. It can only be used as a parameter default value."),
            });
        }

        [TestMethod]
        public void Az_functions_are_blocked()
        {
            var result = CompilationHelper.CompileParams(
              ("parameters.bicepparam", @"
using 'main.bicep'

param foo = resourceId('Microsoft.Compute/virtualMachines', 'foo')
param bar = deployment()
"),
              ("main.bicep", @"
param foo string
param bar object
"));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP057", DiagnosticLevel.Error, "The name \"resourceId\" does not exist in the current context."),
                ("BCP057", DiagnosticLevel.Error, "The name \"deployment\" does not exist in the current context."),
            });
        }

        [TestMethod]
        public void Parameter_with_complex_functions()
        {
            var result = CompilationHelper.CompileParams(
              ("test.txt", @"Hello $NAME!"),
              ("parameters.bicepparam", @"
using 'main.bicep'

param foo = 'foo/bar/baz'
param bar = [
  toLower(foo)
  toUpper(foo)
  map(split(foo, '/'), v => { segment: v })
  replace(loadTextContent('test.txt'), '$NAME', 'Anthony')
]
"),
              ("main.bicep", @"
param foo string
param bar array
"));

            // Exclude the "No using declaration is present in this parameters file" diagnostic
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Parameters.Should().DeepEqual(JToken.Parse(@"{
  ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
  ""contentVersion"": ""1.0.0.0"",
  ""parameters"": {
    ""foo"": {
      ""value"": ""foo/bar/baz""
    },
    ""bar"": {
      ""value"": [
        ""foo/bar/baz"",
        ""FOO/BAR/BAZ"",
        [
          {
            ""segment"": ""foo""
          },
          {
            ""segment"": ""bar""
          },
          {
            ""segment"": ""baz""
          }
        ],
        ""Hello Anthony!""
      ]
    }
  }
}"));
        }

        [TestMethod]
        public void Missing_required_parameters_generates_a_codefix()
        {
            var result = CompilationHelper.CompileParams(
              ("parameters.bicepparam", """
using './main.bicep'
"""),
              ("main.bicep", """
param stringParam string
param intParam int
param boolParam bool
param objectParam object
param arrayParam array
"""));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics([
                ("BCP258", DiagnosticLevel.Error, """The following parameters are declared in the Bicep file but are missing an assignment in the params file: "arrayParam", "boolParam", "intParam", "objectParam", "stringParam".""")
            ]);

            result.ApplyCodeFix(result.Diagnostics.Single(x => x.Code == "BCP258")).Should().EqualIgnoringNewlines("""
using './main.bicep'

param arrayParam =  /*TODO*/

param boolParam =  /*TODO*/

param intParam =  /*TODO*/

param objectParam =  /*TODO*/

param stringParam =  /*TODO*/

""");
        }

        [TestMethod]
        public void Valid_extends_should_not_fail()
        {
            var result = CompilationHelper.CompileParams(
              ("bicepconfig.json", @"
                {
                    ""experimentalFeaturesEnabled"": {
                        ""extendableParamFiles"": true
                    }
                }
              "), ("parameters.bicepparam", @"
                using 'main.bicep'
                extends 'shared.bicepparam'
              "),
              ("shared.bicepparam", @"
                using none
              "),
              ("main.bicep", @"
              "));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Invalid_extends_reference_does_not_exist_should_fail()
        {
            var result = CompilationHelper.CompileParams(
              ("bicepconfig.json", @"
                {
                    ""experimentalFeaturesEnabled"": {
                        ""extendableParamFiles"": true
                    }
                }
              "),
              ("parameters.bicepparam", @"
                using 'main.bicep'
                extends 'does-not-exists.bicepparam'
                param foo = ''
              "),
              ("main.bicep", @"
                param foo string
              "));

            var path = InMemoryFileResolver.GetFileUri("/path/to/does-not-exists.bicepparam").LocalPath;

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP091", DiagnosticLevel.Error, $"An error occurred reading file. Could not find file '{path}'."),
            });
        }

        [TestMethod]
        public void Invalid_extends_reference_file_type_should_fail()
        {
            var result = CompilationHelper.CompileParams(
              ("bicepconfig.json", @"
                {
                    ""experimentalFeaturesEnabled"": {
                        ""extendableParamFiles"": true
                    }
                }
              "),
              ("parameters.json", @"
                { ""foo"": ""bar"" }
              "),
              ("parameters.bicepparam", @"
                using 'main.bicep'
                extends 'parameters.json'
                param foo = ''
              "),
              ("main.bicep", @"
                param foo string
              "));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP404", DiagnosticLevel.Error, "The \"extends\" declaration is missing a bicepparam file path reference"),
            });
        }

        [TestMethod]
        public void Invalid_extends_and_more_than_one_extends_should_fail()
        {
            var result = CompilationHelper.CompileParams(
              ("parameters.bicepparam", @"
                using 'main.bicep'
                extends
                extends 'shared.bicepparam'
                param foo = ''
                param bar = ''
              "),
              ("shared.bicepparam", @"
                using none
                param foo = ''
              "),
              ("main.bicep", @"
                param foo string
                param bar string
              "));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP405", DiagnosticLevel.Error, "More than one \"extends\" declaration are present"),
                ("BCP406", DiagnosticLevel.Error, "Using \"extends\" keyword requires enabling EXPERIMENTAL feature \"ExtendableParamFiles\"."),
                ("BCP404", DiagnosticLevel.Error, "The \"extends\" declaration is missing a bicepparam file path reference"),
                ("BCP405", DiagnosticLevel.Error, "More than one \"extends\" declaration are present"),
                ("BCP406", DiagnosticLevel.Error, "Using \"extends\" keyword requires enabling EXPERIMENTAL feature \"ExtendableParamFiles\"."),
            });
        }

        [TestMethod]
        public void Extending_parameters_allow_overriding_should_succeed()
        {
            var result = CompilationHelper.CompileParams(
              ("bicepconfig.json", @"
                {
                    ""experimentalFeaturesEnabled"": {
                        ""extendableParamFiles"": true
                    }
                }
              "),
              ("parameters.bicepparam", @"
                using 'main.bicep'
                extends 'shared.bicepparam'
                param foo = 'bar'
              "),
              ("shared.bicepparam", @"
                using none
                param foo = 'foo'
              "),
              ("main.bicep", @"
                param foo string
              "));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Parameters.Should().DeepEqual(JToken.Parse(@"{
            ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
            ""contentVersion"": ""1.0.0.0"",
            ""parameters"": {
                ""foo"": {
                ""value"": ""bar""
                },
            }
            }"));
        }

        [TestMethod]
        public void Using_variables_objects_arrays_in_base_parameters_file_should_succeed()
        {
            var result = CompilationHelper.CompileParams(
              ("bicepconfig.json", @"
                {
                    ""experimentalFeaturesEnabled"": {
                        ""extendableParamFiles"": true
                    }
                }
              "),
              ("parameters.bicepparam", @"
                using 'main.bicep'
                extends 'shared.bicepparam'
              "),
              ("shared.bicepparam", @"
                using none

                var var_foo = 'foo'
                param foo = var_foo

                var var_bar = {
                    a: 'a'
                    b: 'b'
                }

                param bar = var_bar

                var var_baz = [
                    var_foo
                    var_bar
                ]

                param baz = var_baz
              "),
              ("main.bicep", @"
                param foo string = ''
                param bar object = {}
                param baz array = []
              "));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Parameters.Should().DeepEqual(JToken.Parse(@"{
                ""$schema"": ""https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#"",
                ""contentVersion"": ""1.0.0.0"",
                ""parameters"": {
                    ""foo"": {
                        ""value"": ""foo""
                    },
                    ""bar"": {
                        ""value"": {
                            ""a"": ""a"",
                            ""b"": ""b""
                        }
                    },
                    ""baz"": {
                        ""value"": [
                            ""foo"",
                            {
                                ""a"": ""a"",
                                ""b"": ""b""
                            }
                        ]
                    }
                }
            }"));
        }

        [TestMethod]
        public void Decorators_on_using_param_and_extends_statements_should_raise_errors()
        {
            var result = CompilationHelper.CompileParams(
              ("bicepconfig.json", @"
                {
                    ""experimentalFeaturesEnabled"": {
                        ""extendableParamFiles"": true
                    }
                }
              "),
              ("parameters.bicepparam", @"
                @foo('bar')
                using 'main.bicep'

                @notARealDecorator(1, 2, 3)
                param fizz = 'buzz'

                @minLength(3)
                extends 'shared.bicepparam'
              "),
              ("shared.bicepparam", @"
                using none
              "),
              ("main.bicep", @"
                param fizz string
              "));

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP130", DiagnosticLevel.Error, "Decorators are not allowed here."),
                ("BCP130", DiagnosticLevel.Error, "Decorators are not allowed here."),
                ("BCP130", DiagnosticLevel.Error, "Decorators are not allowed here."),
            });
        }

        [TestMethod]
        public void Decorators_on_param_declarations_in_bicep_files_should_be_allowed()
        {
            var result = CompilationHelper.Compile(@"
                @description('A parameter with a decorator')
                @minLength(3)
                param myParam string

                @secure()
                param secureParam string
            ");

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }
    }
}
