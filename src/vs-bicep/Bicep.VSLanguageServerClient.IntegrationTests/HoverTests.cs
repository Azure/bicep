// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class HoverTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_HoverInformation()
        {
            ProjectItemTestExtension projectItem = TestProject!["main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            WaitForBicepLanguageServiceActivation(editor);

            editor.Caret.MoveToExpression("description");
            string expected = "function description(text: string): any";
            HoverUtility.TestQuickInfo(text => expected == text, editor);
        }
    }
}
