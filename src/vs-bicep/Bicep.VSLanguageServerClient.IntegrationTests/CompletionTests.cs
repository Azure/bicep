// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Test.Apex.Editor;
using System;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Test.Apex.Services;
using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;

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

                ColorizationsUtility.WaitForColorizations(editor);

                editor.Caret.MoveToLine(3);
                editor.KeyboardCommands.Type("res");
                CompletionList? items = WaitForCompletionItems(editor, 5000);
                Assert.IsNotNull(items, "Time out waiting for the version completion list");
            }
        }

        public CompletionList? WaitForCompletionItems(IVisualStudioTextEditorTestExtension editor, int timeout = 1000)
        {
            CompletionList? items = null;

            WaitFor.TryIsTrue(() =>
            {
                try
                {
                    IVisualStudioCompletionListTestExtension completionList = editor.Intellisense.GetActiveCompletionList();

                    // Make another call if completion list is not available or is still loading.
                    if (completionList == null)
                    {
                        return false;
                    }

                    items = completionList.Items;

                    return true;
                }
                catch (EditorException)
                {
                    return false;
                }
            }, TimeSpan.FromMilliseconds(timeout), TimeSpan.FromMilliseconds(500));

            return items;
        }
    }
}
