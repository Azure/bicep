// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.TypeSystem.Az;
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
        [NotNull]
        public TestContext? TestContext { get; set; }

        private CompilationHelper.CompilationHelperContext EnabledImportsContext
            => new CompilationHelper.CompilationHelperContext(AzResourceTypeProvider.CreateWithAzTypes(), BicepTestConstants.CreateFeaturesProvider(TestContext, importsEnabled: true));

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
    }
}