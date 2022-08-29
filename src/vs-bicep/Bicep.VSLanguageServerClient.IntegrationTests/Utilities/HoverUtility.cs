// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.VSLanguageServerClient.TestServices.Utilitites;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.VisualStudio;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public class HoverUtility
    {
        public static void TestQuickInfo(Func<string, bool> funcIsCorrectQuickInfo, IVisualStudioTextEditorTestExtension editor)
        {
            string lastQuickInfoText = string.Empty;

            WaitForExtensions.IsTrue(() =>
            {
                InvokeQuickInfo();

                try
                {
                    IQuickInfoTestExtension? quickInfo = WaitForExtensions.IsNotNull(() => editor.Intellisense.GetActiveQuickInfo(), 1000);

                    if (quickInfo is not null)
                    {
                        // The Text property isn't available immediately.
                        WaitForExtensions.IsTrue(() =>
                        {
                            lastQuickInfoText = quickInfo.Text;

                            return !string.IsNullOrEmpty(lastQuickInfoText);
                        }, TimeSpan.FromMilliseconds(1000));

                    return funcIsCorrectQuickInfo(quickInfo.Text);
                   }
                }
                catch
                {
                }

                return false;
            }, TimeSpan.FromMilliseconds(5000), () => "last QuickInfo text: " + lastQuickInfoText);
        }

        public static void InvokeQuickInfo()
        {
            CommandUtility.ExecuteCommand(VSConstants.VSStd2KCmdID.QUICKINFO);
        }
    }
}
