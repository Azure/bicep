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
    public class OnlyIfNotExistsDecoratorTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void OnlyIfNotExistsDecorator_ValidScenario()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @onlyIfNotExists()
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");

            var onlyIfNotExistsJObject = new JArray { };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.onlyIfNotExists", onlyIfNotExistsJObject);
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.languageVersion", "2.0");
            }
        }

        [TestMethod]
        public void OnlyIfNotExistsAndRetryOnDecorator_ValidScenario()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @onlyIfNotExists()
            @retryOn(['ResourceNotFound', 'ServerError'], 1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");

            var onlyIfNotExistsJObject = new JArray { };

            var retryOnJObject = new JArray
            {
                new JArray("ResourceNotFound", "ServerError"),
                1
            };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().NotBeNull().And.HaveValueAtPath("$.languageVersion", "2.1-experimental");
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.onlyIfNotExists", onlyIfNotExistsJObject);
                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.retryOn", retryOnJObject);
            }
        }

        [TestMethod]
        public void OnlyIfNotExistsDecorator_WithModuleDeclaration_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));

            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
                    @onlyIfNotExists()
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
            var diagnosticsByFile = compilation.GetAllDiagnosticsByBicepFile().ToDictionary(kvp => kvp.Key.Uri, kvp => kvp.Value);
            var success = diagnosticsByFile.Values.SelectMany(x => x).All(d => !d.IsError());

            using (new AssertionScope())
            {
                diagnosticsByFile[mainUri].ExcludingLinterDiagnostics().ExcludingMissingTypes().Should().HaveDiagnostics(new[] {
                    ("BCP128", DiagnosticLevel.Error, "Function \"onlyIfNotExists\" cannot be used as a module decorator.")
                });
                success.Should().BeFalse();
            }
        }

        [TestMethod]
        public void OnlyIfNotExistsDecorator_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @onlyIfNotExists()
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
        public void OnlyIfNotExistsDecoratorWithCollections_ValidScenario()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @onlyIfNotExists()
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(1, 2):{
                name: 'sql-server-name${i}'
                location: 'polandcentral'
            }]
            ");

            var onlyIfNotExistsJObject = new JArray { };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.onlyIfNotExists", onlyIfNotExistsJObject);
            }
        }

        [TestMethod]
        public void OnlyIfNotExistsDecorator_Arguments_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @onlyIfNotExists(1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP071", DiagnosticLevel.Error, "Expected 0 arguments, but got 1."),
                });
            }
        }
    }
}

