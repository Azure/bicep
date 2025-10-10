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
    public class RetryOnDecoratorTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void RetryOnDecorator_InvalidErrorMessageItemType()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound', 1010])
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"string\" but the provided value is of type \"'ResourceNotFound' | 1010\".")
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_ValidScenario()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound', 'ServerError'], 1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");

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
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.retryOn", retryOnJObject);
            }
        }

        [TestMethod]
        public void RetryOnDecorator_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'])
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
        public void RetryOnDecorator_WithModuleDeclaration_ShouldFail()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));

            var mainUri = new Uri("file:///main.bicep");
            var moduleUri = new Uri("file:///module.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] = @"
                    @retryOn(['ResourceNotFound'])
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
                    ("BCP128", DiagnosticLevel.Error, "Function \"retryOn\" cannot be used as a module decorator.")
                });
                success.Should().BeFalse();
            }
        }

        [TestMethod]
        public void RetryOnDecorator_WithRetryCountOptionalParameter_ExpectedResourceDeclaration()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 5)
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
        public void RetryOnDecorator_InvalidRetryCount()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 0)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 0) is too small to assign to a target for which the minimum allowable value is 1.")
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_NegativeRetryCount()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], -5)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to -5) is too small to assign to a target for which the minimum allowable value is 1."),
                });
            }
        }

        [TestMethod]
        public void RetryOnDecorator_NonIntegerRetryCountValue()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 'randomString')
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
                name: 'sql-server-name'
                location: 'polandcentral'
            }
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"'randomString'\" is not assignable to parameter of type \"int\"."),
                });
            }
        }


        [TestMethod]
        public void RetryOnDecoratorWithCollections_ValidScenario()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound', 'ServerError'], 1)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(1, 2):{
                name: 'sql-server-name${i}'
                location: 'polandcentral'
            }]
            ");

            var retryOnJObject = new JArray
            {
                new JArray("ResourceNotFound", "ServerError"),
                1
            };

            using (new AssertionScope())
            {
                diagnostics.ExcludingLinterDiagnostics().Should().BeEmpty();

                template.Should().NotBeNull()
                    .And.HaveValueAtPath("$.resources['sqlServer'].@options.retryOn", retryOnJObject);
            }
        }

        [TestMethod]
        public void RetryOnDecoratorWithCollection_InvalidErrorMessageItemType()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound', 1010])
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(1, 2):{
                name: 'sql-server-name${i}'
                location: 'polandcentral'
            }]
            ");

            using (new AssertionScope())
            {
                template.Should().NotHaveValue();

                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP033", DiagnosticLevel.Error, "Expected a value of type \"string\" but the provided value is of type \"'ResourceNotFound' | 1010\".")
                });
            }
        }

        [TestMethod]
        public void RetryOnDecoratorWithCollection_InvalidRetryCount()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 0)
             resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(1, 2):{
                name: 'sql-server-name${i}'
                location: 'polandcentral'
            }]
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to 0) is too small to assign to a target for which the minimum allowable value is 1.")
                });
            }
        }

        [TestMethod]
        public void RetryOnDecoratorWithCollection_NegativeRetryCount()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], -5)
            resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(1, 2):{
                name: 'sql-server-name${i}'
                location: 'polandcentral'
            }]
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP328", DiagnosticLevel.Error, "The provided value (which will always be less than or equal to -5) is too small to assign to a target for which the minimum allowable value is 1."),
                });
            }
        }

        [TestMethod]
        public void RetryOnDecoratorWithCollections_NonIntegerRetryCountValue()
        {
            var services = new ServiceBuilder().WithFeatureOverrides(new FeatureProviderOverrides(TestContext, WaitAndRetryEnabled: true));
            var (template, diagnostics, _) = CompilationHelper.Compile(services, @"
            @retryOn(['ResourceNotFound'], 'randomString')
             resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = [for i in range(1, 2):{
                name: 'sql-server-name${i}'
                location: 'polandcentral'
            }]
            ");
            using (new AssertionScope())
            {
                template.Should().NotHaveValue();
                diagnostics.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                    ("BCP070", DiagnosticLevel.Error, "Argument of type \"'randomString'\" is not assignable to parameter of type \"int\"."),
                });
            }
        }
    }
}
