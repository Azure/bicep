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

            WaitForBicepLanguageServiceActivation(editor);

            string baselineFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TestSolution\BicepTestProject\results\Colorization\BicepClassifications.bsl");

            ColorizationsUtility.TestClassifications(editor, baselineFile);
        }
    }
}
