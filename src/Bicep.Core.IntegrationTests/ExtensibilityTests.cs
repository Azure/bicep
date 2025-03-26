// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.IntegrationTests.Extensibility;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ExtensibilityTests : TestBase
    {
        private static ServiceBuilder Services => new ServiceBuilder()
            .WithFeatureOverrides(new(ExtensibilityEnabled: true))
            .WithConfigurationPatch(c => c.WithExtensions("""
            {
              "az": "builtin:",
              "kubernetes": "builtin:",
              "foo": "builtin:",
              "bar": "builtin:"
            }
            """))
            .WithNamespaceProvider(TestExtensibilityNamespaceProvider.CreateWithDefaults());

        private static ServiceBuilder ServicesWithModuleExtensionConfigs => Services.WithFeatureOverrides(new(ExtensibilityEnabled: true, ModuleExtensionConfigsEnabled: true));

        [TestMethod]
        public void Bar_import_bad_config_is_blocked()
        {
            var result = CompilationHelper.Compile(Services, @"
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
            var result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
        public void Ambiguous_type_references_return_errors()
        {
            var result = CompilationHelper.Compile(Services, """
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

            result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
            // we've accidentally used 'name' even though this resource type doesn't support it
            var result = CompilationHelper.Compile(Services, """
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
            result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
        public void Kubernetes_competing_imports_are_blocked()
        {
            var result = CompilationHelper.Compile(Services, @"
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
            var result = CompilationHelper.Compile(Services, @"
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
            var result = CompilationHelper.Compile(Services, @"
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
            var result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, """
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
            var result = CompilationHelper.Compile(Services, @"
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
        public void Bar_import_end_to_end_test()
        {
            var result = CompilationHelper.Compile(Services,
                ("main.bicep", @"
param accountName string

resource stgAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: toLower(accountName)
  location: resourceGroup().location
  kind: 'Storage'
  sku: {
    name: 'Standard_LRS'
  }
}

var connectionString = 'DefaultEndpointsProtocol=https;AccountName=${stgAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${stgAccount.listKeys().keys[0].value}'

module website './website.bicep' = {
  name: 'website'
  params: {
    connectionString: connectionString
  }
}
"),
                ("website.bicep", @"
@secure()
param connectionString string

extension bar with {
  connectionString: connectionString
} as stg

resource container 'container' = {
  name: 'bicep'
}

resource blob 'blob' = {
  name: 'blob.txt'
  containerName: container.name
  base64Content: base64(loadTextContent('blob.txt'))
}
"),
                ("blob.txt", @"
Hello from Bicep!"));

            result.ExcludingLinterDiagnostics().Should().NotHaveAnyDiagnostics();
            result.Template.Should().DeepEqual(JToken.Parse("""
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
  "languageVersion": "2.1-experimental",
  "contentVersion": "1.0.0.0",
  "metadata": {
    "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
    "_EXPERIMENTAL_FEATURES_ENABLED": [
      "Extensibility"
    ],
    "_generator": {
      "name": "bicep",
      "version": "dev",
      "templateHash": "8414774091366329766"
    }
  },
  "parameters": {
    "accountName": {
      "type": "string"
    }
  },
  "resources": {
    "stgAccount": {
      "type": "Microsoft.Storage/storageAccounts",
      "apiVersion": "2019-06-01",
      "name": "[toLower(parameters('accountName'))]",
      "location": "[resourceGroup().location]",
      "kind": "Storage",
      "sku": {
        "name": "Standard_LRS"
      }
    },
    "website": {
      "type": "Microsoft.Resources/deployments",
      "apiVersion": "2022-09-01",
      "name": "website",
      "properties": {
        "expressionEvaluationOptions": {
          "scope": "inner"
        },
        "mode": "Incremental",
        "parameters": {
          "connectionString": {
            "value": "[format('DefaultEndpointsProtocol=https;AccountName={0};EndpointSuffix={1};AccountKey={2}', toLower(parameters('accountName')), environment().suffixes.storage, listKeys(resourceId('Microsoft.Storage/storageAccounts', toLower(parameters('accountName'))), '2019-06-01').keys[0].value)]"
          }
        },
        "template": {
          "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#",
          "languageVersion": "2.1-experimental",
          "contentVersion": "1.0.0.0",
          "metadata": {
            "_EXPERIMENTAL_WARNING": "This template uses ARM features that are experimental. Experimental features should be enabled for testing purposes only, as there are no guarantees about the quality or stability of these features. Do not enable these settings for any production usage, or your production environment may be subject to breaking.",
            "_EXPERIMENTAL_FEATURES_ENABLED": [
              "Extensibility"
            ],
            "_generator": {
              "name": "bicep",
              "version": "dev",
              "templateHash": "8473853033217630197"
            }
          },
          "parameters": {
            "connectionString": {
              "type": "securestring"
            }
          },
          "variables": {
            "$fxv#0": "\nHello from Bicep!"
          },
          "imports": {
            "stg": {
              "provider": "Bar",
              "version": "0.0.1",
              "config": {
                "connectionString": "[parameters('connectionString')]"
              }
            }
          },
          "resources": {
            "container": {
              "import": "stg",
              "type": "container",
              "properties": {
                "name": "bicep"
              }
            },
            "blob": {
              "import": "stg",
              "type": "blob",
              "properties": {
                "name": "blob.txt",
                "containerName": "[reference('container').name]",
                "base64Content": "[base64(variables('$fxv#0'))]"
              },
              "dependsOn": [
                "container"
              ]
            }
          }
        }
      },
      "dependsOn": [
        "stgAccount"
      ]
    }
  }
}
"""));
        }

        [TestMethod]
        public void Extensibility_v2_emitting_produces_expected_template()
        {
            var services = new ServiceBuilder()
                .WithFeatureOverrides(new(
                    ExtensibilityEnabled: true,
                    ExtensibilityV2EmittingEnabled: true))
                .WithConfigurationPatch(c => c.WithExtensions("""
                    {
                      "az": "builtin:",
                      "foo": "builtin:"
                    }
                    """))
                .WithNamespaceProvider(TestExtensibilityNamespaceProvider.CreateWithDefaults());

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

        [TestMethod]
        public void Module_with_required_extension_config_can_be_compiled_successfully()
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            // TODO(kylealbert): Remove 'with' clause in template when that's removed
            // TODO(kylealbert): Uncomment graph when I figure out how to deal with the registry.
            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    """
                    using 'main.bicep'

                    param inputa = 'abc'

                    extension k8s with {
                      kubeConfig: 'abc'
                      namespace: 'other'
                    }
                    """,
                [mainUri] =
                    """
                    param inputa string

                    extension kubernetes with {
                      kubeConfig: 'DELETE'
                      namespace: 'DELETE'
                    } as k8s

                    //extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview'

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
                    """,
                [moduleAUri] =
                    """
                    param inputa string

                    extension kubernetes with {
                      kubeConfig: 'DELETE'
                      namespace: 'DELETE'
                    }

                    //extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

                    output outputa string = inputa
                    """
            };

            var compilation = ServicesWithModuleExtensionConfigs.BuildCompilation(files, paramsUri);

            compilation.Should().NotHaveAnyDiagnostics_WithAssertionScoping(d => d.IsError());
        }

        [DataTestMethod]
        [DataRow(
            "MissingExtensionConfigsDeclaration",
            "",
            "BCP035",
            """The specified "module" declaration is missing the following required properties: "extensionConfigs".""")]
        [DataRow(
            "MissingRequiredExtensionConfig",
            "extensionConfigs: {}",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "kubernetes".""")]
        [DataRow(
            "MissingRequiredConfigProperty",
            "extensionConfigs: { kubernetes: { namespace: 'other' } }",
            "BCP035",
            """The specified "object" declaration is missing the following required properties: "kubeConfig".""")]
        [DataRow(
            "PropertyIsNotDefinedInSchema",
            "extensionConfigs: { kubernetes: { kubeConfig: 'test', namespace: 'other', extra: 'extra' } }",
            "BCP037",
            """The property "extra" is not allowed on objects of type "configuration". Permissible properties include "context".""")]
        [DataRow(
            "ConfigProvidedForExtensionThatDoesNotAcceptConfig",
            "extensionConfigs: { kubernetes: { kubeConfig: 'test', namespace: 'other' }, graph: { } }",
            "BCP037",
            """The property "graph" is not allowed on objects of type "extensionConfigs". No other properties are allowed.""")]
        public void Module_with_invalid_extension_config_produces_diagnostic(
            string scenarioName,
            string moduleExtensionConfigsStr,
            string expectedDiagnosticCode,
            string expectedDiagnosticMessage)
        {
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            // TODO(kylealbert): Remove 'with' clause in template when that's removed
            // TODO(kylealbert): Uncomment graph when I figure out how to deal with the registry.
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
                    """
                    param inputa string

                    extension kubernetes with {
                      kubeConfig: ''
                      namespace: 'default'
                    }

                    //extension microsoftGraph as graph

                    output outputa string = inputa
                    """
            };

            var compilation = ServicesWithModuleExtensionConfigs.BuildCompilation(files, mainUri);

            compilation.Should().ContainSingleDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
        }

        [DataTestMethod]
        [DataRow(
            "ParamsFile",
            "BCP337",
            $"""This declaration type is not valid for a Bicep Parameters file. Specify a "{LanguageConstants.UsingKeyword}", "{LanguageConstants.ExtendsKeyword}", "{LanguageConstants.ParameterKeyword}" or "{LanguageConstants.VariableKeyword}" declaration.""")]
        [DataRow(
            "MainFile",
            "BCP037",
            """The property "extensionConfigs" is not allowed on objects of type "module". Permissible properties include "dependsOn", "scope".""")]
        public void Extension_config_assignments_raise_error_diagnostic_if_expr_feature_disabled(string scenario, string expectedDiagnosticCode, string expectedDiagnosticMessage)
        {
            var paramsUri = new Uri("file:///main.bicepparam");
            var mainUri = new Uri("file:///main.bicep");
            var moduleAUri = new Uri("file:///modulea.bicep");

            // TODO(kylealbert): Remove 'with' clause in template when that's removed
            // TODO(kylealbert): Uncomment graph when I figure out how to deal with the registry.
            var files = new Dictionary<Uri, string>
            {
                [paramsUri] =
                    """
                    using 'main.bicep'

                    param inputa = 'abc'

                    extension k8s with {
                      kubeConfig: 'abc'
                      namespace: 'other'
                    }
                    """,
                [mainUri] =
                    """
                    param inputa string

                    extension kubernetes with {
                      kubeConfig: 'DELETE'
                      namespace: 'DELETE'
                    } as k8s

                    //extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview'

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
                    """,
                [moduleAUri] =
                    """
                    param inputa string

                    extension kubernetes with {
                      kubeConfig: 'DELETE'
                      namespace: 'DELETE'
                    }

                    //extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview' as graph

                    output outputa string = inputa
                    """
            };

            if (scenario is "ParamsFile")
            {
                files[mainUri] = files[moduleAUri];
                files.Remove(moduleAUri);
            }

            var compilation = Services.BuildCompilation(files, paramsUri);

            var diagByFile = compilation.GetAllDiagnosticsByBicepFileUri();

            var fileUriWithDiag = scenario is "ParamsFile" ? paramsUri : mainUri;
            diagByFile[fileUriWithDiag].Should().ContainSingleDiagnostic(expectedDiagnosticCode, DiagnosticLevel.Error, expectedDiagnosticMessage);
        }
    }
}
