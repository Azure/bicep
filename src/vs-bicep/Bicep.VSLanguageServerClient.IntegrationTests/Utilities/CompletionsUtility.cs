// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Linq;
using Bicep.VSLanguageServerClient.TestServices.Utilitites;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public static class CompletionsUtility
    {
        public static void VerifyCompletions(IVisualStudioTextEditorTestExtension editor, string[] expectedCompletionTexts)
        {
            IVisualStudioCompletionListTestExtension? completionListTestExtension = GetCompletionList(editor);

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

        private static IVisualStudioCompletionListTestExtension? GetCompletionList(IVisualStudioTextEditorTestExtension editor)
        {
            IVisualStudioCompletionListTestExtension? completionList = editor.Intellisense.GetActiveCompletionList();

            if (completionList is null)
            {
                try
                {
                    editor.Intellisense.InvokeCompletionList();
                }
                catch
                {
                    // This can throw when the list was coming up when we were invoking it. This is an issue coming from apex assembly
                    // that we use in tests. There's no product issue here.
                }

                completionList = WaitForExtensions.TryIsNotNull(() => editor.Intellisense.GetActiveCompletionList(), 2000);
            }

            return completionList;
        }

    }
}
