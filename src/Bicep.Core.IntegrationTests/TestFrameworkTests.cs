// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
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
test test1 '' = {}
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP050", DiagnosticLevel.Error, @"The specified path is empty."),
            });
            result = CompilationHelper.Compile(@"
test test1 '' =
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP050", DiagnosticLevel.Error, @"The specified path is empty."),
                ("BCP018", DiagnosticLevel.Error, "Expected the \"{\" character at this location."),
            });
            result = CompilationHelper.Compile(@"
test test1 ''
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP050", DiagnosticLevel.Error, @"The specified path is empty."),
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location."),
            });
            result = CompilationHelper.Compile(@"
test test1
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP0347", DiagnosticLevel.Error,  "Expected a test path string at this location."),
                ("BCP358", DiagnosticLevel.Error, "This declaration is missing a template file path reference.")
            });
            result = CompilationHelper.Compile(@"
test
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP348", DiagnosticLevel.Error, "Using a test declaration statement requires enabling EXPERIMENTAL feature \"TestFramework\"."),
                ("BCP0346", DiagnosticLevel.Error,  "Expected a test identifier at this location."),
                ("BCP358", DiagnosticLevel.Error, "This declaration is missing a template file path reference.")
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
                ("BCP358", DiagnosticLevel.Error,  "This declaration is missing a template file path reference.")

            });

            result = CompilationHelper.Compile(ServicesWithTestFramework, @"
test test1
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP0347", DiagnosticLevel.Error,  "Expected a test path string at this location."),
                ("BCP358", DiagnosticLevel.Error, "This declaration is missing a template file path reference.")
            });

            result = CompilationHelper.Compile(ServicesWithTestFramework, @"
test test1 ''
");
            result.Should().HaveDiagnostics(new[] {
                ("BCP050", DiagnosticLevel.Error, @"The specified path is empty."),
                ("BCP018", DiagnosticLevel.Error, "Expected the \"=\" character at this location."),
            });

        }
        [TestMethod]
        public void TestFramework_should_not_have_diagnostics_on_required_parameters()
        {
            var result = CompilationHelper.Compile(ServicesWithTestFramework,
                                                    ("testMain.bicep", @"
                                                param name string
                                                "),
                                                    ("main.bicep", @"
                                                test foo 'testMain.bicep' = {
                                                params: {
                                                    name: 'us'
                                                }
                                                }
                                                "));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }
        [TestMethod]
        public void TestFramework_should_have_diagnostics_on_wrong_parameter_type()
        {
            var result = CompilationHelper.Compile(ServicesWithTestFramework,
                                                    ("main.bicep", @"
                                                test foo 'testMain.bicep' = {
                                                params: {
                                                    name: 1
                                                }
                                                }
                                                "), ("testMain.bicep", @"
                                                param name string
                                                "));

            result.Should().HaveDiagnostics(new[] {
            ("BCP036", DiagnosticLevel.Error, "The property \"name\" expected a value of type \"string\" but the provided value is of type \"1\"."),
        });
        }

        [TestMethod]
        public void TestFramework_should_have_diagnostics_when_missing_parameters()
        {
            var result = CompilationHelper.Compile(ServicesWithTestFramework,
                                                    ("main.bicep", @"
                                                test foo 'testMain.bicep' = {
                                                params: {
                                                }
                                                }
                                                "), ("testMain.bicep", @"
                                                param name string
                                                "));

            result.Should().HaveDiagnostics(new[] {
        ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"name\"."),
        });
        }

        [TestMethod]
        public void TestFramework_should_have_diagnostics_when_missing_parameters_property()
        {
            var result = CompilationHelper.Compile(ServicesWithTestFramework,
                                                    ("main.bicep", @"
                                                test foo 'testMain.bicep' = {
                                                }
                                                "), ("testMain.bicep", @"
                                                param name string
                                                "));

            result.Should().HaveDiagnostics(new[] {
        ("BCP035", DiagnosticLevel.Error, "The specified \"test\" declaration is missing the following required properties: \"params\"."),
        });
        }

    }

}
