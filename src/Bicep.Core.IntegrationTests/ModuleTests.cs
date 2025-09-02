// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.TextFixtures.Assertions;
using Bicep.TextFixtures.IO;
using Bicep.TextFixtures.Mocks;
using Bicep.TextFixtures.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ModuleTests
    {
        private readonly TestCompiler compiler = TestCompiler.ForMockFileSystemCompilation();

        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private ServiceBuilder ServicesWithResourceTyped => new ServiceBuilder().WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true));

        private ServiceBuilder ServicesWithExtensionConfigs => new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, ModuleExtensionConfigsEnabled: true));

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void Modules_can_be_compiled_successfully()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var moduleBUri = new Uri("file:///moduleb.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

module moduleb 'moduleb.bicep' = {
  name: 'moduleb'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

output outputa string = modulea.outputs.outputa
output outputb string = moduleb.outputs.outputb
",
                [moduleAUri] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
                [moduleBUri] = @"
param inputa string
param inputb string

output outputb string = '${inputa}-${inputb}'
",
            };

            var compilation = Services.BuildCompilation(files, mainUri);

            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            compilation.GetTestTemplate().Should().NotBeEmpty();
        }

        [TestMethod]
        public void Module_self_cycle_is_detected_correctly()
        {
            var mainUri = new Uri("file:///main.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module mainRecursive 'main.bicep' = {
  name: 'mainRecursive'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
",
            };

            var compilation = Services.BuildCompilation(files, mainUri);

            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP094", DiagnosticLevel.Error, "This module references itself, which is not allowed."),
            });

            success.Should().BeFalse();
        }

        [TestMethod]
        public void Module_cycles_are_detected_correctly()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var moduleBUri = new Uri("file:///moduleb.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
",
                [moduleAUri] = @"
param inputa string
param inputb string

module moduleb 'moduleb.bicep' = {
  name: 'moduleb'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
",
                [moduleBUri] = @"
param inputa string
param inputb string

module main 'main.bicep' = {
  inputa: inputa
  inputb: inputb
}
",
            };

            var compilation = Services.BuildCompilation(files, mainUri);

            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The file is involved in a cycle (\"/modulea.bicep\" -> \"/moduleb.bicep\" -> \"/main.bicep\")."),
            });
            diagnosticsByFile[moduleAUri].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The file is involved in a cycle (\"/moduleb.bicep\" -> \"/main.bicep\" -> \"/modulea.bicep\")."),
            });
            diagnosticsByFile[moduleBUri].Should().HaveDiagnostics(new[] {
                ("BCP095", DiagnosticLevel.Error, "The file is involved in a cycle (\"/main.bicep\" -> \"/modulea.bicep\" -> \"/moduleb.bicep\")."),
            });
            success.Should().BeFalse();
        }

        private delegate bool TryReadDelegate(Uri fileUri, out string? fileContents, out DiagnosticBuilder.DiagnosticBuilderDelegate? failureBuilder);

        [TestMethod]
        public void SourceFileGroupingBuilder_build_should_throw_diagnostic_exception_if_entrypoint_file_read_fails()
        {
            var fileUri = new IOUri("file", "", "/path/to/main.bicep");

            var fileHandleMock = StrictMock.Of<IFileHandle>();
            fileHandleMock.Setup(x => x.OpenRead()).Throws(new IOException("Mock read failure!"));

            var fileExplorerMock = StrictMock.Of<IFileExplorer>();
            fileExplorerMock.Setup(x => x.GetFile(fileUri)).Returns(fileHandleMock.Object);

            var mockDispatcher = StrictMock.Of<IModuleDispatcher>().Object;

            Action buildAction = () => SourceFileGroupingBuilder.Build(fileExplorerMock.Object, mockDispatcher, new Workspace(), BicepTestConstants.SourceFileFactory, fileUri);
            buildAction.Should().Throw<DiagnosticException>()
                .And.Diagnostic.Should().HaveCodeAndSeverity("BCP091", DiagnosticLevel.Error).And.HaveMessage("An error occurred reading file. Mock read failure!");
        }

        [TestMethod]
        public async Task Module_should_include_diagnostic_if_module_file_cannot_be_resolved()
        {
            var mainFileUri = new Uri("file:///path/to/main.bicep");
            var mainFileText = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
";
            var result = await this.compiler.CompileInline(mainFileText);

            result.Should().HaveDiagnostics(new[] {
                ("BCP091", DiagnosticLevel.Error, $"An error occurred reading file. Could not find file '{TestFileUri.FromMockFileSystemPath("modulea.bicep")}'."),
            });
        }

        [TestMethod]
        public void Modules_should_have_metadata()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var moduleBUri = new Uri("file:///moduleb.bicep");
            var moduleCUri = new Uri("file:///modulec.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

module moduleb 'moduleb.bicep' = {
  name: 'moduleb'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

output outputa string = modulea.outputs.outputa
output outputb string = moduleb.outputs.outputb
",
                [moduleAUri] = @"
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
",
                [moduleBUri] = @"
param inputa string
param inputb string

module module 'modulec.bicep' = {
  name: 'modulec'
  params: {
    inputa: inputa
    inputb: 2
  }
}

output outputb string = '${inputa}-${inputb}'
",
                [moduleCUri] = @"
param inputa string
param inputb int

var mystr = 'hello'

output outputc1 string = '${inputa}-${mystr}'
output outputc2 int = inputb + 1
"
            };

            var compilation = Services.BuildCompilation(files, mainUri);

            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();

            var templateString = compilation.GetTestTemplate();
            var template = JToken.Parse(templateString);
            template.Should().NotBeNull();

            var mainTemplateHash = template.SelectToken(BicepTestConstants.GeneratorTemplateHashPath)?.ToString()!;
            var moduleATemplateHash = template.SelectToken("resources[0].properties.template.metadata._generator.templateHash")?.ToString()!;
            var moduleBTemplateHash = template.SelectToken("resources[1].properties.template.metadata._generator.templateHash")?.ToString()!;
            var moduleCTemplateHash = template.SelectToken("resources[1].properties.template.resources[0].properties.template.metadata._generator.templateHash")?.ToString()!;

            new HashSet<String> {
              mainTemplateHash,
              moduleATemplateHash,
              moduleBTemplateHash,
              moduleCTemplateHash,
            }.Should().HaveCount(4);

            // Confirming hashes equal individual template hashes
            ModuleTemplateHashValidator(
              Services.BuildCompilation(new Dictionary<Uri, string>
              {
                  [moduleAUri] = files[moduleAUri]
              },
              moduleAUri), moduleATemplateHash);

            ModuleTemplateHashValidator(
              Services.BuildCompilation(new Dictionary<Uri, string>
              {
                  [moduleBUri] = files[moduleBUri],
                  [moduleCUri] = files[moduleCUri]
              },
              moduleBUri), moduleBTemplateHash);

            ModuleTemplateHashValidator(
              Services.BuildCompilation(new Dictionary<Uri, string>
              {
                  [moduleCUri] = files[moduleCUri]
              },
              moduleCUri), moduleCTemplateHash);
        }

        [TestMethod]
        public async Task Module_should_include_diagnostic_if_module_file_cannot_be_loaded()
        {
            var mainUri = new Uri("file:///path/to/main.bicep");
            var moduleAUri = new Uri("file:///path/to/modulea.bicep");

            var mainFileContents = @"
param inputa string
param inputb string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}
";
            var fileExplorer = new InMemoryFileExplorer();
            fileExplorer.GetFile(mainUri.ToIOUri()).Write(mainFileContents);

            var compiler = ServiceBuilder.Create(s => s.WithFileExplorer(fileExplorer).WithDisabledAnalyzersConfiguration()).GetCompiler();
            var compilation = await compiler.CreateCompilation(mainUri.ToIOUri());

            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP091", DiagnosticLevel.Error, "An error occurred reading file. File '/path/to/modulea.bicep' does not exist."),
            });
        }

        [TestMethod]
        public void External_module_reference_with_unknown_scheme_should_be_rejected()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new(TestContext, RegistryEnabled: true));
            var result = CompilationHelper.Compile(services, @"module test 'fake:totally-fake' = {}");

            result.Should().HaveDiagnostics(new[]
            {
                ("BCP189", DiagnosticLevel.Error, "The specified module reference scheme \"fake\" is not recognized. Specify a path to a local module file or a module reference using one of the following schemes: \"br\", \"ts\"")
            });
        }

        [TestMethod]
        public void Module_cannot_use_resource_typed_parameter_without_feature_enabled()
        {
            var result = CompilationHelper.Compile(
("main.bicep", @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

module mod './module.bicep' = {
    name: 'test'
    params: {
        p: resource
    }
}

"),
("module.bicep", @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'
output out string = p.properties.accessTier

"));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP104", DiagnosticLevel.Error, "The referenced module has errors."),
                ("BCP036", DiagnosticLevel.Error, "The property \"p\" expected a value of type \"error\" but the provided value is of type \"Microsoft.Storage/storageAccounts@2019-06-01\".")
            });
        }

        [TestMethod]
        public void Module_cannot_use_resource_typed_output_without_feature_enabled()
        {
            var result = CompilationHelper.Compile(
("main.bicep", @"
module mod './module.bicep' = {
    name: 'test'
}

"),
("module.bicep", @"
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

output storage resource = storage
"));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP104", DiagnosticLevel.Error, "The referenced module has errors.")
            });
        }

        [TestMethod]
        public void Module_can_pass_correct_resource_type_as_parameter()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped,
("main.bicep", @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

module mod './module.bicep' = {
    name: 'test'
    params: {
        p: resource
    }
}

"),
("module.bicep", @"
param p resource 'Microsoft.Storage/storageAccounts@2019-06-01'
output out string = p.properties.accessTier

"));
            result.Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            result.Template.Should().HaveValueAtPath("$.resources[0].properties.parameters.p.value", "[resourceId('Microsoft.Storage/storageAccounts', 'test')]");
        }

        [TestMethod]
        public void Module_can_pass_correct_resource_type_with_different_api_version_as_parameter()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped,
("main.bicep", @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

module mod './module.bicep' = {
    name: 'test'
    params: {
        p: resource
    }
}

"),
("module.bicep", @"
param p resource 'Microsoft.Storage/storageAccounts@2021-04-01'
output out string = p.properties.accessTier

"));
            result.Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            result.Template.Should().HaveValueAtPath("$.resources[0].properties.parameters.p.value", "[resourceId('Microsoft.Storage/storageAccounts', 'test')]");
        }

        // Regression test for https://github.com/Azure/bicep/issues/6038
        //
        // Object-typed parameters should work the same way regardless of whether resource-typed parameters are enabled.
        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Module_can_pass_resource_body_as_object_typed_parameter(bool enableResourceTypeParameters)
        {
            var services = enableResourceTypeParameters ? ServicesWithResourceTyped : new ServiceBuilder();
            var result = CompilationHelper.Compile(
                services,
("main.bicep", @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

module mod './module.bicep' = {
    name: 'test'
    params: {
        p: resource
    }
}

"),
("module.bicep", @"
param p object
output out string = p.properties.accessTier

"));
            result.Should().NotHaveAnyDiagnostics();

            var model = result.Compilation.GetEntrypointSemanticModel();
            result.Template.Should().HaveValueAtPath("$.resources[0].properties.parameters.p.value", "[reference(resourceId('Microsoft.Storage/storageAccounts', 'test'), '2019-06-01', 'full')]");
        }

        [TestMethod]
        public void Module_with_resource_type_output_can_be_evaluated()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped,
("main.bicep", @"
module mod './module.bicep' = {
    name: 'test'
}

output id string = mod.outputs.storage.id
output name string = mod.outputs.storage.name
output type string = mod.outputs.storage.type
output apiVersion string = mod.outputs.storage.apiVersion

"),
("module.bicep", @"
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

output storage resource = storage
"));
            result.Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.id", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[reference(resourceId('Microsoft.Resources/deployments', 'test'), '2025-04-01').outputs.storage.value]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.name", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[last(split(reference(resourceId('Microsoft.Resources/deployments', 'test'), '2025-04-01').outputs.storage.value, '/'))]"),
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
        }

        [TestMethod]
        public void Module_array_with_resource_type_output_can_be_evaluated()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped,
("main.bicep", @"
module mod './module.bicep' = [for (item, i) in []:  {
    name: 'test-${i}'
}]

output id string = mod[0].outputs.storage.id
output name string = mod[0].outputs.storage.name
output type string = mod[0].outputs.storage.type
output apiVersion string = mod[0].outputs.storage.apiVersion

"),
("module.bicep", @"
resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

output storage resource = storage
"));
            result.Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.id", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[reference(resourceId('Microsoft.Resources/deployments', format('test-{0}', 0)), '2025-04-01').outputs.storage.value]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.name", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[last(split(reference(resourceId('Microsoft.Resources/deployments', format('test-{0}', 0)), '2025-04-01').outputs.storage.value, '/'))]"),
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
        }

        [TestMethod]
        public void Module_with_unknown_resourcetype_as_parameter_and_output_has_diagnostics()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped,
("main.bicep", @"
module mod './module.bicep' = {
    name: 'test'
    params: {
        p: 'something'
    }
}

output foo resource = mod.outputs.storage

"),
("module.bicep", @"
param p resource 'Some.Fake/Type@2019-06-01'

resource fake 'Another.Fake/Type@2019-06-01' = {
  name: 'test'
  properties: {
      value: p.properties.value
  }
}

output storage resource 'Another.Fake/Type@2019-06-01' = fake
"));
            var diagnosticsMap = result.Compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
            using (new AssertionScope())
            {
                diagnosticsMap[InMemoryFileResolver.GetFileUri("/path/to/module.bicep")].Should().HaveDiagnostics(new[]
                {
                    ("BCP081", DiagnosticLevel.Warning, "Resource type \"Some.Fake/Type@2019-06-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                    ("BCP081", DiagnosticLevel.Warning, "Resource type \"Another.Fake/Type@2019-06-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                    ("BCP081", DiagnosticLevel.Warning, "Resource type \"Another.Fake/Type@2019-06-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                });
                diagnosticsMap[InMemoryFileResolver.GetFileUri("/path/to/main.bicep")].Should().HaveDiagnostics(new[]
                {
                    ("BCP230", DiagnosticLevel.Warning, "The referenced module uses resource type \"Some.Fake/Type@2019-06-01\" which does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                    ("BCP230", DiagnosticLevel.Warning, "The referenced module uses resource type \"Another.Fake/Type@2019-06-01\" which does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                    ("BCP036", DiagnosticLevel.Error, "The property \"p\" expected a value of type \"Some.Fake/Type\" but the provided value is of type \"'something'\"."),
                });
            }
        }

        [TestMethod]
        public void Module_cannot_pass_incorrect_resource_type_as_parameter()
        {
            var result = CompilationHelper.Compile(
                ServicesWithResourceTyped,
("main.bicep", @"
resource resource 'Microsoft.Storage/storageAccounts@2019-06-01' existing = {
  name: 'test'
}

module mod './module.bicep' = {
    name: 'test'
    params: {
        p: resource
    }
}

"),
("module.bicep", @"
param p resource 'Microsoft.Sql/servers@2021-02-01-preview'
output out string = p.properties.minimalTlsVersion

"));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP036", DiagnosticLevel.Error, "The property \"p\" expected a value of type \"Microsoft.Sql/servers\" but the provided value is of type \"Microsoft.Storage/storageAccounts@2019-06-01\"."),
            });
        }

        [TestMethod]
        public void Module_cannot_reference_bicep_params_file()
        {
            var result = CompilationHelper.Compile(
                Services,
("main.bicep", @"
module mod './foo.bicepparam' = {
  name: 'hello'
}
"),
("foo.bicepparam", @""));
            result.Should().OnlyContainDiagnostic("BCP277", DiagnosticLevel.Error, "A module declaration can only reference a Bicep File, an ARM template, a registry reference or a template spec reference.");
        }

        [DataRow("a", "a")]
        [DataRow("hello", "hello")]
        [DataRow("this______has_________fifty_____________characters", "this______has_________fifty_____________characters")]
        [DataRow("this______has_________fifty_one__________characters", "this______has_________fifty_one__________character")]
        [DataRow("module_symbolic_name_with_a_super_long_name_that_has_seventy_seven_characters", "module_symbolic_name_with_a_super_long_name_that_h")]
        [DataTestMethod]
        public void Module_name_is_generated_correctly_when_optional_module_names_enabled(string symbolicName, string symbolicNamePrefix)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var result = CompilationHelper.Compile(
                services,
("main.bicep", $@"
module {symbolicName} 'mod.bicep' = {{}}
"),
("mod.bicep", string.Empty));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath($"$.resources.{symbolicName}.name", $"[format('{symbolicNamePrefix}-{{0}}', uniqueString('{symbolicName}', deployment().name))]");
        }

        [DataRow("a", "a")]
        [DataRow("hello", "hello")]
        [DataRow("this______has_______forty_six_______characters", "this______has_______forty_six_______characters")]
        [DataRow("this______has_______forty_seven______characters", "this______has_______forty_seven______character")]
        [DataRow("module_symbolic_name_with_a_super_long_name_that_has_seventy_seven_characters", "module_symbolic_name_with_a_super_long_name_th")]
        [DataTestMethod]
        public void Module_collection_name_is_generated_correctly_when_optional_module_names_enabled(string symbolicName, string symbolicNamePrefix)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var result = CompilationHelper.Compile(
                services,
("main.bicep", $@"
module {symbolicName} 'mod.bicep' = [for x in []: {{
}}]
"),
("mod.bicep", string.Empty));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath($"$.resources.{symbolicName}.name", $"[format('{symbolicNamePrefix}-{{0}}-{{1}}', copyIndex(), uniqueString('{symbolicName}', deployment().name))]");
        }

        [DataRow("a", "a")]
        [DataRow("hello", "hello")]
        [DataRow("this______has_________fifty_____________characters", "this______has_________fifty_____________characters")]
        [DataRow("this______has_________fifty_one__________characters", "this______has_________fifty_one__________character")]
        [DataRow("module_symbolic_name_with_a_super_long_name_that_has_seventy_seven_characters", "module_symbolic_name_with_a_super_long_name_that_h")]
        [DataTestMethod]
        public void Module_with_generated_name_can_be_referenced_correctly(string symbolicName, string symbolicNamePrefix)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var result = CompilationHelper.Compile(services,
                ("main.bicep", $$"""
                    module {{symbolicName}} 'mod1.bicep' = {}

                    output name string = {{symbolicName}}.name
                    """),
                ("mod1.bicep", ""));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.name.value", $"[format('{symbolicNamePrefix}-{{0}}', uniqueString('{symbolicName}', deployment().name))]");
        }

        [DataRow("a", "a")]
        [DataRow("hello", "hello")]
        [DataRow("this______has_______forty_six_______characters", "this______has_______forty_six_______characters")]
        [DataRow("this______has_______forty_seven______characters", "this______has_______forty_seven______character")]
        [DataRow("module_symbolic_name_with_a_super_long_name_that_has_seventy_seven_characters", "module_symbolic_name_with_a_super_long_name_th")]
        [DataTestMethod]
        public void Module_collection_with_generated_name_can_be_referenced_correctly(string symbolicName, string symbolicNamePrefix)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var result = CompilationHelper.Compile(services,
                ("main.bicep", $$"""
                    module {{symbolicName}} 'mod.bicep' = [for _ in range(0, 10): {}]

                    output firstModName string = {{symbolicName}}[0].name
                    output allModNames string[] = [for i in range(0, 10): {{symbolicName}}[i].name]
                    """),
                ("mod.bicep", ""));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.outputs.firstModName.value", $"[format('{symbolicNamePrefix}-{{0}}-{{1}}', 0, uniqueString('{symbolicName}', deployment().name))]");
            result.Template.Should().HaveValueAtPath("$.outputs.firstModName.value", $"[format('{symbolicNamePrefix}-{{0}}-{{1}}', 0, uniqueString('{symbolicName}', deployment().name))]");
            result.Template.Should().HaveValueAtPath("$.outputs.allModNames.copy.input", $"[format('{symbolicNamePrefix}-{{0}}-{{1}}', range(0, 10)[copyIndex()], uniqueString('{symbolicName}', deployment().name))]");
        }

        [TestMethod]
        public void Module_can_be_compiled_with_identity_with_param_and_function_successfully()
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var moduleBUri = new Uri("file:///moduleb.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = """
param inputa string
param inputb string
param identityId string

module modulea 'modulea.bicep' = {
  name: 'modulea'
  params: {
    inputa: inputa
    inputb: inputb
  }
}

func test () string => 'val'

module moduleb 'moduleb.bicep' = {
  name: 'moduleb'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identityId}': {}
      '${test()}': {}
      id: {}
    }
  }
  params: {
    inputa: inputa
    inputb: inputb
  }
}

module modulec 'moduleb.bicep' = {
  name: 'modulec'
  identity: {
    type: 'None'
  }
  params: {
    inputa: inputa
    inputb: inputb
  }
}

""",
                [moduleAUri] = """
param inputa string
param inputb string

output outputa string = '${inputa}-${inputb}'
""",
                [moduleBUri] = """
param inputa string
param inputb string

output outputb string = '${inputa}-${inputb}'
"""
            };

            var compilation = ServicesWithExtensionConfigs.BuildCompilation(files, mainUri);

            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            compilation.GetTestTemplate().Should().NotBeEmpty();
            var templateString = compilation.GetTestTemplate();
            templateString.Should().NotBeNull();
            var template = JToken.Parse(templateString);
            template.Should().NotBeNull();
            var test = template.SelectToken("resources.moduleb.identity")?.ToString();
            template.Should().HaveValueAtPath("$.resources.moduleb.identity", new JObject()
            {
                ["type"] = new JValue("UserAssigned"),
                ["userAssignedIdentities"] = new JObject()
                {
                    ["[format('{0}', parameters('identityId'))]"] = new JObject(),
                    ["[format('{0}', __bicep.test())]"] = new JObject(),
                    ["id"] = new JObject(),
                },
            });
            template.Should().HaveValueAtPath("$.resources.modulec.identity", new JObject()
            {
                ["type"] = new JValue("None"),
            });
            template.Should().HaveValueAtPath("$.resources.modulec.apiVersion", new JValue("2025-04-01"));
        }

        [TestMethod]
        public void Module_can_be_compiled_with_identity_successfully()
        {
            var result = CompilationHelper.Compile(
                ServicesWithExtensionConfigs,
("main.bicep", """
module mod './module.bicep' = {
    identity: {
        type: 'UserAssigned'
        userAssignedIdentities: {
            identityId: {}
        }
    }
    name: 'test'
    params: {
        keyVaultUri: 'keyVaultUri'
        identityId: 'identityId'
    }
}

"""),
("module.bicep", """
param keyVaultUri string
param identityId string

output out string = '${keyVaultUri}-${identityId}'
"""));
            result.Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.resources.mod.identity", new JObject()
            {
                ["type"] = new JValue("UserAssigned"),
                ["userAssignedIdentities"] = new JObject()
                {
                    ["identityId"] = new JObject(),
                },
            });
            result.Template.Should().HaveValueAtPath("$.resources.mod.apiVersion", new JValue("2025-04-01"));
        }

        [TestMethod]
        public void Module_can_be_compiled_with_identity_successfully_with_object_param()
        {
            var result = CompilationHelper.Compile(
                ServicesWithExtensionConfigs,
("main.bicep", """
param identity object
module mod './module.bicep' = {
    identity: identity
    name: 'test'
    params: {
        keyVaultUri: 'keyVaultUri'
        identityId: 'identityId'
    }
}

"""),
("module.bicep", """
param keyVaultUri string
param identityId string

output out string = '${keyVaultUri}-${identityId}'
"""));
            result.Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.resources.mod.identity", new JValue("[parameters('identity')]"));

            result.Template.Should().HaveValueAtPath("$.resources.mod.apiVersion", new JValue("2025-04-01"));
        }

        [TestMethod]
        public void Module_incorrect_identity_raises_diagnostics()
        {
            var result = CompilationHelper.Compile(
                ServicesWithExtensionConfigs,
("main.bicep", """
module mod './module.bicep' = {
    identity: {
        userAssignedIdentities: [
            'val'
        ]
        additionalProperty: {}
    }
    name: 'test'
    params: {
        keyVaultUri: 'keyVaultUri'
        identityId: 'identityId'
    }
}

"""),
("module.bicep", """
param keyVaultUri string
param identityId string

output out string = '${keyVaultUri}-${identityId}'
"""));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"type\"."),
                ("BCP036", DiagnosticLevel.Error, "The property \"userAssignedIdentities\" expected a value of type \"object\" but the provided value is of type \"['val']\"."),
                ("BCP037", DiagnosticLevel.Error, "The property \"additionalProperty\" is not allowed on objects of type \"identity\". Permissible properties include \"type\".")
            });
        }

        [TestMethod]
        public void Module_invalid_identity_type_raises_diagnostics()
        {
            var result = CompilationHelper.Compile(
                ServicesWithExtensionConfigs,
("main.bicep", """
module mod './module.bicep' = {
    identity: {
        type: 'SystemAssigned'
    }
    name: 'test'
    params: {
        keyVaultUri: 'keyVaultUri'
        identityId: 'identityId'
    }
}

"""),
("module.bicep", """
param keyVaultUri string
param identityId string

output out string = '${keyVaultUri}-${identityId}'
"""));
            result.Should().HaveDiagnostics(new[]
            {
                ("BCP088", DiagnosticLevel.Error, "The property \"type\" expected a value of type \"'None' | 'UserAssigned'\" but the provided value is of type \"'SystemAssigned'\". Did you mean \"'UserAssigned'\"?")
            });
        }

        [TestMethod]
        public void Module_with_identity_runtime_value_raises_diagnostic()
        {
            var result = CompilationHelper.Compile(
                ServicesWithExtensionConfigs,
("main.bicep", """
module outputModule './modoutput.bicep' = {
    name: 'modWithOutput'
}

module mod './module.bicep' = {
    identity: {
        type: 'UserAssigned'
        userAssignedIdentities: {
            '${outputModule.outputs.runtimevalue}': {}
        }
    }
    name: 'test'
    params: {
        keyVaultUri: 'keyVaultUri'
        identityId: 'identityId'
    }
}

"""),
("module.bicep", """
param keyVaultUri string
param identityId string

output out string = '${keyVaultUri}-${identityId}'
"""),
("modoutput.bicep", """
output runtimevalue string = 'runtime'
"""));
            result.Should().HaveDiagnostics(new[]
           {
                ("BCP120", DiagnosticLevel.Error, "This expression is being used in an assignment to the \"identity\" property of the \"module\" type, which requires a value that can be calculated at the start of the deployment. Properties of outputModule which can be calculated at the start include \"name\".")
            });
        }

        private static void ModuleTemplateHashValidator(Compilation compilation, string expectedTemplateHash)
        {
            var (success, diagnosticsByFile) = compilation.GetSuccessAndDiagnosticsByBicepFile();
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            var templateString = compilation.GetTestTemplate();
            var template = JToken.Parse(templateString);
            template.Should().NotBeNull();
            template.SelectToken(BicepTestConstants.GeneratorTemplateHashPath)?.ToString().Should().Be(expectedTemplateHash);
        }
    }
}
