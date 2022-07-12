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
        public static IVisualStudioCompletionListTestExtension? GetCompletionList(IVisualStudioTextEditorTestExtension editor, bool invokeIfNotPresent)
        {
            return GetCompletionList(editor, invokeIfNotPresent, ignoreExceptions: true);
        }

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

        private static IVisualStudioCompletionListTestExtension? GetCompletionList(IVisualStudioTextEditorTestExtension editor, bool invokeIfNotPresent, bool ignoreExceptions)
        {
            IVisualStudioCompletionListTestExtension? completionList = null;
            try
            {
                completionList = editor.Intellisense.GetActiveCompletionList();
                if (completionList == null)
                {
                    if (invokeIfNotPresent)
                    {
                        try
                        {
                            editor.Intellisense.ShowCompletionList();
                        }
                        catch
                        {
                            // This can throw when the list was coming up when we were invoking it
                            if (!ignoreExceptions)
                            {
                                throw;
                            }
                        }

                        completionList = WaitForExtensions.TryIsNotNull(() => editor.Intellisense.GetActiveCompletionList(), 2000);
                    }
                }
            }
            catch
            {
                // This can occur if completion is dismissed between the call to IsCompletionListPresent and accessing the completion
                // list items.
                if (!ignoreExceptions)
                {
                    throw;
                }
            }

            return completionList;
        }

    }
}
