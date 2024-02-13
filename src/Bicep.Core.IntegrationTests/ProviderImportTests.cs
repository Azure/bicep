// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Bicep.Types.Az;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Samples;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                ["/types/index.json"] = """{"Resources": {}, "Functions": {}}""",
            };

            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ExtensibilityEnabled: true))
                .WithContainerRegistryClientFactory(RegistryHelper.CreateOciClientForAzProvider())
                .WithMockFileSystem(fileSystem)
                .WithAzResourceTypeLoader(azTypeLoaderLazy.Value);

            await RegistryHelper.PublishAzProvider(services.Build(), "/types/index.json");

            return services;
        }

        private class TestNamespaceProvider : INamespaceProvider
        {
            private readonly ImmutableDictionary<string, Func<string, NamespaceType>> builderDict;

            private readonly HashSet<string> builtInNamespacesNames = new(){
                SystemNamespaceType.BuiltInName,
                AzNamespaceType.BuiltInName,
                MicrosoftGraphNamespaceType.BuiltInName,
                K8sNamespaceType.BuiltInName,
            };

            public TestNamespaceProvider(Dictionary<string, Func<string, NamespaceType>> builderDict)
            {
                this.builderDict = builderDict.ToImmutableDictionary();
            }

            public ResultWithDiagnostic<NamespaceType> TryGetNamespace(ResourceTypesProviderDescriptor providerDescriptor, ResourceScope resourceScope, IFeatureProvider features, BicepSourceFileKind sourceFileKind)
            {
                if (builtInNamespacesNames.Contains(providerDescriptor.Name))
                {
                    return new(TestTypeHelper.GetBuiltInNamespaceType(providerDescriptor));
                }
                var namespaceBuilderFn = builderDict[providerDescriptor.Name];
                return new(namespaceBuilderFn(providerDescriptor.Alias));
            }
        }

        [TestMethod]
        public async Task Imports_are_disabled_unless_feature_is_enabled()
        {
            var services = new ServiceBuilder();
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}'
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Using provider statements requires enabling EXPERIMENTAL feature \"Extensibility\"."),
                // BCP084 is raised because BCP203 prevented the compiler from binding a namespace to the `az` symbol (an ErrorType was bound instead).
                ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\"."),
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
                ("BCP201", DiagnosticLevel.Error, "Expected a provider specification string of format \"<providerName>@<providerVersion>\" at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_With_Invalid_Keyword_Should_Emit_Diagnostic()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'sys@1.0.0' blahblah
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP305", DiagnosticLevel.Error, "Expected the \"with\" keyword, \"as\" keyword, or a new line character at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Brace_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'kubernetes@1.0.0' with
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"{\" character at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_As_Keyword_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'kubernetes@1.0.0' with {
            kubeConfig: 'foo'
            namespace: 'bar'
            } something
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP012", DiagnosticLevel.Error, "Expected the \"as\" keyword at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Alias_Name_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'kubernetes@1.0.0' with {
            kubeConfig: 'foo'
            namespace: 'bar'
            } as
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected a provider alias name at this location."),
            });
        }

        [TestMethod]
        public async Task Provider_Statement_Without_Alias_Name_For_Sys_Should_Raise_Error()
        {
            var services = await GetServices();
            var result = await CompilationHelper.RestoreAndCompile(services, @"
            provider 'sys@1.0.0' as
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected a provider alias name at this location."),
            });
        }

        [TestMethod]
        public async Task Using_import_instead_of_provider_raises_warning()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            import 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as foo
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP381", DiagnosticLevel.Warning, "Declaring provider namespaces with the \"import\" keyword has been deprecated. Please use the \"provider\" keyword instead."),
            });
        }

        [DataRow("az")]
        [DataRow("sys")]
        [TestMethod]
        public async Task Using_legacy_import_syntax_raises_warning_for_az_provider(string providerName)
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));

            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider '{providerName}@1.0.0' as {providerName}
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP395", DiagnosticLevel.Warning, "Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead."),
            });
        }

        [TestMethod]
        public async Task Import_configuration_is_blocked_by_default()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' with {{
              foo: 'bar'
            }}
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Provider namespace \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public async Task Imports_return_error_with_unrecognized_namespace()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), @"
provider 'madeUpNamespace@1.0.0'
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"madeUpNamespace\" is not recognized."),
            });
        }

        [TestMethod]
        public async Task Using_import_statements_frees_up_the_namespace_symbol()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));

            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as newAz

            var az = 'Fake AZ!'
            var myRg = newAz.resourceGroup()

            output az string = az
            output rgLocation string = myRg.location
            ");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task You_can_swap_imported_namespaces_if_you_really_really_want_to()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as sys
            provider 'sys@1.0.0' as az

            var myRg = sys.resourceGroup()

            @az.description('why on earth would you do this?')
            output rgLocation string = myRg.location
            ");

            result.Should().GenerateATemplate();
            result.Should().HaveDiagnostics(new[]{
                ("BCP395", DiagnosticLevel.Warning,"Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead."),
            });
            result.Template.Should().HaveValueAtPath("$.outputs.rgLocation.metadata.description", "why on earth would you do this?");
        }

        [TestMethod]
        public async Task Overwriting_single_built_in_namespace_with_import_is_prohibited()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as sys

            var myRg = sys.resourceGroup()

            output rgLocation string = myRg.location
            ");

            result.Should().ContainDiagnostic("BCP084", DiagnosticLevel.Error, "The symbolic name \"sys\" is reserved. Please use a different symbolic name. Reserved namespaces are \"sys\".");
        }

        [TestMethod]
        public async Task Singleton_imports_cannot_be_used_multiple_times()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as az1
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}' as az2

            provider 'sys@1.0.0' as sys1
            provider 'sys@1.0.0' as sys2
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is declared multiple times. Remove the duplicates."),
                ("BCP395", DiagnosticLevel.Warning,"Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is declared multiple times. Remove the duplicates."),
                ("BCP395", DiagnosticLevel.Warning,"Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead.")
            });
        }

        [TestMethod]
        public async Task Import_names_must_not_conflict_with_other_symbols()
        {
            var services = await GetServices();
            services = services.WithFeatureOverrides(new(ExtensibilityEnabled: true, DynamicTypeLoadingEnabled: true));
            var result = await CompilationHelper.RestoreAndCompile(services, @$"
            provider 'br/public:az@{BicepTestConstants.BuiltinAzProviderVersion}'
            provider 'kubernetes@1.0.0' with {{
            kubeConfig: ''
            namespace: ''
            }} as az
            ");

            result.Should().HaveDiagnostics(new[] {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"az\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP395", DiagnosticLevel.Warning,"Declaring provider namespaces using the '<providerName>@<version>' expression has been deprecated. Please use an identifier instead."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"az\" is declared multiple times. Remove or rename the duplicates."),
            });
        }

        [TestMethod]
        public async Task Ambiguous_function_references_must_be_qualified()
        {
            var nsProvider = new TestNamespaceProvider(new()
            {
                ["ns1"] = aliasName => new NamespaceType(
                    aliasName,
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
                    new EmptyResourceTypeProvider()),
                ["ns2"] = aliasName => new NamespaceType(
                    aliasName,
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
                    new EmptyResourceTypeProvider()),
            });

            var services = (await GetServices()).WithNamespaceProvider(nsProvider);

            var result = await CompilationHelper.RestoreAndCompile(services, @"
provider 'ns1@1.0.0' as ns1
provider 'ns2@1.0.0' as ns2

output ambiguousResult string = dupeFunc()
output ns1Result string = ns1Func()
output ns2Result string = ns2Func()
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP056", DiagnosticLevel.Error, "The reference to name \"dupeFunc\" is ambiguous because it exists in namespaces \"ns1\", \"ns2\". The reference must be fully-qualified."),
            });

            // fix by fully-qualifying
            result = await CompilationHelper.RestoreAndCompile(services, @"
provider 'ns1@1.0.0' as ns1
provider 'ns2@1.0.0' as ns2

output ambiguousResult string = ns1.dupeFunc()
output ns1Result string = ns1Func()
output ns2Result string = ns2Func()
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task Config_with_optional_properties_can_be_skipped()
        {
            var nsProvider = new TestNamespaceProvider(new()
            {
                ["mockNs"] = aliasName => new NamespaceType(
                    aliasName,
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
                    new EmptyResourceTypeProvider()),
            });

            var services = (await GetServices()).WithNamespaceProvider(nsProvider);

            var result = await CompilationHelper.RestoreAndCompile(services, @"
provider 'mockNs@1.0.0' with {
  optionalConfig: 'blah blah'
} as ns1
provider 'mockNs@1.0.0' as ns2
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public async Task MicrosoftGraph_imports_succeed_with_preview_feature_enabled()
        {
            var result = await CompilationHelper.RestoreAndCompile(await GetServices(), @"provider 'microsoftGraph@1.0.0' as graph");

            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"microsoftGraph\" is not recognized."),
            });

            var serviceWithPreview = new ServiceBuilder()
                .WithFeatureOverrides(new(TestContext, ExtensibilityEnabled: true, MicrosoftGraphPreviewEnabled: true));

            result = await CompilationHelper.RestoreAndCompile(serviceWithPreview, @"provider 'microsoftGraph@1.0.0' as graph");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}
