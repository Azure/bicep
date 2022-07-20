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

            WaitForExtensions.AreEqual(
                () => editor.Contents,
                expected,
                TimeSpan.FromSeconds(5),
                onTimeout: actual =>
                {
                    string resultFile = Path.ChangeExtension(baselineFile, ".new");
                    File.WriteAllText(resultFile, actual);

                    string message = string.Format("\r\nInvalid data in the log!\r\nwindiff \"{0}\" \"{1}\"\r\n", baselineFile, resultFile);

                    throw new Exception(message);
                });
        }
    }
}
