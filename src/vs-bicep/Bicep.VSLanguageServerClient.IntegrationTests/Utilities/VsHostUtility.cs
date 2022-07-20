// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.Test.Apex.VisualStudio.Shell;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public class VsHostUtility
    {
        public static VisualStudioHost? VsHost;

        public static TextEditorDocumentWindowTestExtension? OpenFile(string fileRelativePath)
        {
            string filePath = Path.Combine(Path.GetDirectoryName(typeof(VsHostUtility).Assembly.Location), fileRelativePath);

            if (VsHost is null)
            {
                throw new Exception("VsHost is null");
            }

            VsHost.ObjectModel.Commanding.ExecuteCommand("File.OpenFile", "\"" + filePath + "\"");
            return GetActiveTextEditorDocument();
        }

        public static TextEditorDocumentWindowTestExtension? GetActiveTextEditorDocument()
        {
            if (VsHost is null)
            {
                throw new Exception("VsHost is null");
            }

            return VsHost.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor;
        }
    }
}
