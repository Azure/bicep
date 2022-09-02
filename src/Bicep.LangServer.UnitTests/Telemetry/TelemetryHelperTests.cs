// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Telemetry;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Telemetry
{
    public class TelemetryHelperTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly LinterRulesProvider LinterRulesProvide = new();

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithOverallLinterStateChange_ShouldReturnTelemetryEvent()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(1);

            var telemetryEvent = telemetryEvents.First();
            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterCoreEnabledStateChange);

            var properties = new Dictionary<string, string>
            {
                { "previousState", "true" },
                { "currentState", "false" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithNoStateChange_ShouldDoNothing()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithNoEnabledSectionInCurrentConfigurationAndPreviousSettingIsFalse_ShouldFireTelemetryEvent()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(1);

            var telemetryEvent = telemetryEvents.First();
            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterCoreEnabledStateChange);

            var properties = new Dictionary<string, string>
            {
                { "previousState", "false" },
                { "currentState", "true" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithNoEnabledSectionInPreviousConfigurationAndCurrentSettingIsFalse_ShouldFireTelemetryEvent()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(1);

            var telemetryEvent = telemetryEvents.First();
            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterCoreEnabledStateChange);

            var properties = new Dictionary<string, string>
            {
                { "previousState", "true" },
                { "currentState", "false" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithNoEnabledSectionInCurrentConfigurationAndPreviousSettingIsTrue_ShouldDoNothing()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithNoEnabledSectionInPreviousConfigurationAndCurrentSettingIsTrue_ShouldDoNothing()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithEmptyCurrentConfig_ShouldUseDefaultSettingsAndFireTelemetryEvent()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{}";

            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(1);

            var telemetryEvent = telemetryEvents.First();
            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterCoreEnabledStateChange);

            var properties = new Dictionary<string, string>
            {
                { "previousState", "false" },
                { "currentState", "true" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithEmptyPreviousConfig_ShouldUseDefaultSettingsAndFireTelemetryEvent()
        {
            var prevBicepConfigFileContents = @"{}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";

            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(1);

            var telemetryEvent = telemetryEvents.First();
            telemetryEvent.EventName.Should().Be(TelemetryConstants.EventNames.LinterCoreEnabledStateChange);

            var properties = new Dictionary<string, string>
            {
                { "previousState", "true" },
                { "currentState", "false" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithEmptyCurrentAndPreviousConfig_ShouldDoNothing()
        {
            var prevBicepConfigFileContents = "{}";
            var curBicepConfigFileContents = "{}";

            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithOverallLinterSettingDisabledInBothPreviousAndCurrentConfig_ShouldDoNothing()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";

            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Should().BeEmpty();
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_WithMismatchInRuleSettingsInPreviousAndCurrentConfig_ShouldFireTelemetryEvent()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        },
        ""no-unused-vars"": {
          ""level"": ""info""
        }
      }
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""warning""
        },
        ""no-unused-vars"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";

            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(2);

            var telemetryEvent = telemetryEvents.First(x => x.Properties is not null && x.Properties["rule"] == "no-unused-params");
            telemetryEvent.EventName!.Should().Be(TelemetryConstants.EventNames.LinterRuleStateChange);

            var properties = new Dictionary<string, string>
            {
                { "rule", "no-unused-params" },
                { "previousDiagnosticLevel", "info" },
                { "currentDiagnosticLevel", "warning" }
            };

            telemetryEvent.Properties.Should().Equal(properties);

            telemetryEvent = telemetryEvents.First(x => x.Properties is not null && x.Properties["rule"] == "no-unused-vars");
            telemetryEvent.EventName!.Should().Be(TelemetryConstants.EventNames.LinterRuleStateChange);

            properties = new Dictionary<string, string>
            {
                { "rule", "no-unused-vars" },
                { "previousDiagnosticLevel", "info" },
                { "currentDiagnosticLevel", "warning" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        [TestMethod]
        public void GetTelemetryEventsForBicepConfigChange_VerifyDefaultRuleSettingsAreUsed()
        {
            var prevBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true
    }
  }
}";
            var curBicepConfigFileContents = @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""error""
        },
        ""no-unused-vars"": {
          ""level"": ""error""
        }
      }
    }
  }
}";
            (RootConfiguration prevConfiguration, RootConfiguration curConfiguration) = GetPreviousAndCurrentRootConfiguration(prevBicepConfigFileContents, curBicepConfigFileContents);

            var telemetryEvents = TelemetryHelper.GetTelemetryEventsForBicepConfigChange(prevConfiguration, curConfiguration, LinterRulesProvide);

            telemetryEvents.Count().Should().Be(2);

            var telemetryEvent = telemetryEvents.First(x => x.Properties is not null && x.Properties["rule"] == "no-unused-params");
            telemetryEvent.EventName!.Should().Be(TelemetryConstants.EventNames.LinterRuleStateChange);

            var properties = new Dictionary<string, string>
            {
                { "rule", "no-unused-params" },
                { "previousDiagnosticLevel", "warning" },
                { "currentDiagnosticLevel", "error" }
            };

            telemetryEvent.Properties.Should().Equal(properties);

            telemetryEvent = telemetryEvents.First(x => x.Properties is not null && x.Properties["rule"] == "no-unused-vars");
            telemetryEvent.EventName!.Should().Be(TelemetryConstants.EventNames.LinterRuleStateChange);

            properties = new Dictionary<string, string>
            {
                { "rule", "no-unused-vars" },
                { "previousDiagnosticLevel", "warning" },
                { "currentDiagnosticLevel", "error" }
            };

            telemetryEvent.Properties.Should().Equal(properties);
        }

        private (RootConfiguration, RootConfiguration) GetPreviousAndCurrentRootConfiguration(string prevBicepConfigContents, string curBicepConfigContents)
        {
            var configurationManager = new ConfigurationManager(new IOFileSystem());
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var prevConfiguration = GetRootConfiguration(testOutputPath, prevBicepConfigContents, configurationManager);
            var curConfiguration = GetRootConfiguration(testOutputPath, curBicepConfigContents, configurationManager);

            return (prevConfiguration, curConfiguration);
        }

        private RootConfiguration GetRootConfiguration(string testOutputPath, string bicepConfigContents, ConfigurationManager configurationManager)
        {
            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", bicepConfigContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath);

            return configurationManager.GetConfiguration(bicepConfigUri.ToUri());
        }
    }
}
