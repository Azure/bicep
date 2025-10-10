// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Features;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests.Decorators
{
    [TestClass]
    public class WaitUntilDecoratorTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        [DataRow("@waitUntil(x => x.ProvisionStatus == 'Succeeded', 'PT20S')", "[lambda('x', equals(lambdaVariables('x').ProvisionStatus, 'Succeeded'))]")]
        [DataRow("@waitUntil(x => x.ProvisionStatus == 'Succeeded' && x.routingState == 'Provisioned', 'PT20S')", "[lambda('x', and(equals(lambdaVariables('x').ProvisionStatus, 'Succeeded'), equals(lambdaVariables('x').routingState, 'Provisioned')))]")]
        public void WaitUntilDecorator_ValidScenario(string input, string output)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var fileContent = $@"
            {input}
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {{
                name: 'sql-server-name'
                location: 'polandcentral'
            }}";

            var (template, diagnostics, _) = CompilationHelper.Compile(services, fileContent);

            var waitUntilObject = new JArray
            {
                output,
                "PT20S"
            };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();
                template.Should().NotBeNull().And.HaveValueAtPath("$.languageVersion", "2.1-experimental");
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.waitUntil", waitUntilObject);
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_MissingDeclaration_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @waitUntil(x => x.ProvisionStatus == 'Succeeded', 'PT20S')
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP149", DiagnosticLevel.Error, "Expected a resource declaration after the decorator."),
                });
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_MissingParameters_ExpectedTwoParameters()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @waitUntil(x => x.ProvisionStatus == 'Succeeded')
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP149", DiagnosticLevel.Error, "Expected a resource declaration after the decorator."),
                    ("BCP071", DiagnosticLevel.Error, "Expected 2 arguments, but got 1."),

                });
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_WithModuleDeclaration_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
                    @waitUntil(x => x.ProvisionStatus == 'Succeeded', 'PT20S')
                    module myModule 'module.bicep' = {
                      name: 'moduleb'
                      params: {
                        inputa: 'foo'
                        inputb: 'bar'
                      }
                    }
                    "
                ,
                [moduleUri] = @"
                    param inputa string
                    param inputb string
                    "
            };

            var compilation = services.BuildCompilation(files, mainUri);
            var success = !compilation.HasErrors();

            using (new AssertionScope())
            {
                compilation.GetSourceFileDiagnostics(mainUri).ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP128", DiagnosticLevel.Error, "Function \"waitUntil\" cannot be used as a module decorator.")
                });
                success.Should().BeFalse();
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_FirstArgumentIsntLambda()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var (template, diagnostics, _) = CompilationHelper.Compile(services, $@"
            @waitUntil('PT20S', x => x.ProvisionStatus == 'Succeeded')
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {{
                name: 'sql-server-name'
                location: 'polandcentral'
            }}");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"'PT20S'\" is not assignable to parameter of type \"object => bool\".")
                });
            }
        }

        [TestMethod]
        public void WaitUntilDecorator_SecondArgumentIsntString()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var (template, diagnostics, _) = CompilationHelper.Compile(services, $@"
            @waitUntil( x => x.ProvisionStatus == 'Succeeded', 1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {{
                name: 'sql-server-name'
                location: 'polandcentral'
            }}");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"1\" is not assignable to parameter of type \"string\".")
                });
            }
        }

        [TestMethod]
        [DataRow("@waitUntil(x => x.ProvisionStatus == 'Succeeded', 'PT20S')", "[lambda('x', equals(lambdaVariables('x').ProvisionStatus, 'Succeeded'))]")]
        [DataRow("@waitUntil(x => x.ProvisionStatus == 'Succeeded' && x.routingState == 'Provisioned', 'PT20S')", "[lambda('x', and(equals(lambdaVariables('x').ProvisionStatus, 'Succeeded'), equals(lambdaVariables('x').routingState, 'Provisioned')))]")]
        public void WaitUntilDecoratorWithCollection_ValidScenario(string input, string output)
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var fileContent = $@"
            {input}
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(0, 2) :{{
                name: 'sql-server-name_${{i}}'
                location: 'polandcentral'
            }}]";

            var (template, diagnostics, _) = CompilationHelper.Compile(services, fileContent);

            var waitUntilObject = new JArray
            {
                output,
                "PT20S"
            };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.waitUntil", waitUntilObject);
            }
        }

        [TestMethod]
        public void WaitUntilDecoratorWithCollection_FirstArgumentIsntLambda()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var (template, diagnostics, _) = CompilationHelper.Compile(services, $@"
            @waitUntil('PT20S', x => x.ProvisionStatus == 'Succeeded')
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(0, 2) :{{
                name: 'sql-server-name_${{i}}'
                location: 'polandcentral'
            }}]");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"'PT20S'\" is not assignable to parameter of type \"object => bool\".")
                });
            }
        }

        [TestMethod]
        public void WaitUntilDecoratorWithCollection_SecondArgumentIsntString()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var (template, diagnostics, _) = CompilationHelper.Compile(services, $@"
            @waitUntil( x => x.ProvisionStatus == 'Succeeded', 1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(0, 2) :{{
                name: 'sql-server-name_${{i}}'
                location: 'polandcentral'
            }}]");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"1\" is not assignable to parameter of type \"string\".")
                });
            }
        }
    }
}
