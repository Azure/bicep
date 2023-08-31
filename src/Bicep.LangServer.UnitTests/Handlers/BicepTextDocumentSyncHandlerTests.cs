// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests.Handlers;

[TestClass]
public class BicepTextDocumentSyncHandlerTests
{
    private static readonly LinterRulesProvider linterRulesProvider = new();

    [TestMethod]
    public async Task ChangingLinterRuleDiagnosticLevel_ShouldFireTelemetryEvent()
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
          ""level"": ""off""
        }
      }
    }
  }
}";
        var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
        await ChangeLinterRuleState(telemetryProvider, prevBicepConfigFileContents, curBicepConfigFileContents);

        var properties = new Dictionary<string, string>
            {
                { "rule", "no-unused-params" },
                { "previousDiagnosticLevel", "info" },
                { "currentDiagnosticLevel", "off" }
            };

        telemetryProvider.Verify(m => m.PostEvent(It.Is<BicepTelemetryEvent>(
            p => p.EventName == TelemetryConstants.EventNames.LinterRuleStateChange &&
            p.Properties != null &&
            p.Properties.Count == properties.Count && p.Properties.SequenceEqual(properties))), Times.Exactly(1));
    }

    [TestMethod]
    public async Task ChangingOverallLinterState_ShouldFireTelemetryEvent()
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
        var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
        await ChangeLinterRuleState(telemetryProvider, prevBicepConfigFileContents, curBicepConfigFileContents);

        var properties = new Dictionary<string, string>
            {
                { "previousState", "true" },
                { "currentState", "false" }
            };

        telemetryProvider.Verify(m => m.PostEvent(It.Is<BicepTelemetryEvent>(
            p => p.EventName == TelemetryConstants.EventNames.LinterCoreEnabledStateChange &&
            p.Properties != null &&
            p.Properties.Count == properties.Count && p.Properties.SequenceEqual(properties))), Times.Exactly(1));
    }

    [TestMethod]
    public async Task ChangingLinterRuleDiagnosticLevel_ToDefaultValue_ShouldNotFireTelemetryEvent()
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
        },
        ""no-unused-vars"": {
          ""level"": ""warning""
        }
      }
    }
  }
}";
        var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
        await ChangeLinterRuleState(telemetryProvider, prevBicepConfigFileContents, curBicepConfigFileContents);

        telemetryProvider.Verify(m => m.PostEvent(It.IsAny<BicepTelemetryEvent>()), Times.Never);
    }

    [TestMethod]
    public async Task ChangingLinterRuleDiagnosticLevel_WithOverallStateSetToFalse_ShouldNotFireTelemetryEvent()
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
        var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
        await ChangeLinterRuleState(telemetryProvider, prevBicepConfigFileContents, curBicepConfigFileContents);

        telemetryProvider.Verify(m => m.PostEvent(It.IsAny<BicepTelemetryEvent>()), Times.Never);
    }

    private async Task ChangeLinterRuleState(Mock<ITelemetryProvider> telemetryProvider, string prevBicepConfigFileContents, string curBicepConfigFileContents)
    {
        var fileSystem = new MockFileSystem();
        fileSystem.File.WriteAllText("/bicepconfig.json", prevBicepConfigFileContents);
        var bicepConfigUri = new Uri("file:///bicepconfig.json");

        var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepConfigUri, prevBicepConfigFileContents);
        var bicepConfigChangeHandler = new BicepConfigChangeHandler(compilationManager,
                                                                    new ConfigurationManager(fileSystem),
                                                                    linterRulesProvider,
                                                                    telemetryProvider.Object,
                                                                    new Workspace());

        var bicepTextDocumentSyncHandler = new BicepTextDocumentSyncHandler(compilationManager, bicepConfigChangeHandler);

        await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepConfigUri, prevBicepConfigFileContents, 1), CancellationToken.None);

        fileSystem.File.WriteAllText("/bicepconfig.json", curBicepConfigFileContents);

        await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidSaveTextDocumentParams(bicepConfigUri, curBicepConfigFileContents, 2), CancellationToken.None);
    }
}