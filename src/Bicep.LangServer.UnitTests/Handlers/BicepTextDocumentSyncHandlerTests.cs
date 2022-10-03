// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bicep.Core.Analyzers.Linter;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Workspaces;
using Bicep.LangServer.IntegrationTests.Helpers;
using Bicep.LanguageServer.Configuration;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using IOFileSystem = System.IO.Abstractions.FileSystem;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test methods do not need to follow this convention.")]
    public class BicepTextDocumentSyncHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private static readonly LinterRulesProvider linterRulesProvider = new();

        [TestMethod]
        [Ignore("Disabled due to flakiness - https://github.com/Azure/bicep/issues/7370")]
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
        [Ignore("Disabled due to flakiness - https://github.com/Azure/bicep/issues/7370")]
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
        [Ignore("Disabled due to flakiness - https://github.com/Azure/bicep/issues/7370")]
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
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", prevBicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepConfigUri, prevBicepConfigFileContents);
            var bicepConfigChangeHandler = new BicepConfigChangeHandler(compilationManager,
                                                                        new ConfigurationManager(new IOFileSystem()),
                                                                        linterRulesProvider,
                                                                        telemetryProvider.Object,
                                                                        new Workspace());

            var bicepTextDocumentSyncHandler = new BicepTextDocumentSyncHandler(compilationManager, bicepConfigChangeHandler);

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepConfigUri, prevBicepConfigFileContents, 1), CancellationToken.None);

            bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", curBicepConfigFileContents, testOutputPath);
            bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidSaveTextDocumentParams(bicepConfigUri, curBicepConfigFileContents, 2), CancellationToken.None);
        }
    }
}
