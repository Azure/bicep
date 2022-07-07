// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.ObjectModel;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.Services;
using Microsoft.Test.Apex.VisualStudio.Editor;

namespace Bicep.VSLanguageServerClient.IntegrationTests.Utilities
{
    public class ColorizationsUtility
    {
        public static ReadOnlyCollection<Classification> GetClassifications(IVisualStudioTextEditorTestExtension editor)
        {
            WaitFor.TryIsTrue((() => editor.Classification.GetAllClassifications().Count > 0), TimeSpan.FromSeconds(30));

            return editor.Classification.GetAllClassifications();
        }

        public static void WaitForColorizations(IVisualStudioTextEditorTestExtension editor)
        {
            WaitFor.TryIsTrue((() => editor.Classification.GetAllClassifications().Count > 0), TimeSpan.FromSeconds(30));
        }
    }
}
