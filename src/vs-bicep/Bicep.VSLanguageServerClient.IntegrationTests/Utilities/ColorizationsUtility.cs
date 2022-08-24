// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bicep.VSLanguageServerClient.TestServices.Utilitites;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.Services;
using Microsoft.Test.Apex.VisualStudio.Editor;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public class ColorizationsUtility
    {
        public static void WaitForColorizations(IVisualStudioTextEditorTestExtension editor)
        {
            WaitFor.TryIsTrue((() => editor.Classification.GetAllClassifications().Count > 0), TimeSpan.FromSeconds(30));
        }

        public static void TestClassifications(
                IVisualStudioTextEditorTestExtension editor,
                string baselineFile,
                Func<IEnumerable<Classification>>? getActualClassifications = null)
        {
            string expected = File.ReadAllText(baselineFile);

            if (getActualClassifications is null)
            {
                getActualClassifications = editor.Classification.GetAllClassifications;
            }

            WaitForExtensions.IsTrue(
                () => GetClassificationsContent(editor, getActualClassifications) == expected,
                TimeSpan.FromSeconds(10));
        }

        private static string GetClassificationsContent(
                IVisualStudioTextEditorTestExtension editor,
                Func<IEnumerable<Classification>> getClassifications)
        {
            StringBuilder sb = new();

            foreach (Classification c in getClassifications())
            {
                sb.Append("[Name: ");
                sb.Append(c.Name);
                sb.Append(", Text: \"");
                sb.Append(editor.GetText(c.Span));
                sb.Append("\", Start: ");
                sb.Append(c.Span.Start);
                sb.Append("]\r\n");
            }

            return sb.ToString();
        }
    }
}
