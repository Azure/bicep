// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.FileSystem;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.FileSystem;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
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
        private static ServiceBuilder Services => new ServiceBuilder().WithEmptyAzResources();

        private static readonly MockRepository Repository = new(MockBehavior.Strict);

        private ServiceBuilder ServicesWithResourceTyped => new ServiceBuilder().WithFeatureOverrides(new(TestContext, ResourceTypedParamsAndOutputsEnabled: true));

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

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            GetTemplate(compilation).Should().NotBeEmpty();
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

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
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

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
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

        private delegate bool TryReadDelegate(Uri fileUri, out string? fileContents, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        private static void SetupFileReaderMock(Mock<IFileResolver> mockFileResolver, Uri fileUri, string? fileContents, DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
        {
            mockFileResolver.Setup(x => x.TryRead(fileUri)).Returns(ResultHelper.Create(fileContents, failureBuilder));
        }

        [TestMethod]
        public void SourceFileGroupingBuilder_build_should_throw_diagnostic_exception_if_entrypoint_file_read_fails()
        {
            var fileUri = new Uri("file:///path/to/main.bicep");

            var mockFileResolver = Repository.Create<IFileResolver>();
            var mockDispatcher = Repository.Create<IModuleDispatcher>().Object;
            var mockConfigurationManager = Repository.Create<IConfigurationManager>().Object;
            SetupFileReaderMock(mockFileResolver, fileUri, null, x => x.ErrorOccurredReadingFile("Mock read failure!"));

            Action buildAction = () => SourceFileGroupingBuilder.Build(mockFileResolver.Object, mockDispatcher, mockConfigurationManager, new Workspace(), fileUri, BicepTestConstants.FeatureProviderFactory);
            buildAction.Should().Throw<ErrorDiagnosticException>()
                .And.Diagnostic.Should().HaveCodeAndSeverity("BCP091", DiagnosticLevel.Error).And.HaveMessage("An error occurred reading file. Mock read failure!");
        }

        [TestMethod]
        public async Task Module_should_include_diagnostic_if_module_file_cannot_be_resolved()
        {
            var mainFileUri = new Uri("file:///path/to/main.bicep");
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

            var mockFileResolver = Repository.Create<IFileResolver>();
            SetupFileReaderMock(mockFileResolver, mainFileUri, mainFileContents, null);
            mockFileResolver.Setup(x => x.TryResolveFilePath(mainFileUri, "modulea.bicep")).Returns((Uri?)null);

            var compiler = ServiceBuilder.Create(s => s.WithFileResolver(mockFileResolver.Object).WithDisabledAnalyzersConfiguration()).GetCompiler();
            var compilation = await compiler.CreateCompilation(mainFileUri);

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile[mainFileUri].Should().HaveDiagnostics(new[] {
                ("BCP093", DiagnosticLevel.Error, "File path \"modulea.bicep\" could not be resolved relative to \"/path/to/main.bicep\"."),
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

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();

            var templateString = GetTemplate(compilation);
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
            var mockFileResolver = Repository.Create<IFileResolver>();
            SetupFileReaderMock(mockFileResolver, mainUri, mainFileContents, null);
            SetupFileReaderMock(mockFileResolver, moduleAUri, null, x => x.ErrorOccurredReadingFile("Mock read failure!"));
            mockFileResolver.Setup(x => x.TryResolveFilePath(mainUri, "modulea.bicep")).Returns(moduleAUri);

            var compiler = ServiceBuilder.Create(s => s.WithFileResolver(mockFileResolver.Object).WithDisabledAnalyzersConfiguration()).GetCompiler();
            var compilation = await compiler.CreateCompilation(mainUri);

            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile[mainUri].Should().HaveDiagnostics(new[] {
                ("BCP091", DiagnosticLevel.Error, "An error occurred reading file. Mock read failure!"),
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
                ["value"] = new JValue("[reference(resourceId('Microsoft.Resources/deployments', 'test'), '2022-09-01').outputs.storage.value]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.name", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[last(split(reference(resourceId('Microsoft.Resources/deployments', 'test'), '2022-09-01').outputs.storage.value, '/'))]"),
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
                ["value"] = new JValue("[reference(resourceId('Microsoft.Resources/deployments', format('test-{0}', 0)), '2022-09-01').outputs.storage.value]"),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.name", new JObject()
            {
                ["type"] = new JValue("string"),
                ["value"] = new JValue("[last(split(reference(resourceId('Microsoft.Resources/deployments', format('test-{0}', 0)), '2022-09-01').outputs.storage.value, '/'))]"),
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
            var diagnosticsMap = result.Compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.FileUri, kvp => kvp.Value);
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
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, OptionalModuleNamesEnabled: true));
            var result = CompilationHelper.Compile(
                services,
("main.bicep", $@"
module {symbolicName} 'mod.bicep' = {{}}
"),
("mod.bicep", string.Empty));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.resources[0].name", $"[format('{symbolicNamePrefix}-{{0}}', uniqueString('{symbolicName}', deployment().name))]");
        }

        [DataRow("a", "a")]
        [DataRow("hello", "hello")]
        [DataRow("this______has_______forty_six_______characters", "this______has_______forty_six_______characters")]
        [DataRow("this______has_______forty_seven______characters", "this______has_______forty_seven______character")]
        [DataRow("module_symbolic_name_with_a_super_long_name_that_has_seventy_seven_characters", "module_symbolic_name_with_a_super_long_name_th")]
        [DataTestMethod]
        public void Module_collection_name_is_generated_correctly_when_optional_module_names_enabled(string symbolicName, string symbolicNamePrefix)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, OptionalModuleNamesEnabled: true));
            var result = CompilationHelper.Compile(
                services,
("main.bicep", $@"
module {symbolicName} 'mod.bicep' = [for x in []: {{
}}]
"),
("mod.bicep", string.Empty));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.resources[0].name", $"[format('{symbolicNamePrefix}-{{0}}-{{1}}', copyIndex(), uniqueString('{symbolicName}', deployment().name))]");
        }

        private static string GetTemplate(Compilation compilation)
        {
            var stringBuilder = new StringBuilder();
            var stringWriter = new StringWriter(stringBuilder);

            var emitter = new TemplateEmitter(compilation.GetEntrypointSemanticModel());
            emitter.Emit(stringWriter);

            return stringBuilder.ToString();
        }

        private static (bool success, IDictionary<Uri, ImmutableArray<IDiagnostic>> diagnosticsByFile) GetSuccessAndDiagnosticsByFile(Compilation compilation)
        {
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.FileUri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => d.Level != DiagnosticLevel.Error);

            return (success, diagnosticsByFile);
        }

        private static void ModuleTemplateHashValidator(Compilation compilation, string expectedTemplateHash)
        {
            var (success, diagnosticsByFile) = GetSuccessAndDiagnosticsByFile(compilation);
            diagnosticsByFile.Values.SelectMany(x => x).Should().BeEmpty();
            success.Should().BeTrue();
            var templateString = GetTemplate(compilation);
            var template = JToken.Parse(templateString);
            template.Should().NotBeNull();
            template.SelectToken(BicepTestConstants.GeneratorTemplateHashPath)?.ToString().Should().Be(expectedTemplateHash);
        }
    }
}
