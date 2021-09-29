// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ImportTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private CompilationHelper.CompilationHelperContext EnabledImportsContext
            => new CompilationHelper.CompilationHelperContext(Features: BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true));

        [TestMethod]
        public void Imports_are_disabled_unless_feature_is_enabled()
        {
            var result = CompilationHelper.Compile(@"
import foo from az
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP203", DiagnosticLevel.Error, "Import statements are currently not supported."),
            });
        }

        [TestMethod]
        public void Import_statement_parse_diagnostics_are_guiding()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import 
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP202", DiagnosticLevel.Error, "Expected an import alias name at this location."),
            });

            result = CompilationHelper.Compile(EnabledImportsContext, @"
import ns 
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP012", DiagnosticLevel.Error, "Expected the \"from\" keyword at this location."),
            });

            result = CompilationHelper.Compile(EnabledImportsContext, @"
import ns from 
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP201", DiagnosticLevel.Error, "Expected an import provider name at this location."),
            });
        }

        [TestMethod]
        public void Imports_return_error_with_unrecognized_namespace()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import foo from madeUpNamespace
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP204", DiagnosticLevel.Error, "Imported namespace \"madeUpNamespace\" is not recognized."),
            });
        }

        [TestMethod]
        public void Import_configuration_is_blocked_by_default()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import ns from az {
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
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import newAz from az

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
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import sys from az
import az from sys

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
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import sys from az

var myRg = sys.resourceGroup()

output rgLocation string = myRg.location
");

            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Singleton_imports_cannot_be_used_multiple_times()
        {
            var result = CompilationHelper.Compile(EnabledImportsContext, @"
import az1 from az
import az2 from az

import sys1 from sys
import sys2 from sys
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
            var nsProviderMock = new Mock<INamespaceProvider>(MockBehavior.Strict);
            nsProviderMock.SetupGet(x => x.AllowImportStatements).Returns(true);
            nsProviderMock.Setup(x => x.TryGetNamespace(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResourceScope>()))
                .Returns<string, string, ResourceScope>((providerName, aliasName, resourceScope) => {
                    switch (providerName)
                    {
                        case "ns1":
                            return new NamespaceType(
                                aliasName,
                                new NamespaceSettings(
                                    IsSingleton: true,
                                    BicepProviderName: "ns1",
                                    ConfigurationType: null,
                                    ArmTemplateProviderName: "Ns1-Unused",
                                    ArmTemplateProviderVersion: "1.0"),
                                ImmutableArray<TypeProperty>.Empty,
                                new [] { 
                                    new FunctionOverloadBuilder("ns1Func").Build(),
                                    new FunctionOverloadBuilder("dupeFunc").Build(),
                                },
                                ImmutableArray<BannedFunction>.Empty,
                                ImmutableArray<Decorator>.Empty,
                                new EmptyResourceTypeProvider());
                        case "ns2":
                            return new NamespaceType(
                                aliasName,
                                new NamespaceSettings(
                                    IsSingleton: true,
                                    BicepProviderName: "ns2",
                                    ConfigurationType: null,
                                    ArmTemplateProviderName: "Ns2-Unused",
                                    ArmTemplateProviderVersion: "1.0"),
                                ImmutableArray<TypeProperty>.Empty,
                                new [] { 
                                    new FunctionOverloadBuilder("ns2Func").Build(),
                                    new FunctionOverloadBuilder("dupeFunc").Build(),
                                },
                                ImmutableArray<BannedFunction>.Empty,
                                ImmutableArray<Decorator>.Empty,
                                new EmptyResourceTypeProvider());
                        default:
                            return null;
                    }
                });

            var context = new CompilationHelper.CompilationHelperContext(
                Features: BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true),
                NamespaceProvider: nsProviderMock.Object);

            var result = CompilationHelper.Compile(context, @"
import ns1 from ns1
import ns2 from ns2

output ambiguousResult string = dupeFunc()
output ns1Result string = ns1Func()
output ns2Result string = ns2Func()
");

            result.Should().HaveDiagnostics(new[] {
                ("BCP056", DiagnosticLevel.Error, "The reference to name \"dupeFunc\" is ambiguous because it exists in namespaces \"ns1\", \"ns2\". The reference must be fully-qualified."),
            });

            // fix by fully-qualifying
            result = CompilationHelper.Compile(context, @"
import ns1 from ns1
import ns2 from ns2

output ambiguousResult string = ns1.dupeFunc()
output ns1Result string = ns1Func()
output ns2Result string = ns2Func()
");

            result.Should().NotHaveAnyDiagnostics();
        }
    }
}