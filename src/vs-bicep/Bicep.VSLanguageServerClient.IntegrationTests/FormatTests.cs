// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class FormatTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_Formatting()
        {
            ProjectItemTestExtension projectItem = TestProject!["main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            ColorizationsUtility.WaitForColorizations(editor);

            editor.Caret.MoveToLine(3);

            string[] expectedCompletionTexts = new string[] { "module", "output", "param", "resource", "targetScope", "var" };

            CompletionsUtility.VerifyCompletions(editor, expectedCompletionTexts);
        }
    }
}
