// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Shell;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class BicepConfigTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void ChangeInLinterRuleLevelInBicepConfigFile_ShouldUpdateDiagnostics()
        {
            ProjectItemTestExtension projectItem = TestProject![@"Validation\main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            WaitForBicepLanguageServiceActivation(editor);

            var errorListService = VsHostUtility.VsHost!.ObjectModel.Shell.ToolWindows.ErrorList;
            errorListService.FilterScope = ErrorListFilterScope.CurrentDocument;
            errorListService.TryWaitForErrorListItems(ErrorListErrorLevel.All, waitTimeout: TimeSpan.FromSeconds(30));

            ErrorItemTestExtension[] diagnostics = errorListService.AllItems;

            Assert.IsNotNull(diagnostics);
            Assert.AreEqual(1, diagnostics.Length);

            Assert.IsTrue(diagnostics.Any(
                d => d.Description.Contains("Parameter \"cosmosDBDatabaseName\" is declared but never used.") &&
                d.ErrorLevel == ErrorListErrorLevel.Warning &&
                d.LineNumber == 1 &&
                d.Column == 7));

            CreateBicepConfigFile(projectItem.FullPath, @"{
  ""analyzers"": {
    ""core"": {
      ""verbose"": false,
      ""enabled"": true,
      ""rules"": {
        ""no-unused-params"": {
          ""level"": ""error""
        }
      }
    }
  }
}");
            errorListService.TryWaitForErrorListItems(ErrorListErrorLevel.All, waitTimeout:TimeSpan.FromSeconds(30));

            // Re trigger diagnostics
            diagnostics = errorListService.AllItems;

            Assert.IsNotNull(diagnostics);
            Assert.AreEqual(1, diagnostics.Length);

            Assert.IsTrue(diagnostics.Any(
                d => d.Description.Contains("Parameter \"cosmosDBDatabaseName\" is declared but never used.") &&
                d.ErrorLevel == ErrorListErrorLevel.Error &&
                d.LineNumber == 1 &&
                d.Column == 7));
        }

        private void CreateBicepConfigFile(string bicepFilePath, string bicepConfigFileContents)
        {
            var directory = Path.GetDirectoryName(bicepFilePath);
            var bicepConfigFilePath = Path.Combine(directory, "bicepconfig.json");
            File.WriteAllText(bicepConfigFilePath, bicepConfigFileContents);
        }
    }
}
