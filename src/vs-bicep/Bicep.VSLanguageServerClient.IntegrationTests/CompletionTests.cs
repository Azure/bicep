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

            // This is a hack to wait for language service activation. If this is the first test, it may
            // take long for the compilation to complete. Subsequent tests should be faster.
            ColorizationsUtility.WaitForColorizations(editor);

            editor.Caret.MoveToEndOfFile();

            string[] expectedCompletionTexts = new string[] { "module", "output", "param", "resource", "targetScope", "var" };

            CompletionsUtility.VerifyCompletions(editor, expectedCompletionTexts);
        }
    }
}
