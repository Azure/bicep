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
using Bicep.Core.Workspaces;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class TestFrameworkTests
    {
        private ServiceBuilder ServicesWithTestFramework => new ServiceBuilder()
            .WithFeatureOverrides(new(TestContext, TestFrameworkEnabled: true));

        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void TestFramework_is_disabled_unless_feature_is_enabled()
        {
            var result = CompilationHelper.Compile(@"
test test1 'test1.bicep' = {}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
            });
            result = CompilationHelper.Compile(@"
test test1 'test1.bicep' =
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP018", DiagnosticLevel.Error, "Expected the \"{\" character at this location."),
            });
            result = CompilationHelper.Compile(@"
test test1 'test1.bicep'
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location."),
            });
            result = CompilationHelper.Compile(@"
test test1
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP0347", DiagnosticLevel.Error,  "Expected a test path string at this location."),
            });
            result = CompilationHelper.Compile(@"
test
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP0346", DiagnosticLevel.Error,  "Expected a test identifier at this location."),
            });
        }

        [TestMethod]
        public void TestFramework_statement_parse_diagnostics_are_guiding()
        {
            var result = CompilationHelper.Compile(ServicesWithTestFramework, @"
test
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP0346", DiagnosticLevel.Error,  "Expected a test identifier at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithTestFramework, @"
test test1
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP0347", DiagnosticLevel.Error,  "Expected a test path string at this location."),
            });

            result = CompilationHelper.Compile(ServicesWithTestFramework, @"
test test1 ''
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location."),
            });
               
        }
        
    }
}
