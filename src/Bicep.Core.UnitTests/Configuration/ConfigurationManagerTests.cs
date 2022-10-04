// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using IOFileSystem = System.IO.Abstractions.FileSystem;

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
            configuration.Should().HaveContents(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {
      ""public"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""api.loganalytics.io"",
            ""asazure.windows.net"",
            ""azuredatalakeanalytics.net"",
            ""azuredatalakestore.net"",
            ""batch.core.windows.net"",
            ""core.windows.net"",
            ""database.windows.net"",
            ""datalake.azure.net"",
            ""gallery.azure.com"",
            ""graph.windows.net"",
            ""login.microsoftonline.com"",
            ""management.azure.com"",
            ""management.core.windows.net"",
            ""region.asazure.windows.net"",
            ""trafficmanager.net"",
            ""vault.azure.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        }
      }
    }
  },
  ""experimentalFeaturesEnabled"": {
    ""symbolicNameCodegen"": null,
    ""imports"": null,
    ""resourceTypedParamsAndOutputs"": null,
    ""sourceMapping"": null,
    ""paramsFiles"": null
  }
}");
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
            configuration.Should().HaveContents(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {
      ""public"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep""
      }
    }
  },
  ""analyzers"": {},
  ""experimentalFeaturesEnabled"": {
    ""symbolicNameCodegen"": null,
    ""imports"": null,
    ""resourceTypedParamsAndOutputs"": null,
    ""sourceMapping"": null,
    ""paramsFiles"": null
  }
}");
        }

        [TestMethod]
        public void GetBuiltInConfiguration_DisableAnalyzers_ReturnsBuiltInConfiguration_WithSomeAnalyzersSetToLevelOff()
        {
            // Arrange.
            var configuration = IConfigurationManager.GetBuiltInConfiguration().WithAnalyzersDisabled("no-hardcoded-env-urls", "no-unused-vars");

            // Assert.
            configuration.Should().HaveContents(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      }
    },
    ""credentialPrecedence"": [
      ""AzureCLI"",
      ""AzurePowerShell""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {},
    ""br"": {
      ""public"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""off"",
          ""disallowedhosts"": [
            ""api.loganalytics.io"",
            ""asazure.windows.net"",
            ""azuredatalakeanalytics.net"",
            ""azuredatalakestore.net"",
            ""batch.core.windows.net"",
            ""core.windows.net"",
            ""database.windows.net"",
            ""datalake.azure.net"",
            ""gallery.azure.com"",
            ""graph.windows.net"",
            ""login.microsoftonline.com"",
            ""management.azure.com"",
            ""management.core.windows.net"",
            ""region.asazure.windows.net"",
            ""trafficmanager.net"",
            ""vault.azure.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        },
        ""no-unused-vars"": {
          ""level"": ""off""
        }
      }
    }
  },
  ""experimentalFeaturesEnabled"": {
    ""symbolicNameCodegen"": null,
    ""imports"": null,
    ""resourceTypedParamsAndOutputs"": null,
    ""sourceMapping"": null,
    ""paramsFiles"": null
  }
}");
        }

        [TestMethod]
        public void GetConfiguration_CustomConfigurationNotFound_ReturnsBuiltInConfiguration()
        {
            // Arrange.
            var sut = new ConfigurationManager(new IOFileSystem());
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
            var configurataionPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurataionPath] = "",
            });

            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            var diagnostics = sut.GetConfiguration(sourceFileUri).DiagnosticBuilders.Select(b => b(DiagnosticBuilder.ForDocumentStart())).ToList();
            diagnostics.Count.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().Be($"Failed to parse the contents of the Bicep configuration file \"{configurataionPath}\" as valid JSON: \"The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.\".");
        }

        [TestMethod]
        public void GetConfiguration_ConfigurationFileNotReadable_PropagatesCouldNotLoadConfigurationDiagnostic()
        {
            // Arrange.
            var configurataionPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurataionPath] = "",
            });

            var fileSystemMock = StrictMock.Of<IFileSystem>();
            fileSystemMock.SetupGet(x => x.Path).Returns(fileSystem.Path);
            fileSystemMock.SetupGet(x => x.Directory).Returns(fileSystem.Directory);
            fileSystemMock.SetupGet(x => x.File).Returns(fileSystem.File);
            fileSystemMock.Setup(x => x.FileStream.Create(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                .Throws(new UnauthorizedAccessException("Not allowed."));

            var sut = new ConfigurationManager(fileSystemMock.Object);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            var diagnostics = sut.GetConfiguration(sourceFileUri).DiagnosticBuilders.Select(b => b(DiagnosticBuilder.ForDocumentStart())).ToList();
            diagnostics.Count.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().Be($"Could not load the Bicep configuration file \"{configurataionPath}\": \"Not allowed.\".");
        }

        [TestMethod]
        public void GetConfiguration_IOExceptionWhenDiscovringConfiguration_ReturnsDefaultConfigurationWithInfoDiagnostic()
        {
            // Arrange.
            var fileSystemMock = StrictMock.Of<IFileSystem>();
            fileSystemMock.Setup(x => x.Path.GetDirectoryName(It.IsAny<string>())).Returns("foo");
            fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns("");
            fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(x => x.Directory.GetParent(It.IsAny<string>())).Throws(new IOException("Oops."));

            var sut = new ConfigurationManager(fileSystemMock.Object);
            var configurataionPath = CreatePath("path/to/main.bicep");
            var sourceFileUri = new Uri(configurataionPath);
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Act & Assert.
            var diagnostics = configuration.DiagnosticBuilders.Select(b => b(DiagnosticBuilder.ForDocumentStart())).ToList();
            diagnostics.Count.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Info);
            diagnostics[0].Message.Should().Be("Error scanning \"foo\" for bicep configuration: \"Oops.\".");
            configuration.ToUtf8Json().Should().Be(IConfigurationManager.GetBuiltInConfiguration().ToUtf8Json());
        }

        [DataTestMethod]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud""
  }
}", "The cloud profile \"MyCloud\" does not exist in the Bicep configuration \"__CONFIGURATION_PATH__\". Available profiles include \"AzureChinaCloud\", \"AzureCloud\", \"AzureUSGovernment\".")]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
      }
    }
  }
}", "The cloud profile \"MyCloud\" in the Bicep configuration \"__CONFIGURATION_PATH__\". The \"resourceManagerEndpoint\" property cannot be null or undefined.")]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""Not and URL""
      }
    }
  }
}", "The cloud profile \"MyCloud\" in the Bicep configuration \"__CONFIGURATION_PATH__\" is invalid. The value of the \"resourceManagerEndpoint\" property \"Not and URL\" is not a valid URL.")]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""https://example.invalid""
      }
    }
  }
}",
            "The cloud profile \"MyCloud\" in the Bicep configuration \"__CONFIGURATION_PATH__\". The \"activeDirectoryAuthority\" property cannot be null or undefined.")]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""https://example.invalid"",
        ""activeDirectoryAuthority"": ""Not an URL""
      }
    }
  }
}", "The cloud profile \"MyCloud\" in the Bicep configuration \"__CONFIGURATION_PATH__\" is invalid. The value of the \"activeDirectoryAuthority\" property \"Not an URL\" is not a valid URL.")]
        public void GetConfiguration_InvalidCurrentCloudProfile_PropagatesConfigurationDiagnostic(string configurationContents, string expectedExceptionMessage)
        {
            // Arrange.
            var configurationPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurationPath] = configurationContents,
            });

            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            var diagnostics = sut.GetConfiguration(sourceFileUri).DiagnosticBuilders.Select(b => b(DiagnosticBuilder.ForDocumentStart())).ToList();
            diagnostics.Count.Should().Be(1);
            diagnostics[0].Level.Should().Be(DiagnosticLevel.Error);
            diagnostics[0].Message.Should().Be($"Failed to parse the contents of the Bicep configuration file \"{configurationPath}\": \"{expectedExceptionMessage.Replace("__CONFIGURATION_PATH__", configurationPath)}\".");
        }

        [TestMethod]
        public void GetConfiguration_ValidCustomConfiguration_OverridesBuiltInConfiguration()
        {
            // Arrange.
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [CreatePath("repo")] = new MockDirectoryData(),
                [CreatePath("repo/modules")] = new MockDirectoryData(),
                [CreatePath("repo/bicepconfig.json")] = @"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""https://bicep.example.com"",
        ""activeDirectoryAuthority"": ""https://login.bicep.example.com""
      }
    },
    ""credentialPrecedence"": [
        ""AzurePowerShell"",
        ""VisualStudioCode""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {
      ""mySpecPath"": {
        ""subscription"": ""B34C8680-F688-48C2-A44F-E1EFF5E01173""
      }
    },
    ""br"": {
      ""myRegistry"": {
        ""registry"": ""localhost:8000""
      },
      ""myModulePath"": {
        ""registry"": ""test.invalid"",
        ""modulePath"": ""root/modules""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""enabled"": false,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""datalake.azure.net"",
            ""azuredatalakestore.net"",
            ""azuredatalakeanalytics.net"",
            ""vault.azure.net"",
            ""api.loganalytics.io"",
            ""asazure.windows.net"",
            ""region.asazure.windows.net"",
            ""batch.core.windows.net""
          ]
        }
      }
    }
  },
  ""cacheRootDirectory"": ""/home/username/.bicep/cache"",
  ""experimentalFeaturesEnabled"": {
    ""imports"": true
  }
}"
            });

            // Act.
            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(this.CreatePath("repo/modules/vnet.bicep"));
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Assert.
            configuration.Should().HaveContents(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn"",
        ""activeDirectoryAuthority"": ""https://login.chinacloudapi.cn""
      },
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.com""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net"",
        ""activeDirectoryAuthority"": ""https://login.microsoftonline.us""
      },
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""https://bicep.example.com"",
        ""activeDirectoryAuthority"": ""https://login.bicep.example.com""
      }
    },
    ""credentialPrecedence"": [
      ""AzurePowerShell"",
      ""VisualStudioCode""
    ]
  },
  ""moduleAliases"": {
    ""ts"": {
      ""mySpecPath"": {
        ""subscription"": ""B34C8680-F688-48C2-A44F-E1EFF5E01173"",
        ""resourceGroup"": null
      }
    },
    ""br"": {
      ""myModulePath"": {
        ""registry"": ""test.invalid"",
        ""modulePath"": ""root/modules""
      },
      ""myRegistry"": {
        ""registry"": ""localhost:8000"",
        ""modulePath"": null
      },
      ""public"": {
        ""registry"": ""mcr.microsoft.com"",
        ""modulePath"": ""bicep""
      }
    }
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""datalake.azure.net"",
            ""azuredatalakestore.net"",
            ""azuredatalakeanalytics.net"",
            ""vault.azure.net"",
            ""api.loganalytics.io"",
            ""asazure.windows.net"",
            ""region.asazure.windows.net"",
            ""batch.core.windows.net""
          ],
          ""excludedhosts"": [
            ""schema.management.azure.com""
          ]
        }
      }
    }
  },
  ""cacheRootDirectory"": ""/home/username/.bicep/cache"",
  ""experimentalFeaturesEnabled"": {
    ""symbolicNameCodegen"": null,
    ""imports"": true,
    ""resourceTypedParamsAndOutputs"": null,
    ""sourceMapping"": null,
    ""paramsFiles"": null
  }
}");
        }

        private string CreatePath(string path) => Path.Combine(this.TestContext.ResultsDirectory, path.Replace('/', Path.DirectorySeparatorChar));
    }
}
