// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ImportTests
    {
        private ServiceBuilder ServicesWithImports => new ServiceBuilder()
            .WithFeatureProvider(BicepTestConstants.CreateFeatureProvider(TestContext, importsEnabled: true));

        private class TestNamespaceProvider : INamespaceProvider
        {
            private readonly ImmutableDictionary<string, Func<string, NamespaceType>> builderDict;

            public TestNamespaceProvider(Dictionary<string, Func<string, NamespaceType>> builderDict)
            {
                this.builderDict = builderDict.ToImmutableDictionary();
            }

            public bool AllowImportStatements => true;

            public IEnumerable<string> AvailableNamespaces => builderDict.Keys.Concat(new [] { SystemNamespaceType.BuiltInName });

            public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope, IFeatureProvider features)
            {
                switch (providerName)
                {
                    case SystemNamespaceType.BuiltInName:
                        return SystemNamespaceType.Create(aliasName, features);
                    case { } _ when builderDict.TryGetValue(providerName) is { } builderFunc:
                        return builderFunc(aliasName);
                }

                return null;
            }
        }

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void Imports_are_disabled_unless_feature_is_enabled()
        {
            var result = CompilationHelper.Compile(@"
import az as foo
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Import statements are currently not supported."),
            });
        }

        [TestMethod]
        public void Import_statement_parse_diagnostics_are_guiding()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP201", DiagnosticLevel.Error, "Expected an import provider name at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
import az
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP012", DiagnosticLevel.Error, "Expected the \"as\" keyword at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithImports, @"
import az as
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected an import alias name at this location."),
            });
        }

        [TestMethod]
        public void Imports_return_error_with_unrecognized_namespace()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import madeUpNamespace as foo
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Imported namespace \"madeUpNamespace\" is not recognized."),
            });
        }

        [TestMethod]
        public void Import_configuration_is_blocked_by_default()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import az as ns {
  foo: 'bar'
}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP205", DiagnosticLevel.Error, "Imported namespace \"az\" does not support configuration."),
            });
        }

        [TestMethod]
        public void Using_import_statements_frees_up_the_namespace_symbol()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import az as newAz

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
import az as sys
import sys as az

var myRg = sys.resourceGroup()

@az.description('why on earth would you do this?')
output rgLocation string = myRg.location
");

            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs.rgLocation.metadata.description", "why on earth would you do this?");
        }

        [TestMethod]
        public void Overwriting_single_built_in_namespace_with_import_is_permitted()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import az as sys

var myRg = sys.resourceGroup()

output rgLocation string = myRg.location
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Singleton_imports_cannot_be_used_multiple_times()
        {
            var result = CompilationHelper.Compile(ServicesWithImports, @"
import az as az1
import az as az2

import sys as sys1
import sys as sys2
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is imported multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"az\" is imported multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is imported multiple times. Remove the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"sys\" is imported multiple times. Remove the duplicates."),
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
import ns1 as ns1
import ns2 as ns2

output ambiguousResult string = dupeFunc()
output ns1Result string = ns1Func()
output ns2Result string = ns2Func()
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP056", DiagnosticLevel.Error, "The reference to name \"dupeFunc\" is ambiguous because it exists in namespaces \"ns1\", \"ns2\". The reference must be fully-qualified."),
            });

            // fix by fully-qualifying
            result = CompilationHelper.Compile(services, @"
import ns1 as ns1
import ns2 as ns2

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
                        ArmTemplateProviderVersion: "1.0"),
                    ImmutableArray<TypeProperty>.Empty,
                    ImmutableArray<FunctionOverload>.Empty,
                    ImmutableArray<BannedFunction>.Empty,
                    ImmutableArray<Decorator>.Empty,
                    new EmptyResourceTypeProvider()),
            });

            var services = ServicesWithImports.WithNamespaceProvider(nsProvider);

            var result = CompilationHelper.Compile(services, @"
import mockNs as ns1 {
  optionalConfig: 'blah blah'
}
import mockNs as ns2
");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}
