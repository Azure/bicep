// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class CompletionTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_PlaintextCompletion()
        {
            if (TestProject is not null)
            {
                ProjectItemTestExtension projectItem = TestProject[@"Completions\PlaintextCompletions.bicep"];
                IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;
            }
        }
    }
}
