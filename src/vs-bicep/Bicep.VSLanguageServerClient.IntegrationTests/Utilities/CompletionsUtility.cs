// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public static class CompletionsUtility
    {
        public static void VerifyCompletions(IVisualStudioTextEditorTestExtension editor, string[] expectedCompletionTexts)
        {
            IVisualStudioCompletionListTestExtension? completionListTestExtension = GetCompletionList(editor, true);

            if (completionListTestExtension is null)
            {
                Assert.IsNull(completionListTestExtension, "Completion list is null");
                return;
            }

            CompletionList completionList = completionListTestExtension.Items;

            foreach (var text in expectedCompletionTexts)
            {
                Assert.IsTrue(completionList.Any(e => e.Text == text));
            }
        }

        private static IVisualStudioCompletionListTestExtension? GetCompletionList(IVisualStudioTextEditorTestExtension editor, bool invokeIfNotPresent)
        {
            IVisualStudioCompletionListTestExtension? completionList = editor.Intellisense.GetActiveCompletionList();
            if (completionList == null)
            {
                if (invokeIfNotPresent)
                {
                    editor.Intellisense.ShowCompletionList();
                    completionList = WaitForExtensions.TryIsNotNull(() => editor.Intellisense.GetActiveCompletionList(), 2000);
                }
            }

            return completionList;
        }

    }
}
