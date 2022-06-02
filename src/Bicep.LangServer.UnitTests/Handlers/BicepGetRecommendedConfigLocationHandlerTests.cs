// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Handlers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Handlers
{
    [TestClass]
    public class BicepGetRecommendedConfigLocationTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [TestMethod]
        public void NoWorkspace_NoBicepFile()
        {
            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation((string[]?)null, null);
            var expected = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void NullWorkspaceArray_ShouldBeInBicepFolder()
        {
            var bicepFilePath = Path.Join(Environment.CurrentDirectory, "main.bicep");

            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation((string[]?)null, bicepFilePath);
            var expected = Environment.CurrentDirectory;

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void EmptyWorkspaceArray_ShouldBeInBicepFolder()
        {
            var bicepFilePath = Path.Join(Environment.CurrentDirectory, "main.bicep");

            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(new string[] { }, bicepFilePath);
            var expected = Environment.CurrentDirectory;

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void SingleWorkspaceFolder_NoBicepFile_ShouldReturnFirstWorkspaceFolder()
        {
            string[] workspaceFolders = new string[]
            {
                Environment.CurrentDirectory
            };

            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(workspaceFolders, null);
            var expected = workspaceFolders[0];

            actual.Should().Be(expected);
        }

        // Bicep file is in a workspace folder - return the first one that matches, or else the bicep file's folder
        [DataRow(new string[] { "c:\\workspace1" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1")]
        [DataRow(new string[] { "c:\\workspace1" }, "c:\\workspace1\\two\\bicepconfig.json", "c:\\workspace1")]
        [DataRow(new string[] { "c:\\workspace1" }, "c:\\workspace1\\two\\three\\bicepconfig.json", "c:\\workspace1")]
        [DataRow(new string[] { "c:\\workspace1\\two" }, "c:\\workspace1\\two\\three\\bicepconfig.json", "c:\\workspace1\\two")]
        [DataRow(new string[] { "c:\\workspace1\\two\\three" }, "c:\\workspace1\\two\\three\\bicepconfig.json", "c:\\workspace1\\two\\three")]
        [DataRow(new string[] { "c:\\workspace1", "c:\\workspace2" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1")]
        [DataRow(new string[] { "c:\\workspace2", "c:\\workspace1" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1")]
        [DataRow(
            new string[] { "c:\\workspace1\\two\\three\four", "c:\\workspace1\\two\\three", "c:\\workspace1\\two", "c:\\workspace1" },
            "c:\\workspace1\\two\\three\\five\\bicepconfig.json",
            "c:\\workspace1\\two\\three")]
        // Bicep file not in any workspace folder - return bicep file's folder
        [DataRow(new string[] { "c:\\workspace1" }, "c:\\workspace2\\bicepconfig.json", "c:\\workspace2")]
        [DataRow(new string[] { "c:\\workspace1\\two\\three" }, "c:\\workspace1\\two\\bicepconfig.json", "c:\\workspace1\\two")]
        // Case insensitive
        [DataRow(new string[] { "c:\\Workspace1" }, "c:\\workspace1\\bicepconfig.json", "c:\\Workspace1")]
        [DataRow(new string[] { "C:\\workspace1" }, "c:\\workspace1\\bicepconfig.json", "C:\\workspace1")]
        // Folders partially match
        [DataRow(new string[] { "c:\\workspace" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1")]
        [DataRow(new string[] { "c:\\workspace1" }, "c:\\workspace\\bicepconfig.json", "c:\\workspace")]
        // Unsaved (or otherwise non-absolute) path
        [DataRow(new string[] { "c:\\workspace1" }, "Untitled1.bicep", "c:\\workspace1")]
        [DataRow(new string[] { "c:\\workspace1" }, "folder\\Untitled1.bicep", "c:\\workspace1")]
#if !WINDOWS_BUILD
        [Ignore]
#endif
        [DataTestMethod]
        public void WorkspaceFolders_Windows(string[] workspaceFolders, string bicepFilePath, string expected)
        {
            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(workspaceFolders, bicepFilePath);

            actual.Should().Be(expected);
        }

        // Bicep file is in a workspace folder - return the first one that matches, or else the bicep file's folder
        [DataRow(new string[] { "/workspace1" }, "/workspace1/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace1" }, "/workspace1/two/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace1" }, "/workspace1/two/three/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace1/two" }, "/workspace1/two/three/bicepconfig.json", "/workspace1/two")]
        [DataRow(new string[] { "/workspace1/two/three" }, "/workspace1/two/three/bicepconfig.json", "/workspace1/two/three")]
        [DataRow(new string[] { "/workspace1/two/three" }, "/workspace1/two/three/four/bicepconfig.json", "/workspace1/two/three")]
        [DataRow(new string[] { "/workspace1", "/workspace2" }, "/workspace1/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace2", "/workspace1" }, "/workspace1/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace1/two/three\four", "/workspace1/two/three", "/workspace1/two", "/workspace1" }, "/workspace1/two/three/five/bicepconfig.json", "/workspace1/two/three")]
        // Bicep file not in any workspace folder - return bicep file's folder
        [DataRow(new string[] { "/workspace1" }, "/workspace2/bicepconfig.json", "/workspace2")]
        [DataRow(new string[] { "/workspace1/two/three" }, "/workspace1/two/bicepconfig.json", "/workspace1/two")]
        // Case sensitive - no match, return bicep file's folder
        [DataRow(new string[] { "/Workspace1" }, "/workspace1/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace1" }, "/workspace1/bicepconfig.json", "/workspace1")]
        // Folders partially match
        [DataRow(new string[] { "/workspace" }, "/workspace1/bicepconfig.json", "/workspace1")]
        [DataRow(new string[] { "/workspace1" }, "/workspace/bicepconfig.json", "/workspace")]
        // Unsaved (or otherwise non-absolute) path
        [DataRow(new string[] { "/workspace1" }, "Untitled1.bicep", "/workspace1")]
        [DataRow(new string[] { "/workspace1" }, "folder/Untitled1.bicep", "/workspace1")]
#if WINDOWS_BUILD
        [Ignore]
#endif
        [DataTestMethod]
        public void WorkspaceFolders_MacLinux(string[] workspaceFolders, string bicepFilePath, string expected)
        {
            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(workspaceFolders, bicepFilePath);

            actual.Should().Be(expected);
        }

    }
}
