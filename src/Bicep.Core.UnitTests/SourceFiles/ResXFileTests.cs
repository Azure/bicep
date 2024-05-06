// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.UnitTests.SourceFiles
{
    /// <summary>
    /// Tests related to project source files
    /// </summary>
    [TestClass]
    public class ResXFileTests
    {
        const string Info = "If this test fails, it may indicate that the file was formatted directly in the editor instead of the Resource Editor in Visual Studio. (In VSCode or VS Mac, there is no Resource Editor, so be sure you haven't allowed the editor to format the file after editing.)";
        private string GetRelativeFileContents(string relativePath)
        {
            var path = BaselineHelper.GetAbsolutePathRelativeToRepoRoot(relativePath);
            return File.ReadAllText(path);
        }

        private void AssertDoesNotContainTabs(string relativePath, string text)
        {
            if (text.Contains('\t'))
            {
                throw new Exception($"{relativePath} should be indented with spaces, not tabs. {Info}");
            }
        }

        [TestMethod]
        public void ResXAndDesignerFilesShouldBeConsistentAndNotCauseUnnecessaryMergeConflicts()
        {
            string[] resxFiles = System.IO.Directory.GetFiles(BaselineHelper.GetAbsolutePathRelativeToRepoRoot("src"), "*.resx", SearchOption.AllDirectories)
                .Where(path => !path.ContainsOrdinally("packages"))
                .ToArray();
            resxFiles.Should().HaveCountGreaterThan(2, "There should be at least 3 ResX files found in the project");

            foreach (string resXRelativePath in resxFiles)
            {
                using (new AssertionScope(resXRelativePath))
                {
                    AssertDoesNotContainTabs(resXRelativePath, GetRelativeFileContents(resXRelativePath));
                }

                string designerRelativePath = Path.ChangeExtension(resXRelativePath, ".Designer.cs");
                using (new AssertionScope(designerRelativePath))
                {
                    string designerFileContents = GetRelativeFileContents(designerRelativePath);
                    AssertDoesNotContainTabs(designerFileContents, designerFileContents);

                    if (designerFileContents.Contains("get\n"))
                    {
                        throw new Exception($"{designerRelativePath} should be formatted with this style:\n\n  get\n  {{\n\nand not this style:\n\n  get {{\n\n{Info}");
                    }
                }
            }
        }
    }
}
