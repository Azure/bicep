// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Linq;
using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Shell;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class ValidationTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_Diagnostics()
        {
            ProjectItemTestExtension projectItem = TestProject!["main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            WaitForBicepLanguageServiceActivation(editor);

            editor.Caret.MoveToEndOfFile();
            editor.KeyboardCommands.Type("resource");

            var errorListService = VsHostUtility.VsHost!.ObjectModel.Shell.ToolWindows.ErrorList;
            errorListService.FilterScope = ErrorListFilterScope.CurrentDocument;
            errorListService.TryWaitForErrorListItems(ErrorListErrorLevel.All, waitTimeout: TimeSpan.FromSeconds(30));
            ErrorItemTestExtension[] diagnostics = errorListService.AllItems;

            Assert.IsNotNull(diagnostics);
            Assert.AreEqual(3, diagnostics.Length);

            Assert.IsTrue(diagnostics.Any(
                d => d.Description.Contains("Expected a resource identifier at this location.") &&
                d.ErrorLevel == ErrorListErrorLevel.Error &&
                d.LineNumber == 3 &&
                d.Column == 9));
            Assert.IsTrue(diagnostics.Any(
                d => d.Description.Contains(@"The resource type is not valid. Specify a valid resource type of format ""<types>@<apiVersion>"".") &&
                d.ErrorLevel == ErrorListErrorLevel.Error &&
                d.LineNumber == 3 &&
                d.Column == 9));
            Assert.IsTrue(diagnostics.Any(
                d => d.Description.Contains(@"Parameter ""resourceId"" is declared but never used.") &&
                d.ErrorLevel == ErrorListErrorLevel.Warning &&
                d.LineNumber == 2 &&
                d.Column == 7));
        }
    }
}
