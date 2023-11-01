// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]

    public class ImportTests
    {
        private ServiceBuilder ServicesWithImports => new ServiceBuilder()
           .WithFeatureOverrides(new(
               CacheRootDirectory: @"C:/.bicep",
               ExtensibilityEnabled: true,
               DynamicTypeLoadingEnabled: true))
            .WithNamespaceProvider(
            new TestNamespaceProvider(new()
            {
                [AzNamespaceType.BuiltInName] = aliasName => AzNamespaceType.Create(
                    aliasName,
                    ResourceScope.ResourceGroup,
                     new AzResourceTypeProvider(),
                     BicepSourceFileKind.BicepFile),
                [K8sNamespaceType.BuiltInName] = K8sNamespaceType.Create
            })
            );

        private class TestNamespaceProvider : INamespaceProvider
        {
            private readonly ImmutableDictionary<string, Func<string, NamespaceType>> builderDict;

            public TestNamespaceProvider(Dictionary<string, Func<string, NamespaceType>> builderDict)
            {
                this.builderDict = builderDict.ToImmutableDictionary();
            }

            public static bool AllowImportStatements => true;

            public IEnumerable<string> AvailableNamespaces => builderDict.Keys.Concat(new[] { SystemNamespaceType.BuiltInName });

            public NamespaceType? TryGetNamespace(
                TypesProviderDescriptor typesProviderDescriptor,
                ResourceScope resourceScope,
                IFeatureProvider features,
                BicepSourceFileKind sourceFileKind)
                    => typesProviderDescriptor.Name switch
                    {
                        SystemNamespaceType.BuiltInName => SystemNamespaceType.Create(typesProviderDescriptor.Alias, features, sourceFileKind),
                        { } _ when builderDict.TryGetValue(typesProviderDescriptor.Name) is { } builderFunc => builderFunc(typesProviderDescriptor.Alias),
                        _ => default,
                    };
        }

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void Imports_are_disabled_unless_feature_is_enabled()
        {
            var result = CompilationHelper.Compile(@"
provider 'br/public:az@0.0.0'
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Using provider statements requires enabling EXPERIMENTAL feature \"Extensibility\"."),
                // BCP084 is raised because BCP203 prevented the compiler from binding a namespace to the `az` symbol (an ErrorType was bound instead).
                ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\"."),
            });
        }

        [TestMethod]
        public void Import_statement_parse_diagnostics_are_guiding()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP201", DiagnosticLevel.Error, "Expected a provider specification string of format \"<providerName>@<providerVersion>\" at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'sys@1.0.0' blahblah
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP305", DiagnosticLevel.Error, "Expected the \"with\" keyword, \"as\" keyword, or a new line character at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'kubernetes@1.0.0' with
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"{\" character at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'kubernetes@1.0.0' with {
  kubeConfig: 'foo'
  namespace: 'bar'
} something
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP012", DiagnosticLevel.Error, "Expected the \"as\" keyword at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'kubernetes@1.0.0' with {
  kubeConfig: 'foo'
  namespace: 'bar'
} as
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected a provider alias name at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'sys@1.0.0' as
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected a provider alias name at this location."),
            });
        }

        [TestMethod]
        public void Using_import_instead_of_provider_raises_warning()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import 'br/public:az@0.0.0' as foo
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP381", DiagnosticLevel.Warning, "Declaring provider namespaces with the \"import\" keyword has been deprecated. Please use the \"provider\" keyword instead."),
            });
        }

        [TestMethod]
        public void Imports_return_error_with_unrecognized_namespace()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'madeUpNamespace@1.0.0'
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"madeUpNamespace\" is not recognized."),
            });
        }

        [TestMethod]
        public void Import_configuration_is_blocked_by_default()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
            provider 'br/public:az@0.0.0' with {
              foo: 'bar'
            }
            ");
            result.Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Provider namespace \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public void Using_import_statements_frees_up_the_namespace_symbol()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'br/public:az@0.0.0' as newAz

var az = 'Fake AZ!'
var myRg = newAz.resourceGroup()

output az string = az
output rgLocation string = myRg.location
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void You_can_swap_imported_namespaces_if_you_really_really_want_to()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'br/public:az@0.0.0' as sys
provider 'sys@1.0.0' as az

var myRg = sys.resourceGroup()

@az.description('why on earth would you do this?')
output rgLocation string = myRg.location
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs.rgLocation.metadata.description", "why on earth would you do this?");
        }

        [TestMethod]
        public void Overwriting_single_built_in_namespace_with_import_is_prohibited()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'br/public:az@0.0.0' as sys

var myRg = sys.resourceGroup()

output rgLocation string = myRg.location
");

            result.Should().ContainDiagnostic("BCP084", DiagnosticLevel.Error, "The symbolic name \"sys\" is reserved. Please use a different symbolic name. Reserved namespaces are \"sys\".");
        }

        [TestMethod]
        public void Singleton_imports_cannot_be_used_multiple_times()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'br/public:az@0.0.0' as az1
provider 'br/public:az@0.0.0' as az2

provider 'sys@1.0.0' as sys1
provider 'sys@1.0.0' as sys2
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is declared multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is declared multiple times. Remove the duplicates."),
            });
        }

        [TestMethod]
        public void Import_names_must_not_conflict_with_other_symbols()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
provider 'br/public:az@0.0.0'
provider 'kubernetes@1.0.0' with {
  kubeConfig: ''
  namespace: ''
} as az
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"az\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"az\" is declared multiple times. Remove or rename the duplicates."),
            });
        }

        [TestMethod]
        public void Ambiguous_function_references_must_be_qualified()
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

            var services = ServicesWithImports.WithNamespaceProvider(nsProvider);

            var result = CompilationHelper.Compile(services, @"
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
            result = CompilationHelper.Compile(services, @"
provider 'ns1@1.0.0' as ns1
provider 'ns2@1.0.0' as ns2

output ambiguousResult string = ns1.dupeFunc()
output ns1Result string = ns1Func()
output ns2Result string = ns2Func()
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Config_with_optional_properties_can_be_skipped()
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

            var services = ServicesWithImports.WithNamespaceProvider(nsProvider);

            var result = CompilationHelper.Compile(services, @"
provider 'mockNs@1.0.0' with {
  optionalConfig: 'blah blah'
} as ns1
provider 'mockNs@1.0.0' as ns2
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void MicrosoftGraph_imports_succeed_with_preview_feature_enabled()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"provider 'microsoftGraph@1.0.0' as graph");

            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Provider namespace \"microsoftGraph\" is not recognized."),
            });

            var serviceWithPreview = new ServiceBuilder()
                .WithFeatureOverrides(new(TestContext, ExtensibilityEnabled: true, MicrosoftGraphPreviewEnabled: true));

            result = CompilationHelper.Compile(serviceWithPreview, @"provider 'microsoftGraph@1.0.0' as graph");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}
