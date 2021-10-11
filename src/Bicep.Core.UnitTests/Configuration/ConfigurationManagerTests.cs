// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Configuration;
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
            var sut = new ConfigurationManager(new IOFileSystem());

            // Act.
            var configuration = sut.GetBuiltInConfiguration();

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
    ""br"": {}
  },
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-hardcoded-env-urls"": {
          ""level"": ""warning"",
          ""disallowedhosts"": [
            ""gallery.azure.com"",
            ""management.core.windows.net"",
            ""management.azure.com"",
            ""database.windows.net"",
            ""core.windows.net"",
            ""login.microsoftonline.com"",
            ""graph.windows.net"",
            ""trafficmanager.net"",
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
  }
}");
        }

        [TestMethod]
        public void GetBuiltInConfiguration_DisableAnalyzers_ReturnsBuiltInConfigurationWithoutAnalyzerSettings()
        {
            // Arrange.
            var sut = new ConfigurationManager(new IOFileSystem());

            // Act.
            var configuration = sut.GetBuiltInConfiguration(disableAnalyzers: true);

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
    ""br"": {}
  },
  ""analyzers"": {}
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
            configuration.Should().BeSameAs(sut.GetBuiltInConfiguration());
        }

        [TestMethod]
        public void GetConfiguration_InvalidCustomConfiguration_ThrowsFailedToParseConfigurationException()
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
            FluentActions.Invoking(() => sut.GetConfiguration(sourceFileUri)).Should()
                .Throw<ConfigurationException>()
                .WithMessage($"Failed to parse the contents of the Bicep configuration file \"{configurataionPath}\" as valid JSON: \"The input does not contain any JSON tokens. Expected the input to start with a valid JSON token, when isFinalBlock is true. LineNumber: 0 | BytePositionInLine: 0.\".");
        }

        [TestMethod]
        public void GetConfiguration_ConfigurationFileNotReadable_ThrowsCouldNotLoadConfigurationException()
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
            FluentActions.Invoking(() => sut.GetConfiguration(sourceFileUri)).Should()
                .Throw<ConfigurationException>()
                .WithMessage($"Could not load the Bicep configuration file \"{configurataionPath}\": \"Not allowed.\".");
        }

        [TestMethod]
        public void GetConfiguration_IOExceptionWhenDiscovringConfiguration_ReturnsDefaultConfiguration()
        {
            // Arrange.
            var fileSystemMock = StrictMock.Of<IFileSystem>();
            fileSystemMock.Setup(x => x.Path.GetDirectoryName(It.IsAny<string>())).Returns("foo");
            fileSystemMock.Setup(x => x.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns("");
            fileSystemMock.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);
            fileSystemMock.Setup(x => x.Directory.GetParent(It.IsAny<string>())).Throws(new IOException("Oops."));

            var sut = new ConfigurationManager(fileSystemMock.Object);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));
            var configuration = sut.GetConfiguration(sourceFileUri);

            // Act & Assert.
            configuration.Should().BeSameAs(sut.GetBuiltInConfiguration());
        }

        [DataTestMethod]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud""
  }
}", "The cloud profile \"MyCloud\" does not exist in the Bicep configuration \"*\". Available profiles include \"AzureChinaCloud\", \"AzureCloud\", \"AzureUSGovernment\".")]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
      }
    }
  }
}", "The cloud profile \"MyCloud\" in the Bicep configuration \"*\". The \"resourceManagerEndpoint\" property cannot be null or undefined.")]
        [DataRow(@"{
  ""cloud"": {
    ""currentProfile"": ""MyCloud"",
    ""profiles"": {
      ""MyCloud"": {
        ""resourceManagerEndpoint"": ""Not and URL""
      }
    }
  }
}", "The cloud profile \"MyCloud\" in the Bicep configuration \"*\" is invalid. The value of the \"resourceManagerEndpoint\" property \"Not and URL\" is not a valid URL.")]
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
            "The cloud profile \"MyCloud\" in the Bicep configuration \"*\". The \"activeDirectoryAuthority\" property cannot be null or undefined.")]
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
}", "The cloud profile \"MyCloud\" in the Bicep configuration \"*\" is invalid. The value of the \"activeDirectoryAuthority\" property \"Not an URL\" is not a valid URL.")]
        public void GetConfiguration_InvalidCurrentCloudProfile_ThrowsConfigurationException(string configurationContents, string expectedExceptionMessage)
        {
            // Arrange.
            var configurataionPath = CreatePath("path/to/bicepconfig.json");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [configurataionPath] = configurationContents,
            });

            var sut = new ConfigurationManager(fileSystem);
            var sourceFileUri = new Uri(CreatePath("path/to/main.bicep"));

            // Act & Assert.
            FluentActions.Invoking(() => sut.GetConfiguration(sourceFileUri)).Should()
                .Throw<ConfigurationException>()
                .WithMessage(expectedExceptionMessage);
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
  }
}");
        }

        private string CreatePath(string path) => Path.Combine(this.TestContext.ResultsDirectory, path.Replace('/', Path.DirectorySeparatorChar));
    }
}
