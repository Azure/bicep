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
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ProviderImportTests : TestBase
    {
        private static readonly Lazy<IResourceTypeLoader> azTypeLoaderLazy = new(() => new AzResourceTypeLoader(new AzTypeLoader()));

        private async Task<ServiceBuilder> GetServices()
        {
            var fileSystem = new Dictionary<string, string>
            {
                ["/types/index.json"] = """{"resources": {}, "resourceFunctions": {}}""",
            };

            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true))
                .WithContainerRegistryClientFactory(RegistryHelper.CreateOciClientForAzProvider())
                .WithMockFileSystem(fileSystem)
                .WithAzResourceTypeLoader(azTypeLoaderLazy.Value);

            await RegistryHelper.PublishAzProvider(services.Build(), "/types/index.json");

            return services;
        }

        [TestMethod]
        public async Task Providers_are_disabled_unless_feature_is_enabled()
        {
            var services = new ServiceBuilder();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            provider az
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Using provider statements requires enabling EXPERIMENTAL feature \"Extensibility\"."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Specification_String_Should_Emit_Diagnostic()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
provider
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP201", DiagnosticLevel.Error, """
                Expected a provider specification string of with a valid format at this location. Valid formats:
                * "br:<providerRegistryHost>/<providerRepositoryPath>@<providerVersion>"
                * "br/<providerAlias>:<providerName>@<providerVersion>"
                """)
            });
        }

        [TestMethod]
        public async Task Provider_Statement_With_Invalid_Keyword_Should_Emit_Diagnostic()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            provider sys blahblah
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
            provider kubernetes with
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
            provider kubernetes with {
                kubeConfig: 'foo'
                namespace: 'bar'
            } something
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP012", DiagnosticLevel.Error, "Expected the \"as\" keyword at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Alias_Name_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            provider kubernetes with {
                kubeConfig: 'foo'
                namespace: 'bar'
            } as
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected a provider alias name at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Alias_Name_For_Sys_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, """
            provider sys as
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected a provider alias name at this location."),
            });
        }

        [TestMethod]
        public async Task Using_import_instead_of_provider_raises_warning()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            import 'az@1.0.0' as foo
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP381", DiagnosticLevel.Warning, "Declaring provider namespaces with the \"import\" keyword has been deprecated. Please use the \"provider\" keyword instead."),
                ("BCP395", DiagnosticLevel.Warning, "Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead."),
            });
        }

        [TestMethod]
        public async Task Using_legacy_import_syntax_raises_warning_for_az_provider()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), $"""
            provider 'az@1.0.0' as az
            """);

            result.Should().HaveDiagnostics(new[] {
                ("BCP395", DiagnosticLevel.Warning, "Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead."),
            });
        }

        [TestMethod]
        public async Task Import_configuration_is_blocked_by_default()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            provider az with {
              foo: 'bar'
            }
            """);
            result.Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Provider namespace \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public async Task Imports_return_error_with_unrecognized_namespace()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), @"
provider madeUpNamespace
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"madeUpNamespace\" is not recognized."),
            });
        }

        [TestMethod]
        public async Task Using_import_statements_frees_up_the_namespace_symbol()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            provider az as newAz

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
            provider az as sys
            provider sys as az

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
            provider az as sys

            var myRg = sys.resourceGroup()

            output rgLocation string = myRg.location
            """);

            result.Should().ContainDiagnostic("BCP084", DiagnosticLevel.Error, "The symbolic name \"sys\" is reserved. Please use a different symbolic name. Reserved namespaces are \"sys\".");
        }

        [TestMethod]
        public async Task Singleton_imports_cannot_be_used_multiple_times()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), """
            provider az as az1
            provider az as az2

            provider sys as sys1
            provider sys as sys2
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
            provider az
            provider kubernetes with {
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
                    BicepProviderName: "ns1",
                    ConfigurationType: null,
                    ArmTemplateProviderName: "Ns1-Unused",
                    ArmTemplateProviderVersion: "1.0"),
                ImmutableArray<TypeProperty>.Empty,
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
                    BicepProviderName: "ns2",
                    ConfigurationType: null,
                    ArmTemplateProviderName: "Ns2-Unused",
                    ArmTemplateProviderVersion: "1.0"),
                ImmutableArray<TypeProperty>.Empty,
                new[] {
                    new FunctionOverloadBuilder("ns2Func").Build(),
                    new FunctionOverloadBuilder("dupeFunc").Build(),
                },
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());

            var nsProvider = TestExtensibilityNamespaceProvider.Create(result => result switch
            {
                { ProviderName: "ns1" } => result with { Type = ns1 },
                { ProviderName: "ns2" } => result with { Type = ns2 },
                _ => result,
            });

            var services = (await GetServices())
                .WithNamespaceProvider(nsProvider)
                .WithConfigurationPatch(c => c.WithProvidersConfiguration("""
                {
                  "az": "builtin:",
                  "ns1": "builtin:",
                  "ns2": "builtin:"
                }
                """));

            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider ns1
            provider ns2

            output ambiguousResult string = dupeFunc()
            output ns1Result string = ns1Func()
            output ns2Result string = ns2Func()
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP056", DiagnosticLevel.Error, "The reference to name \"dupeFunc\" is ambiguous because it exists in namespaces \"ns1\", \"ns2\". The reference must be fully-qualified."),
            });

            // fix by fully-qualifying
            result = await CompilationHelper.RestoreAndCompile(services, @"
            provider ns1
            provider ns2

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
                    BicepProviderName: "mockNs",
                    ConfigurationType: new ObjectType(
                        "mockNs",
                        TypeSymbolValidationFlags.Default,
                        new[]
                        {
                            new TypeProperty("optionalConfig", LanguageConstants.String, TypePropertyFlags.DeployTimeConstant),
                        },
                        null),
                    ArmTemplateProviderName: "Unused",
                    ArmTemplateProviderVersion: "1.0.0"),
                ImmutableArray<TypeProperty>.Empty,
                ImmutableArray<FunctionOverload>.Empty,
                ImmutableArray<BannedFunction>.Empty,
                ImmutableArray<Decorator>.Empty,
                new EmptyResourceTypeProvider());

            var nsProvider = TestExtensibilityNamespaceProvider.Create(result => result switch
            {
                { ProviderName: "mockNs" } => result with { Type = mockNs },
                _ => result,
            });

            var services = (await GetServices())
                .WithNamespaceProvider(nsProvider)
                .WithConfigurationPatch(c => c.WithProvidersConfiguration("""
                {
                  "az": "builtin:",
                  "mockNs": "builtin:"
                }
                """));

            var result = await CompilationHelper.RestoreAndCompile(services, """
            provider mockNs with {
              optionalConfig: 'blah blah'
            } as ns1
            provider mockNs
            """);

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task MicrosoftGraph_imports_succeed_with_preview_feature_enabled()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), @"provider microsoftGraph as graph");

            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"microsoftGraph\" is not recognized."),
            });

            var serviceWithPreview = new ServiceBuilder()
                .WithFeatureOverrides(new(TestContext, ExtensibilityEnabled: true, MicrosoftGraphPreviewEnabled: true));

            result = await CompilationHelper.RestoreAndCompile(serviceWithPreview, @"provider microsoftGraph as graph");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}
