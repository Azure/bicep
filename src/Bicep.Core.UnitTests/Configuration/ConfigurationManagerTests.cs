// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OnDiskFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.Core.UnitTests.Configuration
{
    [TestClass]
    public class ConfigurationManagerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void GetBuiltInConfiguration_NoParameter_ReturnsBuiltInConfigurationWithAnalyzerSettings()
        {
            // Arrange.
            var configuration = IConfigurationManager.GetBuiltInConfiguration();

            // Assert.
            configuration.Should().HaveContents(/*lang=json,strict*/ """
      {
        "cloud": {
          "currentProfile": "AzureCloud",
          "profiles": {
            "AzureChinaCloud": {
              "resourceManagerEndpoint": "https://management.chinacloudapi.cn",
              "activeDirectoryAuthority": "https://login.chinacloudapi.cn"
            },
            "AzureCloud": {
              "resourceManagerEndpoint": "https://management.azure.com",
              "activeDirectoryAuthority": "https://login.microsoftonline.com"
            },
            "AzureUSGovernment": {
              "resourceManagerEndpoint": "https://management.usgovcloudapi.net",
              "activeDirectoryAuthority": "https://login.microsoftonline.us"
            }
          },
          "credentialPrecedence": [
            "AzureCLI",
            "AzurePowerShell"
          ]
        },
        "moduleAliases": {
          "ts": {},
          "br": {
            "public": {
              "registry": "mcr.microsoft.com",
              "modulePath": "bicep"
            }
          }
        },
        "extensions": {
          "az": "builtin:",
          "kubernetes": "builtin:"
        },
        "implicitExtensions": ["az"],
        "analyzers": {
          "core": {
            "verbose": false,
            "enabled": true,
            "rules": {
              "no-hardcoded-env-urls": {
                "level": "warning",
                "disallowedhosts": [
                  "azuredatalakeanalytics.net",
                  "azuredatalakestore.net",
                  "batch.core.windows.net",
                  "core.windows.net",
                  "database.windows.net",
                  "datalake.azure.net",
                  "gallery.azure.com",
                  "graph.windows.net",
                  "login.microsoftonline.com",
                  "management.azure.com",
                  "management.core.windows.net",
                  "vault.azure.net"
                ],
                "excludedhosts": [
                  "schema.management.azure.com"
                ]
              }
            }
          }
        },
        "experimentalFeaturesEnabled": {
          "extendableParamFiles": false,
          "symbolicNameCodegen": false,
          "extensibility": false,
          "moduleExtensionConfigs": false,
          "resourceTypedParamsAndOutputs": false,
          "sourceMapping": false,
          "legacyFormatter": false,
          "testFramework": false,
          "assertions": false,
          "waitAndRetry": false,
          "localDeploy": false,
          "resourceInfoCodegen": false,
          "typedVariables": false,
          "desiredStateConfiguration": false,
          "externalInputFunction": false,
          "onlyIfNotExists": false
        },
        "formatting": {
          "indentKind": "Space",
          "newlineKind": "LF",
          "insertFinalNewline": true,
          "indentSize": 2,
          "width": 120
        }
      }
      """);
        }

        [TestMethod]
        public void GetBuiltInConfiguration_CoreLinterShouldDefaultToEnabled()
        {
            var configuration = IConfigurationManager.GetBuiltInConfiguration();

            configuration.Analyzers.GetValue<bool>("core.enabled", false).Should().Be(true, "Core linters should default to enabled");
        }

        [TestMethod]
        public void GetBuiltInConfiguration_DisableAllAnalyzers_ReturnsBuiltInConfigurationWithoutAnalyzerSettings()
        {
            // Arrange.
            var configuration = IConfigurationManager.GetBuiltInConfiguration().WithAllAnalyzersDisabled();

            // Assert.
            configuration.Should().HaveContents(/*lang=json,strict*/ """
      {
        "cloud": {
          "currentProfile": "AzureCloud",
          "profiles": {
            "AzureChinaCloud": {
              "resourceManagerEndpoint": "https://management.chinacloudapi.cn",
              "activeDirectoryAuthority": "https://login.chinacloudapi.cn"
            },
            "AzureCloud": {
              "resourceManagerEndpoint": "https://management.azure.com",
              "activeDirectoryAuthority": "https://login.microsoftonline.com"
            },
            "AzureUSGovernment": {
              "resourceManagerEndpoint": "https://management.usgovcloudapi.net",
              "activeDirectoryAuthority": "https://login.microsoftonline.us"
            }
          },
          "credentialPrecedence": [
            "AzureCLI",
            "AzurePowerShell"
          ]
        },
        "moduleAliases": {
          "ts": {},
          "br": {
            "public": {
              "registry": "mcr.microsoft.com",
              "modulePath": "bicep"
            }
          }
        },
        "extensions": {
            "az": "builtin:",
            "kubernetes": "builtin:"
        },
        "implicitExtensions": [
            "az"
        ],
        "analyzers": {},
        "experimentalFeaturesEnabled": {
          "extendableParamFiles": false,
          "symbolicNameCodegen": false,
          "extensibility": false,
          "resourceTypedParamsAndOutputs": false,
          "sourceMapping": false,
          "legacyFormatter": false,
          "testFramework": false,
          "assertions": false,
          "waitAndRetry": false,
          "localDeploy": false,
          "resourceInfoCodegen": false,
          "typedVariables": false,
          "moduleExtensionConfigs": false,
          "desiredStateConfiguration": false,
          "externalInputFunction": false,
          "onlyIfNotExists": false
        },
        "formatting": {
          "indentKind": "Space",
          "newlineKind": "LF",
          "insertFinalNewline": true,
          "indentSize": 2,
          "width": 120
        }
      }
      """);
        }

        [TestMethod]
        public void GetBuiltInConfiguration_DisableAnalyzers_ReturnsBuiltInConfiguration_WithSomeAnalyzersSetToLevelOff()
        {
            // Arrange.
            var configuration = IConfigurationManager.GetBuiltInConfiguration().WithAnalyzersDisabled("no-hardcoded-env-urls", "no-unused-vars");

            // Assert.
            configuration.Should().HaveContents(/*lang=json,strict*/ """
      {
        "cloud": {
          "currentProfile": "AzureCloud",
          "profiles": {
            "AzureChinaCloud": {
              "resourceManagerEndpoint": "https://management.chinacloudapi.cn",
              "activeDirectoryAuthority": "https://login.chinacloudapi.cn"
            },
            "AzureCloud": {
              "resourceManagerEndpoint": "https://management.azure.com",
              "activeDirectoryAuthority": "https://login.microsoftonline.com"
            },
            "AzureUSGovernment": {
              "resourceManagerEndpoint": "https://management.usgovcloudapi.net",
              "activeDirectoryAuthority": "https://login.microsoftonline.us"
            }
          },
          "credentialPrecedence": [
            "AzureCLI",
            "AzurePowerShell"
          ]
        },
        "moduleAliases": {
          "ts": {},
          "br": {
            "public": {
              "registry": "mcr.microsoft.com",
              "modulePath": "bicep"
            }
          }
        },
        "extensions": {
            "az": "builtin:",
            "kubernetes": "builtin:"
        },
        "implicitExtensions": [
            "az"
        ],
        "analyzers": {
          "core": {
            "verbose": false,
            "enabled": true,
            "rules": {
              "no-hardcoded-env-urls": {
                "level": "off",
                "disallowedhosts": [
                  "azuredatalakeanalytics.net",
                  "azuredatalakestore.net",
                  "batch.core.windows.net",
                  "core.windows.net",
                  "database.windows.net",
                  "datalake.azure.net",
                  "gallery.azure.com",
                  "graph.windows.net",
                  "login.microsoftonline.com",
                  "management.azure.com",
                  "management.core.windows.net",
                  "vault.azure.net"
                ],
                "excludedhosts": [
                  "schema.management.azure.com"
                ]
              },
              "no-unused-vars": {
                "level": "off"
              }
            }
          }
        },
        "experimentalFeaturesEnabled": {
          "extendableParamFiles": false,
          "symbolicNameCodegen": false,
          "extensibility": false,
          "resourceTypedParamsAndOutputs": false,
          "sourceMapping": false,
          "legacyFormatter": false,
          "testFramework": false,
          "assertions": false,
          "waitAndRetry": false,
          "localDeploy": false,
          "resourceInfoCodegen": false,
          "typedVariables": false,
          "moduleExtensionConfigs": false,
          "desiredStateConfiguration": false,
          "externalInputFunction": false,
          "onlyIfNotExists": false
        },
        "formatting": {
          "indentKind": "Space",
          "newlineKind": "LF",
          "insertFinalNewline": true,
          "indentSize": 2,
          "width": 120
        }
      }
      """);
        }

        [TestMethod]
        public void GetConfiguration_CustomConfigurationNotFound_ReturnsBuiltInConfiguration()
        {
            // Arrange.
            var fileExplorer = new FileSystemFileExplorer(new OnDiskFileSystem());
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(this.CreatePath("foo/bar/main.bicep"));

            // Act.
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Assert.
            configuration.Should().BeSameAs(IConfigurationManager.GetBuiltInConfiguration());
        }

        [TestMethod]
        public void GetConfiguration_InvalidCustomConfiguration_PropagatesFailedToParseConfigurationDiagnostic()
        {
            // Arrange.
            var configurationPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurationPath] = "",
            });
            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            var diagnostics = sut.GetConfiguration(sourceFileUri).Diagnostics;
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().Be($"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\" as valid JSON: The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.");
        }

        [TestMethod]
        public void GetConfiguration_ConfigurationFileNotReadable_PropagatesCouldNotLoadConfigurationDiagnostic()
        {
            // Arrange.
            var configurationPath = CreatePath("path/to/bicepconfig.json");
            var configFileData = new MockFileData("")
            {
                AllowedFileShare = FileShare.None,
            };
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurationPath] = configFileData,
            });

            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            var diagnostics = sut.GetConfiguration(sourceFileUri).Diagnostics;
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().StartWith($"Could not load the Bicep configuration file \"{configurationPath}\":");
        }

        [TestMethod]
        public void GetBuiltInConfiguration_EnableExperimentalFeature_ReturnsBuiltInConfiguration_WithSelectedExperimentalFeatureEnabled()
        {
            // Arrange.
            var configuration = IConfigurationManager.GetBuiltInConfiguration();

            ExperimentalFeaturesEnabled experimentalFeaturesEnabled = new(
                SymbolicNameCodegen: false,
                Extensibility: false,
                ExtendableParamFiles: true,
                ResourceTypedParamsAndOutputs: false,
                SourceMapping: false,
                LegacyFormatter: false,
                TestFramework: false,
                Assertions: false,
                WaitAndRetry: false,
                LocalDeploy: false,
                ResourceInfoCodegen: false,
                TypedVariables: false,
                ModuleExtensionConfigs: false,
                DesiredStateConfiguration: false,
                ExternalInputFunction: false,
                OnlyIfNotExists: false);

            configuration.WithExperimentalFeaturesEnabled(experimentalFeaturesEnabled).Should().HaveContents(/*lang=json,strict*/ """
            {
            "cloud": {
                "currentProfile": "AzureCloud",
                "profiles": {
                "AzureChinaCloud": {
                    "resourceManagerEndpoint": "https://management.chinacloudapi.cn",
                    "activeDirectoryAuthority": "https://login.chinacloudapi.cn"
                },
                "AzureCloud": {
                    "resourceManagerEndpoint": "https://management.azure.com",
                    "activeDirectoryAuthority": "https://login.microsoftonline.com"
                },
                "AzureUSGovernment": {
                    "resourceManagerEndpoint": "https://management.usgovcloudapi.net",
                    "activeDirectoryAuthority": "https://login.microsoftonline.us"
                }
                },
                "credentialPrecedence": [
                "AzureCLI",
                "AzurePowerShell"
                ]
            },
            "moduleAliases": {
                "ts": {},
                "br": {
                "public": {
                    "registry": "mcr.microsoft.com",
                    "modulePath": "bicep"
                }
                }
            },
            "extensions": {
                "kubernetes": "builtin:",
                "az": "builtin:"
            },
            "implicitExtensions": [
                "az"
            ],
            "analyzers": {
                "core": {
                "verbose": false,
                "enabled": true,
                "rules": {
                    "no-hardcoded-env-urls": {
                    "level": "warning",
                    "disallowedhosts": [
                        "azuredatalakeanalytics.net",
                        "azuredatalakestore.net",
                        "batch.core.windows.net",
                        "core.windows.net",
                        "database.windows.net",
                        "datalake.azure.net",
                        "gallery.azure.com",
                        "graph.windows.net",
                        "login.microsoftonline.com",
                        "management.azure.com",
                        "management.core.windows.net",
                        "vault.azure.net"
                    ],
                    "excludedhosts": [
                        "schema.management.azure.com"
                    ]
                    }
                }
                }
            },
            "experimentalFeaturesEnabled": {
                "symbolicNameCodegen": false,
                "extensibility": false,
                "extendableParamFiles": true,
                "resourceTypedParamsAndOutputs": false,
                "sourceMapping": false,
                "legacyFormatter": false,
                "testFramework": false,
                "assertions": false,
                "waitAndRetry": false,
                "localDeploy": false,
                "resourceInfoCodegen": false,
                "typedVariables": false,
                "moduleExtensionConfigs": false,
                "desiredStateConfiguration": false,
                "externalInputFunction": false,
                "onlyIfNotExists": false
            },
            "formatting": {
                "indentKind": "Space",
                "newlineKind": "LF",
                "insertFinalNewline": true,
                "indentSize": 2,
                "width": 120
            }
            }
            """);
        }

        [TestMethod]
        public void GetConfiguration_IOExceptionWhenDiscoveringConfiguration_ReturnsDefaultConfigurationWithInfoDiagnostic()
        {
            // Arrange.
            var fileSystemMock = StrictMock.Of<IFileSystem>();
            fileSystemMock.Setup(x => x.Path.GetDirectoryName(It.IsAny<string>())).Returns("foo");
            fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns("");
            fileSystemMock.Setup(x => x.Path.DirectorySeparatorChar).Returns('/');
            fileSystemMock.Setup(x => x.Path.GetFullPath(It.IsAny<string>())).Returns("/foo/bar");
            fileSystemMock.Setup(x => x.Path.GetFullPath(It.IsAny<string>(), It.IsAny<string>())).Returns("/bar/foo");
            fileSystemMock.Setup(x => x.Path.IsPathRooted(It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Throws(new IOException("Oops."));

            var fileExplorer = new FileSystemFileExplorer(fileSystemMock.Object);
            var sut = new ConfigurationManager(fileExplorer);
            var configuration = sut.GetConfiguration(new Uri("file:///foo/bar/main.bicep"));

            // Act & Assert.
            var diagnostics = configuration.Diagnostics;
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Info);
            diagnostics[0].Message.Should().Be("Error scanning \"/foo/bar/\" for bicep configuration: Oops.");
            configuration.ToUtf8Json().Should().Be(IConfigurationManager.GetBuiltInConfiguration().ToUtf8Json());
        }

        [DataTestMethod]
        [DataRow("""
    {
      "cloud": {
        "currentProfile": "MyCloud"
      }
    }
    """, @"The cloud profile ""MyCloud"" does not exist. Available profiles include ""AzureChinaCloud"", ""AzureCloud"", ""AzureUSGovernment"".")]
        [DataRow("""
    {
      "cloud": {
        "currentProfile": "MyCloud",
        "profiles": {
          "MyCloud": {
          }
        }
      }
    }
    """, @"The cloud profile ""MyCloud"" is invalid. The ""resourceManagerEndpoint"" property cannot be null or undefined.")]
        [DataRow("""
    {
      "cloud": {
        "currentProfile": "MyCloud",
        "profiles": {
          "MyCloud": {
            "resourceManagerEndpoint": "Not and URL"
          }
        }
      }
    }
    """, @"The cloud profile ""MyCloud"" is invalid. The value of the ""resourceManagerEndpoint"" property ""Not and URL"" is not a valid URL.")]
        [DataRow("""
    {
      "cloud": {
        "currentProfile": "MyCloud",
        "profiles": {
          "MyCloud": {
            "resourceManagerEndpoint": "https://example.invalid"
          }
        }
      }
    }
    """, @"The cloud profile ""MyCloud"" is invalid. The ""activeDirectoryAuthority"" property cannot be null or undefined.")]
        [DataRow("""
    {
      "cloud": {
        "currentProfile": "MyCloud",
        "profiles": {
          "MyCloud": {
            "resourceManagerEndpoint": "https://example.invalid",
            "activeDirectoryAuthority": "Not an URL"
          }
        }
      }
    }
    """, @"The cloud profile ""MyCloud"" is invalid. The value of the ""activeDirectoryAuthority"" property ""Not an URL"" is not a valid URL.")]
        public void GetConfiguration_InvalidCurrentCloudProfile_PropagatesConfigurationDiagnostic(string configurationContents, string expectedExceptionMessage)
        {
            // Arrange.
            var configurationPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurationPath] = configurationContents,
            });

            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            var diagnostics = sut.GetConfiguration(sourceFileUri).Diagnostics;
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().Be($"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\": {expectedExceptionMessage}");
        }

        [TestMethod]
        [DataRow("""
    {
      "cloud": {
        "credentialOptions": {
            "managedIdentity": {
                "type": "UserAssigned"
            }
        }
      }
    }
    """, @"The managed-identity configuration is invalid. Either ""clientId"" or ""resourceId"" must be set for user-assigned identity.")]
        [DataRow("""
    {
      "cloud": {
        "credentialOptions": {
            "managedIdentity": {
                "type": "UserAssigned",
                "clientId": "foo",
                "resourceId": "bar"
            }
        }
      }
    }
    """, @"The managed-identity configuration is invalid. ""clientId"" and ""resourceId"" cannot be set at the same time for user-assigned identity.")]
        [DataRow("""
    {
      "cloud": {
        "credentialOptions": {
            "managedIdentity": {
                "type": "UserAssigned",
                "clientId": "foo"
            }
        }
      }
    }
    """, @"The managed-identity configuration is invalid. ""clientId"" must be a GUID.")]
        [DataRow("""
    {
      "cloud": {
        "credentialOptions": {
            "managedIdentity": {
                "type": "UserAssigned",
                "resourceId": "bar"
            }
        }
      }
    }
    """, @"The managed-identity configuration is invalid. ""resourceId"" must be a valid Azure resource identifier.")]
        public void GetConfiguration_InvalidUserAssignedIdentityOptions_PropagatesConfigurationDiagnostic(string configurationContents, string expectedExceptionMessage)
        {
            // Arrange.
            var configurationPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurationPath] = configurationContents,
            });
            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act.
            var diagnostics = sut.GetConfiguration(sourceFileUri).Diagnostics;

            // Assert.
            diagnostics.Length.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().Be($"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\": {expectedExceptionMessage}");
        }

        [DataTestMethod]
        [DataRow("repo")]
        [DataRow("re%20po")]
        public void GetConfiguration_ValidCustomConfiguration_OverridesBuiltInConfiguration(string root)
        {
            // Arrange.
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [CreatePath(root)] = new MockDirectoryData(),
                [CreatePath($"{root}/modules")] = new MockDirectoryData(),
                [CreatePath($"{root}/bicepconfig.json")] = /*lang=json,strict*/ """
        {
          "cloud": {
            "currentProfile": "MyCloud",
            "profiles": {
              "MyCloud": {
                "resourceManagerEndpoint": "https://bicep.example.com",
                "activeDirectoryAuthority": "https://login.bicep.example.com"
              }
            },
            "credentialPrecedence": [
                "AzurePowerShell",
                "VisualStudioCode"
            ],
            "credentialOptions": {
              "managedIdentity": {
                "type": "UserAssigned",
                "clientId": "00000000-0000-0000-0000-000000000000"
              }
            }
          },
          "moduleAliases": {
            "ts": {
              "mySpecPath": {
                "subscription": "B34C8680-F688-48C2-A44F-E1EFF5E01173"
              }
            },
            "br": {
              "myRegistry": {
                "registry": "localhost:8000"
              },
              "myModulePath": {
                "registry": "test.invalid",
                "modulePath": "root/modules"
              }
            }
          },
        "analyzers": {
        "core": {
            "enabled": false,
            "rules": {
            "no-hardcoded-env-urls": {
                "level": "warning",
                "disallowedhosts": [
                "datalake.azure.net",
                "azuredatalakestore.net",
                "azuredatalakeanalytics.net",
                "vault.azure.net",
                "asazure.windows.net",
                "batch.core.windows.net"
                ]
            }
            }
        }
        },
        "cacheRootDirectory": "/home/username/.bicep/cache",
        "experimentalFeaturesEnabled": {
        "extensibility": true
        },
        "formatting": {
        "indentKind": "Space",
        "newlineKind": "LF",
        "insertFinalNewline": true,
        "indentSize": 2,
        "width": 80
        }
    }
    """
            });
            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(this.CreatePath($"{root}/modules/vnet.bicep"));

            // Act.
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Assert.
            configuration.Should().HaveContents(/*lang=json,strict*/ """
      {
        "cloud": {
          "currentProfile": "MyCloud",
          "profiles": {
            "AzureChinaCloud": {
              "resourceManagerEndpoint": "https://management.chinacloudapi.cn",
              "activeDirectoryAuthority": "https://login.chinacloudapi.cn"
            },
            "AzureCloud": {
              "resourceManagerEndpoint": "https://management.azure.com",
              "activeDirectoryAuthority": "https://login.microsoftonline.com"
            },
            "AzureUSGovernment": {
              "resourceManagerEndpoint": "https://management.usgovcloudapi.net",
              "activeDirectoryAuthority": "https://login.microsoftonline.us"
            },
            "MyCloud": {
              "resourceManagerEndpoint": "https://bicep.example.com",
              "activeDirectoryAuthority": "https://login.bicep.example.com"
            }
          },
          "credentialPrecedence": [
            "AzurePowerShell",
            "VisualStudioCode"
          ],
          "credentialOptions": {
            "managedIdentity": {
              "type": "UserAssigned",
              "clientId": "00000000-0000-0000-0000-000000000000"
            }
          }
        },
        "moduleAliases": {
          "ts": {
            "mySpecPath": {
              "subscription": "B34C8680-F688-48C2-A44F-E1EFF5E01173"
            }
          },
          "br": {
            "myModulePath": {
              "registry": "test.invalid",
              "modulePath": "root/modules"
            },
            "myRegistry": {
              "registry": "localhost:8000"
            },
            "public": {
              "registry": "mcr.microsoft.com",
              "modulePath": "bicep"
            }
          }
        },
        "extensions": {
            "az": "builtin:",
            "kubernetes": "builtin:"
        },
        "implicitExtensions": [
            "az"
        ],
        "analyzers": {
          "core": {
            "verbose": false,
            "enabled": false,
            "rules": {
              "no-hardcoded-env-urls": {
                "level": "warning",
                "disallowedhosts": [
                  "datalake.azure.net",
                  "azuredatalakestore.net",
                  "azuredatalakeanalytics.net",
                  "vault.azure.net",
                  "asazure.windows.net",
                  "batch.core.windows.net"
                ],
                "excludedhosts": [
                  "schema.management.azure.com"
                ]
              }
            }
          }
        },
        "cacheRootDirectory": "/home/username/.bicep/cache",
        "experimentalFeaturesEnabled": {
          "extendableParamFiles": false,
          "symbolicNameCodegen": false,
          "extensibility": true,
          "resourceTypedParamsAndOutputs": false,
          "sourceMapping": false,
          "legacyFormatter": false,
          "testFramework": false,
          "assertions": false,
          "waitAndRetry": false,
          "localDeploy": false,
          "resourceInfoCodegen": false,
          "typedVariables": false,
          "moduleExtensionConfigs": false,
          "desiredStateConfiguration": false,
          "externalInputFunction": false,
          "onlyIfNotExists": false
        },
        "formatting": {
          "indentKind": "Space",
          "newlineKind": "LF",
          "insertFinalNewline": true,
          "indentSize": 2,
          "width": 80
        }
      }
      """);
        }

        [TestMethod]
        public void Bicepconfig_resolution_is_a_merge_between_closest_bicepconfig_file_and_builtin_config()
        {
            // Verifies and clarifies the resolution behavior of bicepconfig.json per documentation (https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/bicep-config):
            // > The configuration file closest to the Bicep file in the directory hierarchy is used.

            // Arrange.
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [CreatePath("repo")] = new MockDirectoryData(),
                [CreatePath("repo/bicepconfig.json")] = """
        {
          "moduleAliases": {
            "br": {
              "public": {
                "registry": "main.microsoft.com",
                "modulePath": "bicep"
              }
            }
          }
        }
        """,
                [CreatePath("repo/modules")] = new MockDirectoryData(),
                [CreatePath("repo/modules/bicepconfig.json")] = """
        {
        }
        """
            });

            var fileExplorer = new FileSystemFileExplorer(fileSystem);
            var sut = new ConfigurationManager(fileExplorer);
            var sourceFileUri = new Uri(this.CreatePath("repo/modules/vnet.bicep"));

            // Act.
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Assert.
            configuration.ModuleAliases.TryGetOciArtifactModuleAlias("public").IsSuccess(out var moduleAlias).Should().BeTrue();
            moduleAlias!.Registry.Should().Be("mcr.microsoft.com");
        }
        private string CreatePath(string path) => Path.Combine(this.TestContext.ResultsDirectory!, path.Replace('/', Path.DirectorySeparatorChar));
    }
}
