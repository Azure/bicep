// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Reflection;
using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Bicep.VSLanguageServerClient.Settings;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Settings;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Bicep.VSLanguageServerClient.IntegrationTests
{
    [TestClass]
    public class FormatTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_Formatting()
        {
            ProjectItemTestExtension projectItem = TestProject![@"Formatting\main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            WaitForBicepLanguageServiceActivation(editor);
            var settingsService = VsHostUtility.VsHost!.ObjectModel.Settings;

            // Save settings before test execution
            var indentSize = settingsService.GetSetting(TestConstants.TextEditor, BicepLanguageServerClientConstants.LanguageName, TestConstants.IndentSizeKey);
            var insertTabs = settingsService.GetSetting(TestConstants.TextEditor, BicepLanguageServerClientConstants.LanguageName, TestConstants.InsertTabsKey);
            var tabSize = settingsService.GetSetting(TestConstants.TextEditor, BicepLanguageServerClientConstants.LanguageName, TestConstants.TabSizeKey);

            try
            {
                // Verify formatting with spaces
                UpdateSettings(settingsService, insertTabs: false, indentSize: 2, tabSize: 2);
                string baselineFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestSolution\BicepTestProject\results\Formatting\BicepFormattingWithSpaces.bsl");

                FormatUtility.VerifyDocumentFormatting(editor, baselineFile);

                // Verify formatting with spaces
                UpdateSettings(settingsService, insertTabs: true, indentSize: 2, tabSize: 2);
                baselineFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestSolution\BicepTestProject\results\Formatting\BicepFormattingWithTabs.bsl");

                FormatUtility.VerifyDocumentFormatting(editor, baselineFile);
            }
            finally
            {
                // Reset settings
                UpdateSettings(settingsService, insertTabs, indentSize, tabSize);
            }
        }

        private void UpdateSettings(SettingsService settingsService, object insertTabs, object indentSize, object tabSize)
        {
            settingsService.SetSetting(TestConstants.TextEditor, BicepLanguageServerClientConstants.LanguageName, TestConstants.IndentSizeKey, indentSize);
            settingsService.SetSetting(TestConstants.TextEditor, BicepLanguageServerClientConstants.LanguageName, TestConstants.InsertTabsKey, insertTabs);
            settingsService.SetSetting(TestConstants.TextEditor, BicepLanguageServerClientConstants.LanguageName, TestConstants.TabSizeKey, tabSize);
        }
    }
}
