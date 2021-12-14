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
    public class BicepTextDocumentSyncHandlerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

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
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", prevBicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepConfigUri, prevBicepConfigFileContents);
            var bicepConfigChangeHandler = new BicepConfigChangeHandler(new ConfigurationManager(new IOFileSystem()),
                                                                        compilationManager,
                                                                        new Workspace(),
                                                                        telemetryProvider.Object,
                                                                        linterRulesProvider);

            var bicepTextDocumentSyncHandler = new BicepTextDocumentSyncHandler(compilationManager, bicepConfigChangeHandler);

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepConfigUri, prevBicepConfigFileContents, 1), CancellationToken.None);

            bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", curBicepConfigFileContents, testOutputPath);
            bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidSaveTextDocumentParams(bicepConfigUri, curBicepConfigFileContents, 2), CancellationToken.None);

            var properties = new Dictionary<string, string>
            {
                { "rule", "no-unused-params" },
                { "previousDiagnosticLevel", "info" },
                { "currentDiagnosticLevel", "off" }
            };

            var bicepTelemetryEvent = BicepTelemetryEvent.CreateLinterRuleStateChangeInBicepConfig("no-unused-params", "info", "off");

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
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", prevBicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepConfigUri, prevBicepConfigFileContents);
            var bicepConfigChangeHandler = new BicepConfigChangeHandler(new ConfigurationManager(new IOFileSystem()),
                                                                        compilationManager,
                                                                        new Workspace(),
                                                                        telemetryProvider.Object,
                                                                        linterRulesProvider);

            var bicepTextDocumentSyncHandler = new BicepTextDocumentSyncHandler(compilationManager, bicepConfigChangeHandler);

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepConfigUri, prevBicepConfigFileContents, 1), CancellationToken.None);

            bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", curBicepConfigFileContents, testOutputPath);
            bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidSaveTextDocumentParams(bicepConfigUri, curBicepConfigFileContents, 2), CancellationToken.None);

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
      ""enabled"": false,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""info""
        },
        ""no-unused-vars"": {
          ""level"": ""warning""
        },
      }
    }
  }
}";
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", prevBicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepConfigUri, prevBicepConfigFileContents);
            var bicepConfigChangeHandler = new BicepConfigChangeHandler(new ConfigurationManager(new IOFileSystem()),
                                                                        compilationManager,
                                                                        new Workspace(),
                                                                        telemetryProvider.Object,
                                                                        linterRulesProvider);

            var bicepTextDocumentSyncHandler = new BicepTextDocumentSyncHandler(compilationManager, bicepConfigChangeHandler);

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepConfigUri, prevBicepConfigFileContents, 1), CancellationToken.None);

            bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", curBicepConfigFileContents, testOutputPath);
            bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidSaveTextDocumentParams(bicepConfigUri, curBicepConfigFileContents, 2), CancellationToken.None);

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
            var testOutputPath = Path.Combine(TestContext.ResultsDirectory, Guid.NewGuid().ToString());

            var bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", prevBicepConfigFileContents, testOutputPath);
            var bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            var telemetryProvider = BicepTestConstants.CreateMockTelemetryProvider();
            var compilationManager = BicepCompilationManagerHelper.CreateCompilationManager(bicepConfigUri, prevBicepConfigFileContents);
            var bicepConfigChangeHandler = new BicepConfigChangeHandler(new ConfigurationManager(new IOFileSystem()),
                                                                        compilationManager,
                                                                        new Workspace(),
                                                                        telemetryProvider.Object,
                                                                        linterRulesProvider);

            var bicepTextDocumentSyncHandler = new BicepTextDocumentSyncHandler(compilationManager, bicepConfigChangeHandler);

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidOpenDocumentParams(bicepConfigUri, prevBicepConfigFileContents, 1), CancellationToken.None);

            bicepConfigFilePath = FileHelper.SaveResultFile(TestContext, "bicepconfig.json", curBicepConfigFileContents, testOutputPath);
            bicepConfigUri = DocumentUri.FromFileSystemPath(bicepConfigFilePath).ToUri();

            await bicepTextDocumentSyncHandler.Handle(TextDocumentParamHelper.CreateDidSaveTextDocumentParams(bicepConfigUri, curBicepConfigFileContents, 2), CancellationToken.None);

            telemetryProvider.Verify(m => m.PostEvent(It.IsAny<BicepTelemetryEvent>()), Times.Never);
        }
    }
}
