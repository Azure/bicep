// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Reflection;
using Bicep.VSLanguageServerClient.IntegrationTests.Utilities;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Solution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            ColorizationsUtility.WaitForColorizations(editor);

            string baselineFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestSolution\BicepTestProject\results\Formatting\BicepFormatting.bsl");

            FormatUtility.FormatDocument(editor, baselineFile);
        }
    }
}
