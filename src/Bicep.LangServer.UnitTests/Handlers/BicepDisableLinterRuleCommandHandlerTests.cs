// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Bicep.Core;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepDisableLinterRuleCommandHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly MockRepository Repository = new(MockBehavior.Strict);
        private static readonly ISerializer Serializer = Repository.Create<ISerializer>().Object;
        private BicepDisableLinterRuleCommandHandler BicepDisableLinterRuleHandler = new(Serializer);

        [TestMethod]
        public void DisableLinterRule_WithInvalidBicepConfig_ShouldThrow()
        {
            string bicepConfig = @"{
              ""analyzers"": {
                ""core"": {
                  ""verbose"": false,
                  ""enabled"": true,
                  ""rules"": {
                    ""no-unused-params"": {
                      ""level"": ""warning""
            }";

            Action disableLinterRule = () => BicepDisableLinterRuleHandler.DisableLinterRule(bicepConfig, "no-unused-params");

            disableLinterRule.Should().Throw<Exception>().WithMessage("File bicepconfig.json already exists and is invalid. If overwriting the file is intended, delete it manually and retry disable linter rule lightBulb option again");
        }

        [TestMethod]
        public void DisableLinterRule_WithRuleEnabledInBicepConfig_ShouldTurnOffRule()
        {
            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";
            string actual = BicepDisableLinterRuleHandler.DisableLinterRule(bicepConfigFileContents, "no-unused-params");

            actual.Should().BeEquivalentToIgnoringNewlines(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void DisableLinterRule_WithRuleDisabledInBicepConfig_DoesNothing()
        {
            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}";
            string actual = BicepDisableLinterRuleHandler.DisableLinterRule(bicepConfigFileContents, "no-unused-params");

            actual.Should().BeEquivalentToIgnoringNewlines(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void DisableLinterRule_WithNoRuleInBicepConfig_ShouldAddAnEntryInBicepConfig()
        {
            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
      }
    }
  }
}";
            string actual = BicepDisableLinterRuleHandler.DisableLinterRule(bicepConfigFileContents, "no-unused-params");

            actual.Should().BeEquivalentToIgnoringNewlines(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void DisableLinterRule_WithNoLevelPropertyInRule_ShouldAddAnEntryInBicepConfigAndTurnOffRule()
        {
            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
        }
      }
    }
  }
}";
            string actual = BicepDisableLinterRuleHandler.DisableLinterRule(bicepConfigFileContents, "no-unused-params");

            actual.Should().BeEquivalentToIgnoringNewlines(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void DisableLinterRule_WithNoRulesNode_ShouldAddAnEntryInBicepConfigAndTurnOffRule()
        {
            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true
    }
  }
}";
            string actual = BicepDisableLinterRuleHandler.DisableLinterRule(bicepConfigFileContents, "no-unused-params");

            actual.Should().BeEquivalentToIgnoringNewlines(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void DisableLinterRule_WithOnlyCurlyBraces_ShouldUseDefaultConfigAndTurnOffRule()
        {
            string actual = BicepDisableLinterRuleHandler.DisableLinterRule("{}", "no-unused-params");

            actual.Should().BeEquivalentToIgnoringNewlines(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net""
      }
    }
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
        },
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void GetBicepConfigFilePathAndContents_WithInvalidBicepConfigFilePath_ShouldCreateBicepConfigFileUsingDefaultSettings()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"param storageAccountName string = 'test'");
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepPath);

            (string actualBicepConfigFilePath, string actualBicepConfigContents) = BicepDisableLinterRuleHandler.GetBicepConfigFilePathAndContents(documentUri, "no-unused-params", string.Empty);

            var directoryContainingSourceFile = Path.GetDirectoryName(documentUri.GetFileSystemPath());
            string expectedBicepConfigFilePath = Path.Combine(directoryContainingSourceFile!, LanguageConstants.BicepConfigurationFileName);

            actualBicepConfigFilePath.Should().Be(expectedBicepConfigFilePath);
            actualBicepConfigContents.Should().BeEquivalentToIgnoringNewlines(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net""
      }
    }
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
        },
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void GetBicepConfigFilePathAndContents_WithNonExistentBicepConfigFile_ShouldCreateBicepConfigFileUsingDefaultSettings()
        {
            var bicepPath = FileHelper.SaveResultFile(TestContext, "main.bicep", @"param storageAccountName string = 'test'");
            DocumentUri documentUri = DocumentUri.FromFileSystemPath(bicepPath);
            (string actualBicepConfigFilePath, string actualBicepConfigContents) = BicepDisableLinterRuleHandler.GetBicepConfigFilePathAndContents(documentUri, "no-unused-params", @"\nonExistent\bicepconfig.json");

            var directoryContainingSourceFile = Path.GetDirectoryName(documentUri.GetFileSystemPath());
            string expectedBicepConfigFilePath = Path.Combine(directoryContainingSourceFile!, LanguageConstants.BicepConfigurationFileName);
            actualBicepConfigFilePath.Should().Be(expectedBicepConfigFilePath);
            actualBicepConfigContents.Should().BeEquivalentToIgnoringNewlines(@"{
  ""cloud"": {
    ""currentProfile"": ""AzureCloud"",
    ""profiles"": {
      ""AzureCloud"": {
        ""resourceManagerEndpoint"": ""https://management.azure.com""
      },
      ""AzureChinaCloud"": {
        ""resourceManagerEndpoint"": ""https://management.chinacloudapi.cn""
      },
      ""AzureUSGovernment"": {
        ""resourceManagerEndpoint"": ""https://management.usgovcloudapi.net""
      }
    }
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
        },
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }

        [TestMethod]
        public void GetBicepConfigFilePathAndContents_WithValidBicepConfigFile_ShouldReturnUpdatedBicepConfigFile()
        {
            string testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());
            string bicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";
            string bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigFileContents, testOutputPath, Encoding.UTF8);

            DocumentUri documentUri = DocumentUri.FromFileSystemPath("/path/to/main.bicep");
            (string actualBicepConfigFilePath, string actualBicepConfigContents) = BicepDisableLinterRuleHandler.GetBicepConfigFilePathAndContents(documentUri, "no-unused-params", bicepConfigFilePath);

            actualBicepConfigFilePath.Should().Be(bicepConfigFilePath);
            actualBicepConfigContents.Should().BeEquivalentToIgnoringNewlines(@"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""off""
        }
      }
    }
  }
}");
        }
    }
}
