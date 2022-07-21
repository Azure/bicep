// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.VisualStudio;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public class FormatUtility
    {
        public static void VerifyDocumentFormatting(IVisualStudioTextEditorTestExtension editor, string baselineFile)
        {
            string expected = File.ReadAllText(baselineFile);

            VsHostUtility.VsHost!.ObjectModel.Commanding.ExecuteCommand(VSConstants.VSStd2KCmdID.FORMATDOCUMENT, null);

            WaitForExtensions.IsTrue(() => editor.Contents == expected, TimeSpan.FromSeconds(5));
        }
    }
}
