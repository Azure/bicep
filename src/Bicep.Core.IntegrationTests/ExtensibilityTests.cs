// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Concrete;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Extensions;
using Bicep.Core.UnitTests.Utils;
using Bicep.TextFixtures.Mocks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using ITypeReference = Azure.Bicep.Types.Concrete.ITypeReference;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExtensibilityTests : TestBase
    {
        private const string MockSubscriptionId = "00000000-0000-0000-0000-000000000001";
        private const string MockResourceGroupName = "mock-rg";

        #region Tests

        [TestMethod]
        public void Bar_import_bad_config_is_blocked()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension bar with {
  madeUpProperty: 'asdf'
} as stg
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"connectionString\"."),
                ("BCP037", DiagnosticLevel.Error, "The property \"madeUpProperty\" is not allowed on objects of type \"configuration\". Permissible properties include \"connectionString\".")
            });
        }

        [TestMethod]
        public void Bar_import_can_be_duplicated()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension bar with {
                connectionString: 'connectionString1'
            } as stg

            extension bar with {
                connectionString: 'connectionString2'
            } as stg2
            """);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Bar_import_basic_test()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension bar with {
               connectionString: 'asdf'
            } as stg

            resource container 'container' = {
               name: 'myblob'
            }

            resource blob 'blob' = {
               name: 'myblob'
               containerName: container.name
               base64Content: base64('sadfasdfd')
            }
            """);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Baz_import_bad_config_is_blocked()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension baz with {
  kind: 'Three'
}
");
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP036", DiagnosticLevel.Error, @"The property ""kind"" expected a value of type ""'One' | 'Two'"" but the provided value is of type ""'Three'"".")
            });
        }

        [TestMethod]
        public void Baz_import_valid_config_succeeds()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension baz with {
  kind: 'One'
  connectionStringOne: '******'
}
");
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Ambiguous_type_references_return_errors()
        {
            var services = CreateServiceBuilder();
            var result = CompilationHelper.Compile(services, """
            extension bar with {
            connectionString: 'asdf'
            } as stg

            extension bar with {
            connectionString: 'asdf'
            } as stg2

            resource container 'container' = {
            name: 'myblob'
            }
            """);
            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP264", DiagnosticLevel.Error, "Resource type \"container\" is declared in multiple imported namespaces (\"stg\", \"stg2\"), and must be fully-qualified."),
            });

            result = CompilationHelper.Compile(services, """
            extension bar with {
            connectionString: 'asdf'
            } as stg

            extension bar with {
            connectionString: 'asdf'
            } as stg2

            resource container 'stg2:container' = {
            name: 'myblob'
            }
            """);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Bar_import_basic_test_loops_and_referencing()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension bar with {
                connectionString: 'asdf'
            } as stg

            resource container 'container' = {
                name: 'myblob'
            }

            resource blobs 'blob' = [for i in range(0, 10): {
                name: 'myblob-${i}.txt'
                containerName: container.name
                base64Content: base64('Hello blob ${i}!')
            }]

            resource blobs2 'blob' = [for i in range(10, 10): {
                name: blobs[i - 10].name
                containerName: container.name
                base64Content: base64('Hello blob ${i}!')
            }]

            output sourceContainerName string = container.name
            #disable-next-line prefer-unquoted-property-names
            output sourceContainerNameSquare string = container['name']
            output miscBlobContainerName string = blobs[13 % 10].containerName
            output containerName string = blobs[5].containerName
            #disable-next-line prefer-unquoted-property-names
            output base64Content string = blobs[3]['base64Content']
            """);
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['sourceContainerName'].value", "[reference('container').name]");
            result.Template.Should().HaveValueAtPath("$.outputs['sourceContainerNameSquare'].value", "[reference('container').name]");
            result.Template.Should().HaveValueAtPath("$.outputs['miscBlobContainerName'].value", "[reference(format('blobs[{0}]', mod(13, 10))).containerName]");
            result.Template.Should().HaveValueAtPath("$.outputs['containerName'].value", "[reference(format('blobs[{0}]', 5)).containerName]");
            result.Template.Should().HaveValueAtPath("$.outputs['base64Content'].value", "[reference(format('blobs[{0}]', 3)).base64Content]");
        }

        [TestMethod]
        public void Foo_import_basic_test_loops_and_referencing()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension foo as foo
            param numApps int

            resource myApp 'application' = {
                uniqueName: 'foo'
            }

            resource myAppsLoop 'application' = [for i in range(0, numApps): {
                uniqueName: '${myApp.appId}-bar-${i}'
            }]

            output myAppId string = myApp.appId
            #disable-next-line prefer-unquoted-property-names
            output myAppId2 string = myApp['appId']
            output myAppsLoopId string = myAppsLoop[13 % numApps].appId
            #disable-next-line prefer-unquoted-property-names
            output myAppsLoopId2 string = myAppsLoop[3]['appId']
            """);
            result.Should().NotHaveAnyDiagnostics();
            result.Template.Should().HaveValueAtPath("$.outputs['myAppId'].value", "[reference('myApp').appId]");
            result.Template.Should().HaveValueAtPath("$.outputs['myAppId2'].value", "[reference('myApp').appId]");
            result.Template.Should().HaveValueAtPath("$.outputs['myAppsLoopId'].value", "[reference(format('myAppsLoop[{0}]', mod(13, parameters('numApps')))).appId]");
            result.Template.Should().HaveValueAtPath("$.outputs['myAppsLoopId2'].value", "[reference(format('myAppsLoop[{0}]', 3)).appId]");
        }

        [TestMethod]
        public void Foo_import_existing_requires_uniqueName()
        {
            var services = CreateServiceBuilder();
            // we've accidentally used 'name' even though this resource type doesn't support it
            var result = CompilationHelper.Compile(services, """
            extension foo

            resource myApp 'application' existing = {
            name: 'foo'
            }
            """);

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP035", DiagnosticLevel.Error, "The specified \"resource\" declaration is missing the following required properties: \"uniqueName\"."),
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"myApp\" is declared but never used."),
                ("BCP037", DiagnosticLevel.Error, "The property \"name\" is not allowed on objects of type \"application\". Permissible properties include \"uniqueName\". If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
            });

            // oops! let's change it to 'uniqueName'
            result = CompilationHelper.Compile(services, """
            extension foo as foo

            resource myApp 'application' existing = {
                uniqueName: 'foo'
            }
            """);

            result.Should().GenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"myApp\" is declared but never used."),
            });
        }

        [TestMethod]
        public void Kubernetes_import_existing_warns_with_readonly_fields()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension kubernetes with {
            namespace: 'default'
            kubeConfig: ''
            }
            resource service 'core/Service@v1' existing = {
            metadata: {
                name: 'existing-service'
                namespace: 'default'
                labels: {
                format: 'k8s-extension'
                }
                annotations: {
                foo: 'bar'
                }
            }
            }
            """);

            result.Should().GenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"service\" is declared but never used."),
                ("BCP073", DiagnosticLevel.Warning, "The property \"labels\" is read-only. Expressions cannot be assigned to read-only properties. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
                ("BCP073", DiagnosticLevel.Warning, "The property \"annotations\" is read-only. Expressions cannot be assigned to read-only properties. If this is a resource type definition inaccuracy, report it using https://aka.ms/bicep-type-issues."),
            });
        }

        [TestMethod]
        public async Task Extension_import_existing_blocked_for_writeonly_resource()
        {
            // Create a mock extension with a write only resource (no readable scopes, has writable scopes)
            var services = CreateServiceBuilder();
            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExtWithWriteOnlyResource());

            var mainUri = new Uri("file:///main.bicep");
            var files = new Dictionary<Uri, string>
            {
                [mainUri] = """
                extension 'br:mcr.microsoft.com/bicep/extensions/writeonly/v1:1.2.3'

                resource myResource 'writeOnlyType@v1' existing = {
                  identifier: 'test-resource'
                }
                """
            };

            var compilation = await services.BuildCompilationWithRestore(files, mainUri);

            compilation.Should().HaveDiagnostics(new[] {
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"myResource\" is declared but never used."),
                ("BCP441", DiagnosticLevel.Error, "Resource type \"writeOnlyType@v1\" cannot be used with the 'existing' keyword."),
            });
        }

        [TestMethod]
        public async Task Extension_writeonly_resource_can_be_deployed_normally()
        {
            // Create a mock extension with a write only resource
            var services = CreateServiceBuilder();
            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExtWithWriteOnlyResource());

            var mainUri = new Uri("file:///main.bicep");
            var files = new Dictionary<Uri, string>
            {
                [mainUri] = """
                extension 'br:mcr.microsoft.com/bicep/extensions/writeonly/v1:1.2.3'

                // Write-only resources should work fine for normal (non-existing) deployment
                resource myResource 'writeOnlyType@v1' = {
                  identifier: 'test-resource'
                }

                output resourceId string = myResource.identifier
                """
            };

            var compilation = await services.BuildCompilationWithRestore(files, mainUri);

            compilation.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Kubernetes_competing_imports_are_blocked()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension kubernetes with {
  namespace: 'default'
  kubeConfig: ''
}

extension kubernetes with {
  namespace: 'default'
  kubeConfig: ''
}
");

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP028", DiagnosticLevel.Error, "Identifier \"kubernetes\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"kubernetes\" is declared multiple times. Remove the duplicates."),
                ("BCP028", DiagnosticLevel.Error, "Identifier \"kubernetes\" is declared multiple times. Remove or rename the duplicates."),
                ("BCP207", DiagnosticLevel.Error, "Namespace \"kubernetes\" is declared multiple times. Remove the duplicates."),
            });
        }

        [TestMethod]
        public void Kubernetes_import_existing_resources()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension kubernetes with {
  namespace: 'default'
  kubeConfig: ''
}
resource service 'core/Service@v1' existing = {
  metadata: {
    name: 'existing-service'
    namespace: 'default'
  }
}
resource secret 'core/Secret@v1' existing = {
  metadata: {
    name: 'existing-secret'
    namespace: 'default'
  }
}
resource configmap 'core/ConfigMap@v1' existing = {
  metadata: {
    name: 'existing-configmap'
    namespace: 'default'
  }
}
");

            result.Should().GenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"service\" is declared but never used."),
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"secret\" is declared but never used."),
                ("no-unused-existing-resources", DiagnosticLevel.Warning, "Existing resource \"configmap\" is declared but never used."),
            });
        }

        [TestMethod]
        public void Kubernetes_import_existing_connectionstring_test()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension kubernetes with {
  namespace: 'default'
  kubeConfig: ''
}
resource redisService 'core/Service@v1' existing = {
  metadata: {
    name: 'redis-service'
    namespace: 'default'
  }
}
resource redisSecret 'core/Secret@v1' existing = {
  metadata: {
    name: 'redis-secret'
    namespace: 'default'
  }
}
resource secret 'core/Secret@v1' = {
  metadata: {
    name: 'conn-secret'
    namespace: 'default'
    labels: {
      format: 'k8s-extension'
    }
  }
  stringData: {
    connectionString: '${redisService.metadata.name}.${redisService.metadata.namespace}.svc.cluster.local,password=${base64ToString(redisSecret.data.redisPassword)}'
  }
}
");

            result.Should().GenerateATemplate();
            result.Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Kubernetes_CustomResourceType_EmitWarning()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
                extension kubernetes with {
                  namespace: 'default'
                  kubeConfig: ''
                }
                resource crd 'custom/Foo@v1' = {
                  metadata: {
                    name: 'existing-service'
                  }
                }
                """);

            result.Should().GenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP081", DiagnosticLevel.Warning, @"Resource type ""custom/Foo@v1"" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
            });
        }

        [TestMethod]
        public void Kubernetes_AmbiguousFallbackType_MustFullyQualify()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
                extension kubernetes with {
                  namespace: 'default'
                  kubeConfig: ''
                }

                resource ambiguous 'Microsoft.Compute/availabilitySets@2023-01-01' = {
                  metadata: {
                    name: 'existing-service'
                  }
                }

                resource availabilitySet 'az:Microsoft.Compute/availabilitySets@2023-01-01' = {
                }

                resource custom 'kubernetes:Microsoft.Foo/bar@2023-01-01' = {
                  metadata: {
                    name: 'custom'
                  }
                }
                """);

            result.Should().NotGenerateATemplate();
            result.Should().HaveDiagnostics(new[] {
                ("BCP264", DiagnosticLevel.Error, @"Resource type ""Microsoft.Compute/availabilitySets@2023-01-01"" is declared in multiple imported namespaces (""az"", ""kubernetes""), and must be fully-qualified."),
                ("BCP035", DiagnosticLevel.Error, @"The specified ""resource"" declaration is missing the following required properties: ""name""."),
                ("BCP081", DiagnosticLevel.Warning, @"Resource type ""Microsoft.Compute/availabilitySets@2023-01-01"" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                ("BCP081", DiagnosticLevel.Warning, @"Resource type ""Microsoft.Foo/bar@2023-01-01"" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
            });
        }

        [TestMethod]
        public void Bar_import_basic_test_with_qualified_type()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension bar with {
            connectionString: 'asdf'
            } as stg

            resource container 'stg:container' = {
            name: 'myblob'
            }

            resource blob 'stg:blob' = {
            name: 'myblob'
            containerName: container.name
            base64Content: base64('sadfasdfd')
            }
            """);
            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
        }

        [TestMethod]
        public void Invalid_namespace_qualifier_returns_error()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), """
            extension bar with {
            connectionString: 'asdf'
            } as stg

            resource container 'foo:container' = {
            name: 'myblob'
            }

            resource blob 'bar:blob' = {
            name: 'myblob'
            containerName: container.name
            base64Content: base64('sadfasdfd')
            }
            """);

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP208", DiagnosticLevel.Error, "The specified namespace \"foo\" is not recognized. Specify a resource reference using one of the following namespaces: \"az\", \"stg\", \"sys\"."),
                ("BCP208", DiagnosticLevel.Error, "The specified namespace \"bar\" is not recognized. Specify a resource reference using one of the following namespaces: \"az\", \"stg\", \"sys\"."),
            });
        }

        [TestMethod]
        public void Child_resource_with_parent_namespace_mismatch_returns_error()
        {
            var result = CompilationHelper.Compile(CreateServiceBuilder(), @"
extension bar with {
  connectionString: 'asdf'
} as stg

resource parent 'az:Microsoft.Storage/storageAccounts@2020-01-01' existing = {
  name: 'stgParent'

  resource container 'stg:container' = {
    name: 'myblob'
  }
}
");

            result.ExcludingLinterDiagnostics().Should().HaveDiagnostics(new[] {
                ("BCP081", DiagnosticLevel.Warning, "Resource type \"Microsoft.Storage/storageAccounts@2020-01-01\" does not have types available. Bicep is unable to validate resource properties prior to deployment, but this will not block the resource from being deployed."),
                ("BCP210", DiagnosticLevel.Error, "Resource type belonging to namespace \"stg\" cannot have a parent resource type belonging to different namespace \"az\"."),
            });
        }

        [TestMethod]
        public void Extensibility_v2_emitting_produces_expected_template()
        {
            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(ModuleExtensionConfigsEnabled: true))
                .WithConfigurationPatch(c => c.WithExtensions("""
                    {
                      "az": "builtin:",
                      "foo": "builtin:"
                    }
                    """))
                .WithNamespaceProvider(TestExtensionsNamespaceProvider.CreateWithDefaults());

            var result = CompilationHelper.Compile(services, """
                extension foo as foo

                resource myApp 'application' = {
                    uniqueName: 'foo'
                }
                """);

            result.Should().NotHaveAnyDiagnostics();

            result.Template.Should().HaveValueAtPath("$.languageVersion", "2.2-experimental");
            result.Template.Should().HaveValueAtPath("$.extensions.foo.name", "Foo");
            result.Template.Should().HaveValueAtPath("$.resources.myApp.extension", "foo");
        }

        [DataTestMethod]
        [DataRow(
            "InlineValues", "{ kubeConfig: 'fromModule', namespace: 'other' }", null)]
        [DataRow(
            "KeyVaultReference",
            "{ kubeConfig: kv.getSecret('myKubeConfig'), namespace: 'other' }",
            $$"""{ kubeConfig: az.getSecret('{{MockSubscriptionId}}', '{{MockResourceGroupName}}', 'kv', 'myKubeConfig'), namespace: 'other' }""")]
        public async Task Module_with_required_extension_config_can_be_compiled_successfully(string scenario, string moduleKubeExtConfig, string? paramsKubeExtConfig)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");
            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    $$"""
                      using 'main.bicep'

                      param inputa = 'abc'

                      extensionConfig k8s with {{paramsKubeExtConfig ?? moduleKubeExtConfig}}
                      """,
                [mainUri] =
                    $$"""
                      param inputa string

                      extension kubernetes as k8s

                      extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

                      resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
                        name: 'kv'
                      }

                      module modulea 'modulea.bicep' = {
                        name: 'modulea'
                        params: {
                          inputa: inputa
                        }
                        extensionConfigs: {
                          kubernetes: {{moduleKubeExtConfig}}
                        }
                      }

                      output outputa string = modulea.outputs.outputa
                      """,
                [moduleAUri] =
                    """
                    param inputa string

                    extension kubernetes

                    extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1:1.2.3' as graph

                    output outputa string = inputa
                    """
            };

            var services = await CreateServiceBuilderWithMockMsGraph(moduleExtensionConfigsEnabled: true);
            var compilation = await services.BuildCompilationWithRestore(files, paramsUri);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError());
        }

        [DataTestMethod]
        [DataRow(
            "MissingExtensionConfigs",
            "extension kubernetes",
            "",
            "BCP035",
            """The specified "module" declaration is missing the following required properties: "extensionConfigs".""")]
        [DataRow(
            "MissingRequiredExtensionConfig",
            "extension kubernetes",
            "extensionConfigs: {}",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "kubernetes".""")]
        [DataRow(
            "MissingRequiredConfigProperty",
            "extension kubernetes",
            "extensionConfigs: { kubernetes: { namespace: 'other' } }",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "kubeConfig".""")]
        [DataRow(
            "MissingRequiredConfigProperties",
            "extension kubernetes",
            "extensionConfigs: { kubernetes: { } }",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "kubeConfig", "namespace".""")]
        [DataRow(
            "PropertyIsNotDefinedInSchema",
            "extension kubernetes",
            "extensionConfigs: { kubernetes: { kubeConfig: 'test', namespace: 'other', extra: 'extra' } }",
            "BCP037",
            """The property "extra" is not allowed on objects of type "configuration". Permissible properties include "context".""")]
        [DataRow(
            "ConfigProvidedForExtensionThatDoesNotAcceptConfig",
            "extension kubernetes",
            "extensionConfigs: { kubernetes: { kubeConfig: 'test', namespace: 'other' }, noConfig: { } }",
            "BCP037",
            """The property "noConfig" is not allowed on objects of type "extensionConfigs". No other properties are allowed.""")]
        [DataRow(
            "ConfigProvidedForNonExistentExtension",
            "extension kubernetes",
            "extensionConfigs: { kubernetes: { kubeConfig: 'test', namespace: 'other' }, nonExistent: { } }",
            "BCP037",
            """The property "nonExistent" is not allowed on objects of type "extensionConfigs". No other properties are allowed.""")]
        [DataRow(
            "MissingRequiredConfigProperty",
            "extension kubernetes with { namespace: 'default' }",
            "extensionConfigs: { kubernetes: { } }",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "kubeConfig".""")]
        [DataRow(
            "DiscriminatedType_AssignmentMissingProperties",
            "extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' with { discrim: 'a' } as mockExt",
            "extensionConfigs: { mockExt: { a2: 'a1 not defined' } }",
            "BCP035",
            "The specified \"object\" declaration is missing the following required properties: \"a1\".")]
        public async Task Module_with_invalid_extension_config_produces_diagnostic(
            string scenarioName,
            string extensionDeclStr,
            string moduleExtensionConfigsStr,
            string expectedDiagnosticCode,
            string expectedDiagnosticMessage)
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [mainUri] =
                    $$"""
                      param inputa string

                      module modulea 'modulea.bicep' = {
                        name: 'modulea'
                        params: {
                          inputa: inputa
                        }
                        {{moduleExtensionConfigsStr}}
                      }

                      output outputa string = modulea.outputs.outputa
                      """,
                [moduleAUri] =
                    $$"""
                      param inputa string

                      {{extensionDeclStr}}

                      extension 'br:mcr.microsoft.com/bicep/extensions/noconfig/v1:1.2.3' as noConfig

                      output outputa string = inputa
                      """
            };

            var services = CreateServiceBuilder(moduleExtensionConfigsEnabled: true);

            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExtWithNoConfigType(), CreateMockExtWithDiscriminatedConfigType());

            var compilation = await services.BuildCompilationWithRestore(files, mainUri);

            compilation.Should().ContainSingleDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
        }

        [DataTestMethod]
        [DataRow(
            "ParamsFile",
            "BCP337",
            """This declaration type is not valid for a Bicep Parameters file. Supported declarations: "using", "extends", "param", "var", "type".""")]
        [DataRow(
            "MainFile",
            "BCP037",
            """The property "extensionConfigs" is not allowed on objects of type "module". Permissible properties include "dependsOn", "identity", "scope".""")]
        public void Extension_config_assignments_raise_error_diagnostic_if_expr_feature_disabled(string scenario, string expectedDiagnosticCode, string expectedDiagnosticMessage)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    """
                    using 'main.bicep'

                    param inputa = 'abc'

                    extensionConfig k8s with {
                      kubeConfig: 'abc'
                      namespace: 'other'
                    }
                    """,
                [mainUri] =
                    """
                    param inputa string

                    extension kubernetes as k8s

                    module modulea 'modulea.bicep' = {
                      name: 'modulea'
                      params: {
                        inputa: inputa
                      }
                      extensionConfigs: {
                        kubernetes: {
                          kubeConfig: 'fromModule'
                          namespace: 'other'
                        }
                      }
                    }

                    output outputa string = modulea.outputs.outputa
                    output extOutput object = k8s.config
                    """,
                [moduleAUri] =
                    """
                    param inputa string

                    extension kubernetes

                    output outputa string = inputa
                    """
            };

            if (scenario is "ParamsFile")
            {
                files[mainUri] = files[moduleAUri];
                files.Remove(moduleAUri);
            }

            var compilation = CreateServiceBuilder().BuildCompilation(files, paramsUri);

            var fileUriWithDiag = scenario is "ParamsFile" ? paramsUri : mainUri;

            compilation.GetSourceFileDiagnostics(fileUriWithDiag).Should().ContainDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);

            if (scenario is "MainFile")
            {
                compilation.GetSourceFileDiagnostics(mainUri).Should().ContainDiagnostic("BCP052", DiagnosticLevel.Error, """The type "k8s" does not contain property "config".""");
            }
        }

        [DataTestMethod]
        [DataRow(
            "FullInheritance",
            "extensionConfigs: { kubernetes: k8s.config }",
            """{ "kubernetes": "[extensions('k8s').config]" }""")]
        [DataRow(
            "PiecemealInheritance",
            "extensionConfigs: { kubernetes: { kubeConfig: k8s.config.kubeConfig, namespace: k8s.config.namespace } }",
            """{ "kubernetes": { kubeConfig: "[extensions('k8s').config.kubeConfig]", namespace: "[extensions('k8s').config.namespace]" } }""")]
        [DataRow(
            "Ternary",
            "extensionConfigs: { kubernetes: { kubeConfig: inputa == 'a' ? k8s.config.kubeConfig : 'b', namespace: inputa == 'a' ? k8s.config.namespace : 'c' } }",
            """{ "kubernetes": { "kubeConfig": "[if(equals(parameters('inputa'), 'a'), extensions('k8s').config.kubeConfig, createObject('value', 'b'))]", "namespace": "[if(equals(parameters('inputa'), 'a'), extensions('k8s').config.namespace, createObject('value', 'c'))]" } }""")]
        public async Task Modules_can_inherit_parent_module_extension_configs(string scenario, string moduleExtensionConfigsStr, string expectedExtConfigJson)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    """
                    using 'main.bicep'

                    param inputa = 'abc'

                    extensionConfig k8s with {
                      kubeConfig: 'abc'
                      namespace: 'other'
                    }
                    """,
                [mainUri] =
                    $$"""
                      param inputa string

                      extension kubernetes as k8s

                      module modulea 'modulea.bicep' = {
                        name: 'modulea'
                        params: {
                          inputa: inputa
                        }
                        {{moduleExtensionConfigsStr}}
                      }

                      output outputa string = modulea.outputs.outputa
                      """,
                [moduleAUri] =
                    """
                    param inputa string

                    extension kubernetes

                    output outputa string = inputa
                    """
            };

            var services = await CreateServiceBuilderWithMockMsGraph(moduleExtensionConfigsEnabled: true);
            var compilation = services.BuildCompilation(files, paramsUri);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError());

            var templateToken = JToken.Parse(compilation.GetTestTemplate(mainUri));

            templateToken.SelectToken("resources.modulea.properties.extensionConfigs")
                .Should()
                .DeepEqual(JToken.Parse(expectedExtConfigJson));
        }

        [DataTestMethod]
        [DataRow(
            "NoneRequired",
            "",
            "extension kubernetes with { kubeConfig: 'templateKubeConfig', namespace: 'templateNs' } as k8s")]
        [DataRow(
            "PartiallyRequired",
            "extensionConfig k8s with { kubeConfig: 'paramsKubeConfig' }",
            "extension kubernetes with { namespace: 'templateNs' } as k8s")]
        [DataRow(
            "AllRequired",
            "extensionConfig k8s with { kubeConfig: 'paramsKubeConfig', namespace: 'paramsNs'}",
            "extension kubernetes as k8s")]
        [DataRow(
            "DiscriminatedType_DeclHasDiscriminator",
            "extensionConfig mockExt with { a1: 'foo' }",
            "extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' with { discrim: 'a' } as mockExt")]
        [DataRow(
            "DiscriminatedType_NoDefaults",
            "extensionConfig mockExt with { discrim: 'b', b1: 'fooo' }",
            "extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' as mockExt")]
        [DataRow(
            "AssignmentEmpty",
            "extensionConfig k8s with {}",
            "extension kubernetes with { kubeConfig: 'templateKubeConfig', namespace: 'templateNs' } as k8s")]
        public async Task Extension_config_assignment_type_checks_are_cross_module_aware(
            string scenarioName,
            string paramsFileExtensionConfigAssignment,
            string bicepFileExtensionDeclaration)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    $$"""
                      using 'main.bicep'

                      param inputa = 'abc'

                      {{paramsFileExtensionConfigAssignment}}
                      """,
                [mainUri] =
                    $$"""
                      param inputa string

                      {{bicepFileExtensionDeclaration}}
                      """
            };

            var services = CreateServiceBuilder(moduleExtensionConfigsEnabled: true);

            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExtWithDiscriminatedConfigType());

            var compilation = await services.BuildCompilationWithRestore(files, paramsUri);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError());
        }

        [DataTestMethod]
        [DataRow(
            "IncompleteSyntax_ToAlias",
            "extensionConfig k8s",
            "extension kubernetes as k8s",
            "BCP206",
            """Extension "k8s" requires configuration, but none was provided.""")]
        [DataRow(
            "RequiredAssignmentMissing",
            "",
            "extension kubernetes as k8s",
            "BCP424",
            """The following extensions are declared in the Bicep file but are missing a configuration assignment in the params files: "k8s".""")]
        [DataRow(
            "AssignmentDoesNotMatch",
            "extensionConfig k8s with { kubeConfig: '' }",
            "",
            "BCP425",
            """The extension configuration assignment for "k8s" does not match an extension in the Bicep file.""")]
        [DataRow(
            "DiscriminatedType_AssignmentMissingProperties",
            "extensionConfig mockExt with { a2: 'a1 not defined' }",
            "extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' with { discrim: 'a' } as mockExt",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "a1".""")]
        [DataRow(
            "DiscriminatedType_DiscrimReassignment",
            "extensionConfig mockExt with { discrim: 'a', b1: 'here to suppress diag because b type is selected' }",
            "extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' with { discrim: 'b' } as mockExt",
            "BCP037",
            """The property "discrim" is not allowed on objects of type "b". Permissible properties include "z1".""")]
        [DataRow(
            "DiscriminatedType_BicepLimitationForSharedPropertiesWithoutDiscrim",
            "extensionConfig mockExt with { discrim: 'b' }",
            "extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' with { z1: 'shared property' } as mockExt",
            "BCP078",
            """The property "discrim" requires a value of type "'a' | 'b'", but none was supplied.""")]
        [DataRow(
            "TernaryLimitation",
            "extensionConfig mockExt with { requiredSecureString: bool(readEnvironmentVariable('xyz', 'false')) ? az.getSecret('a', 'b', 'c', 'd') : az.getSecret('w', 'x', 'y', 'z') }",
            "extension 'br:mcr.microsoft.com/bicep/extensions/secureconfig/v1:1.2.3' as mockExt",
            "BCP338",
            """Failed to evaluate extension config "mockExt": Cannot emit unexpected expression of type ParameterKeyVaultReferenceExpression"""
        )]
        public async Task Invalid_extension_config_assignments_should_raise_error_diagnostic(string scenario, string paramsFileExtensionConfigAssignment, string bicepFileExtensionDeclaration, string expectedDiagnosticCode, string expectedDiagnosticMessage)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    $$"""
                      using 'main.bicep'

                      {{paramsFileExtensionConfigAssignment}}
                      """,
                [mainUri] = bicepFileExtensionDeclaration
            };

            var services = CreateServiceBuilder(moduleExtensionConfigsEnabled: true);

            await ExtensionTestHelper.AddMockExtensions(services, TestContext, CreateMockExtWithDiscriminatedConfigType(), CreateMockExtWithSecureConfigType());

            var compilation = await services.BuildCompilationWithRestore(files, paramsUri);

            if (scenario is "DiscriminatedType_BicepLimitationForSharedPropertiesWithoutDiscrim")
            {
                compilation.GetSourceFileDiagnostics(mainUri).Should().ContainSingleDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
                compilation.GetSourceFileDiagnostics(paramsUri).Should().ContainDiagnostic("BCP035", DiagnosticLevel.Error, "The specified \"object\" declaration is missing the following required properties: \"b1\".");

                return;
            }

            compilation.GetSourceFileDiagnostics(mainUri).ExcludingLinterDiagnostics().Should().BeEmpty();
            compilation.GetSourceFileDiagnostics(paramsUri).Should().ContainSingleDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
        }

        [TestMethod]
        public async Task Missing_extension_configs_should_have_code_fix()
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");

            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    """
                    using 'main.bicep'
                    """,
                [mainUri] =
                    """
                    extension 'br:mcr.microsoft.com/bicep/extensions/noconfig/v1:1.2.3' as mockExtNoConfig
                    extension 'br:mcr.microsoft.com/bicep/extensions/configobj/v1:1.2.3' as mockExtObj
                    extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' as mockExtDiscrim
                    extension 'br:mcr.microsoft.com/bicep/extensions/configdiscrimobj/v1:1.2.3' with { discrim: 'b' } as mockExtDiscrim2
                    """
            };

            var services = CreateServiceBuilder(moduleExtensionConfigsEnabled: true);

            await ExtensionTestHelper.AddMockExtensions(
                services,
                TestContext,
                CreateMockExtWithNoConfigType(),
                CreateMockExtWithObjectConfigType(),
                CreateMockExtWithDiscriminatedConfigType());

            var compilation = await services.BuildCompilationWithRestore(files, paramsUri);

            compilation.GetSourceFileDiagnostics(mainUri).ExcludingLinterDiagnostics().Should().BeEmpty();
            compilation.GetSourceFileDiagnostics(paramsUri).Should().ContainSingleDiagnostic("BCP424", DiagnosticLevel.Error, "The following extensions are declared in the Bicep file but are missing a configuration assignment in the params files: \"mockExtDiscrim\", \"mockExtDiscrim2\", \"mockExtObj\".");

            var paramsCompilationResult = CompilationHelper.CompileParams(compilation);

            var codeFixDiag = compilation.GetSourceFileDiagnostics(paramsUri).Single(d => d.Code == "BCP424");

            paramsCompilationResult.ApplyCodeFix(codeFixDiag)
                .Should()
                .EqualIgnoringNewlines(
                    """
                    using 'main.bicep'

                    extensionConfig mockExtDiscrim with {
                      discrim:
                    }

                    extensionConfig mockExtDiscrim2 with {
                      b1:
                    }

                    extensionConfig mockExtObj with {
                      requiredString:
                    }
                    """);
        }

        #endregion

        #region Helpers

        private ServiceBuilder CreateServiceBuilder(bool moduleExtensionConfigsEnabled = false) =>
            new ServiceBuilder()
                .WithConfigurationPatch(
                    c => c.WithExtensions(
                        """
                        {
                          "az": "builtin:",
                          "kubernetes": "builtin:",
                          "foo": "builtin:",
                          "bar": "builtin:",
                          "baz": "builtin:"
                        }
                        """))
                .WithNamespaceProvider(TestExtensionsNamespaceProvider.CreateWithDefaults())
                .WithFeaturesOverridden(
                    f => f with { ModuleExtensionConfigsEnabled = moduleExtensionConfigsEnabled });

        private async Task<ServiceBuilder> CreateServiceBuilderWithMockMsGraph(bool moduleExtensionConfigsEnabled = false)
        {
            var services = CreateServiceBuilder(moduleExtensionConfigsEnabled);
            services = await ExtensionTestHelper.AddMockMsGraphExtension(services, TestContext);

            return services;
        }

        private static MockExtensionData CreateMockExtWithNoConfigType(string extName = "noconfig") =>
            ExtensionTestHelper.CreateMockExtensionMockData(
                extName, "1.2.3", "v1", CustomExtensionTypeFactoryDelegates.NoTypes);

        private static MockExtensionData CreateMockExtWithObjectConfigType(string extName = "configobj") =>
            ExtensionTestHelper.CreateMockExtensionMockData(
                extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
                {
                    CreateConfigurationType = (ctx, tf) => tf.Create(() => new ObjectType(
                        "config",
                        new Dictionary<string, ObjectTypeProperty>
                        {
                            ["requiredString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null),
                            ["optionalString"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                        },
                        null))
                });

        private static MockExtensionData CreateMockExtWithSecureConfigType(string extName = "secureconfig") =>
            ExtensionTestHelper.CreateMockExtensionMockData(
                extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
                {
                    CreateConfigurationType = (ctx, tf) => tf.Create(() => new ObjectType(
                        "config",
                        new Dictionary<string, ObjectTypeProperty>
                        {
                            ["requiredSecureString"] = new(ctx.CoreSecureStringTypeRef, ObjectTypePropertyFlags.Required, null)
                        },
                        null))
                });

        private static MockExtensionData CreateMockExtWithDiscriminatedConfigType(string extName = "configdiscrimobj") =>
            ExtensionTestHelper.CreateMockExtensionMockData(
                extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
                {
                    CreateConfigurationType = (ctx, tf) => tf.Create(() => new DiscriminatedObjectType(
                        "config",
                        "discrim",
                        new Dictionary<string, ObjectTypeProperty>
                        {
                            ["z1"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                        },
                        new Dictionary<string, ITypeReference>
                        {
                            ["a"] = ctx.CreateObjectType(
                                "aType", new Dictionary<string, ObjectTypeProperty>
                                {
                                    ["discrim"] = new(ctx.CreateStringLiteralType("a"), ObjectTypePropertyFlags.Required, null),
                                    ["a1"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null),
                                    ["a2"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.None, null)
                                }),
                            ["b"] = ctx.CreateObjectType(
                                "bType", new Dictionary<string, ObjectTypeProperty>
                                {
                                    ["discrim"] = new(ctx.CreateStringLiteralType("b"), ObjectTypePropertyFlags.Required, null),
                                    ["b1"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required, null)
                                })
                        }))
                });

        private static MockExtensionData CreateMockExtWithWriteOnlyResource(string extName = "writeonly") =>
            ExtensionTestHelper.CreateMockExtensionMockData(
                extName, "1.2.3", "v1", new CustomExtensionTypeFactoryDelegates
                {
                    CreateResourceTypes = (ctx, tf) =>
                    {
                        var bodyType = tf.Create(() => new ObjectType(
                            "writeOnlyBody",
                            new Dictionary<string, ObjectTypeProperty>
                            {
                                ["identifier"] = new(ctx.CoreStringTypeRef, ObjectTypePropertyFlags.Required | ObjectTypePropertyFlags.Identifier, "The resource identifier")
                            },
                            null));

                        // Create a write only resource: no readable scopes, but has writable scopes
                        var writeOnlyType = tf.Create(() => new ResourceType(
                            "writeOnlyType@v1",
                            tf.GetReference(bodyType),
                            null,
                            writableScopes_in: ScopeType.All,
                            readableScopes_in: (ScopeType)0)); // No readable scopes makes it write only

                        return [writeOnlyType];
                    }
                });

        #endregion

    }
}
