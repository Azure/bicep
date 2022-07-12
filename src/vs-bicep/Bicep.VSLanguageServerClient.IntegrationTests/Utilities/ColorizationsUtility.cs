// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public static void WaitForClassifications(
                IVisualStudioTextEditorTestExtension editor,
                string baselineFile,
                Func<IEnumerable<Classification>>? getActualClassifications = null)
        {
            string expected = File.ReadAllText(baselineFile);

            if (getActualClassifications is null)
            {
                getActualClassifications = editor.Classification.GetAllClassifications;
            }

            string actual = GetClassificationsContent(editor, getActualClassifications);

            WaitForExtensions.AreEqual(
                () => GetClassificationsContent(editor, getActualClassifications),
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

        private static string GetClassificationsContent(
                IVisualStudioTextEditorTestExtension editor,
                Func<IEnumerable<Classification>> getClassifications)
        {
            StringBuilder sb = new();

            foreach (Classification c in getClassifications())
            {
                string classificationName = c.Name.Replace(" - (TRANSIENT)", string.Empty);

                sb.Append("[Name: ");
                sb.Append(classificationName);
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
