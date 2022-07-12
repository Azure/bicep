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
    public class ColorizationTests : VisualStudioBicepHostTest
    {
        [TestMethod]
        public void Verify_Classifications()
        {
            ProjectItemTestExtension projectItem = TestProject!["main.bicep"];
            IVisualStudioTextEditorTestExtension editor = projectItem.GetDocumentAsTextEditor().Editor;

            // This is a hack to wait for language service activation. If this is the first test, it may
            // take long for the compilation to complete. Subsequent tests should be faster.
            ColorizationsUtility.WaitForColorizations(editor);

            string baselineFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestSolution\BicepTestProject\results\Colorization\BicepClassifications.bsl");

            ColorizationsUtility.WaitForClassifications(editor, baselineFile);
        }
    }
}
