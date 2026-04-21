// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Bicep.Types.Az;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExtensionImportTests : TestBase
    {
        private static readonly Lazy<IResourceTypeLoader> azTypeLoaderLazy = new(() => new AzResourceTypeLoader(new AzTypeLoader()));

        private async Task<ServiceBuilder> GetServices()
        {
            var fileSystem = new Dictionary<string, string>
            {
                ["/types/index.json"] = """{"resources": {}, "resourceFunctions": {}, "namespaceFunctions": []}""",
            };

            var services = new ServiceBuilder()
                .WithContainerRegistryClientFactory(RegistryHelper.CreateOciClientForAzExtension())
                .WithMockFileSystem(fileSystem)
                .WithAzResourceTypeLoader(azTypeLoaderLazy.Value);

            await RegistryHelper.PublishAzExtension(services.Build(), "/types/index.json");

            return services;
        }

        [TestMethod]
        public async Task Extension_Statement_Without_Specification_String_Should_Emit_Diagnostic()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
extension
");
            result.Should().HaveDiagnostics([
                ("BCP201", DiagnosticLevel.Error, """Expected an extension specification string. This should either be a relative path, or a valid OCI artifact specification."""),
            ]);
        }

        [TestMethod]
        public async Task Extension_Statement_With_Invalid_Keyword_Should_Emit_Diagnostic()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            extension sys blahblah
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP305", DiagnosticLevel.Error, "Expected the \"with\" keyword, \"as\" keyword, or a new line character at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Brace_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            extension kubernetes with
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"{\" character at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_As_Keyword_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            extension kubernetes with {
                kubeConfig: 'foo'
                namespace: 'bar'
            } something
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP305", DiagnosticLevel.Error, """Expected the "with" keyword, "as" keyword, or a new line character at this location."""),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Alias_Name_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            extension kubernetes with {
                kubeConfig: 'foo'
                namespace: 'bar'
            } as
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected an extension alias name at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Alias_Name_For_Sys_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            extension sys as
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected an extension alias name at this location."),
            });
        }

        [TestMethod]
        public async Task Import_configuration_is_blocked_by_default()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            extension az with {
              foo: 'bar'
            }
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Extension \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public async Task Imports_return_error_with_unrecognized_namespace()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), @"
extension madeUpNamespace
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Extension \"madeUpNamespace\" is not recognized."),
            });
        }

        [TestMethod]
        public async Task Using_import_statements_frees_up_the_namespace_symbol()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            extension az as newAz

            var az = 'Fake AZ!'
            var myRg = newAz.resourceGroup()

            output az string = az
            output rgLocation string = myRg.location
            """);

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task You_can_swap_imported_namespaces_if_you_really_really_want_to()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            extension az as sys
            extension sys as az

            var myRg = sys.resourceGroup()

            @az.description('why on earth would you do this?')
            output rgLocation string = myRg.location
            """);

            result.Should().GenerateATemplate();
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs.rgLocation.metadata.description", "why on earth would you do this?");
        }

        [TestMethod]
        public async Task Overwriting_single_built_in_namespace_with_import_is_prohibited()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            extension az as sys

            var myRg = sys.resourceGroup()

            output rgLocation string = myRg.location
            """);

            result.Should().ContainDiagnostic("BCP084", DiagnosticLevel.Error, "The symbolic name \"sys\" is reserved. Please use a different symbolic name. Reserved namespaces are \"sys\".");
        }

        [TestMethod]
        public async Task Singleton_imports_cannot_be_used_multiple_times()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            extension az as az1
            extension az as az2

            extension sys as sys1
            extension sys as sys2
            """);

            result.Should().HaveDiagnostics(new[] {
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is declared multiple times. Remove the duplicates."),
            });
        }

        [TestMethod]
        public async Task Import_names_must_not_conflict_with_other_symbols()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            extension az
            extension kubernetes with {
            kubeConfig: ''
            namespace: ''
            } as az
            """);

            result.Should().HaveDiagnostics(new[] {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"az\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"az\" is declared multiple times. Remove or rename the duplicates."),
            });
        }

        [TestMethod]
        public async Task Ambiguous_function_references_must_be_qualified()
        {
            var ns1 = new NamespaceType(
                "ns1",
                new NamespaceSettings(
                    IsSingleton: true,
                    BicepExtensionName: "ns1",
                    ConfigurationType: null,
                    TemplateExtensionName: "Ns1-Unused",
                    TemplateExtensionVersion: "1.0"),
                ImmutableArray<NamedTypeProperty>.Empty,
                new[] {
                    new FunctionOverloadBuilder("ns1Func").Build(),
                    new FunctionOverloadBuilder("dupeFunc").Build(),
                },
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());

            var ns2 = new NamespaceType(
                "ns2",
                new NamespaceSettings(
                    IsSingleton: true,
                    BicepExtensionName: "ns2",
                    ConfigurationType: null,
                    TemplateExtensionName: "Ns2-Unused",
                    TemplateExtensionVersion: "1.0"),
                ImmutableArray<NamedTypeProperty>.Empty,
                new[] {
                    new FunctionOverloadBuilder("ns2Func").Build(),
                    new FunctionOverloadBuilder("dupeFunc").Build(),
                },
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());

            var nsProvider = TestExtensionsNamespaceProvider.Create((extensionName, aliasName) => extensionName switch
            {
                "ns1" => ns1,
                "ns2" => ns2,
                _ => null,
            });

            var services = (await GetServices())
                .WithNamespaceProvider(nsProvider)
                .WithConfigurationPatch(c => c.WithExtensions("""
                {
                  "az": "builtin:",
                  "ns1": "builtin:",
                  "ns2": "builtin:"
                }
                """));

            var result = await CompilationHelper.RestoreAndCompile(services, @"
            extension ns1
            extension ns2

            output ambiguousResult string = dupeFunc()
            output ns1Result string = ns1Func()
            output ns2Result string = ns2Func()
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP056", DiagnosticLevel.Error, "The reference to name \"dupeFunc\" is ambiguous because it exists in namespaces \"ns1\", \"ns2\". The reference must be fully-qualified."),
            });

            // fix by fully-qualifying
            result = await CompilationHelper.RestoreAndCompile(services, @"
            extension ns1
            extension ns2

            output ambiguousResult string = ns1.dupeFunc()
            output ns1Result string = ns1Func()
            output ns2Result string = ns2Func()
            ");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task Config_with_optional_properties_can_be_skipped()
        {
            var mockNs = new NamespaceType(
                "mockNs",
                new(
                    IsSingleton: false,
                    BicepExtensionName: "mockNs",
                    ConfigurationType: new ObjectType(
                        "mockNs",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new NamedTypeProperty("optionalConfig", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant),
                        },
                        null),
                    TemplateExtensionName: "Unused",
                    TemplateExtensionVersion: "1.0.0"),
                ImmutableArray<NamedTypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());

            var nsProvider = TestExtensionsNamespaceProvider.Create((extensionName, aliasName) => extensionName switch
            {
                "mockNs" => mockNs,
                _ => null,
            });

            var services = (await GetServices())
                .WithNamespaceProvider(nsProvider)
                .WithConfigurationPatch(c => c.WithExtensions("""
                {
                  "az": "builtin:",
                  "mockNs": "builtin:"
                }
                """));

            var result = await CompilationHelper.RestoreAndCompile(services, """
            extension mockNs with {
              optionalConfig: 'blah blah'
            } as ns1
            extension mockNs
            """);

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task MicrosoftGraph_imports_succeed_default()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), @"extension microsoftGraph as graph");

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP407", DiagnosticLevel.Error, """Built-in extension "microsoftGraph" is retired. Use dynamic types instead. See https://aka.ms/graphbicep/dynamictypes"""),
            });
        }
    }
}
