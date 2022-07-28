// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class CompletionTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_TopLevelPlainTextDeclarations()
        {
            ProjectItemTestExtension projectItem = TestProject!["main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            WaitForBicepLanguageServiceActivation(editor);

            editor.Caret.MoveToEndOfFile();

            string[] expectedCompletionTexts = new string[] { "module", "output", "param", "resource", "targetScope", "var" };

            CompletionsUtility.VerifyCompletions(editor, expectedCompletionTexts);
        }
    }
}
