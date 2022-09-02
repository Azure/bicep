// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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

        [TestMethod]
        public void SingleWorkspaceFolder_CurrentFileUnsaved_ShouldReturnFirstWorkspaceFolder()
        {
            string[] workspaceFolders = new string[]
            {
                Environment.CurrentDirectory,
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };

            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(workspaceFolders, "Untitled-1");
            var expected = Environment.CurrentDirectory;

            actual.Should().Be(expected);
        }

        [TestMethod]
        public void NoWorkspaceFolder_CurrentFileUnsaved_ShouldReturnProfileFolder()
        {
            string[] workspaceFolders = new string[]
            {
            };

            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(workspaceFolders, "Untitled-1");
            var expected = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            actual.Should().Be(expected);
        }

        private static IEnumerable<object[]> GetWorkspaceFoldersTestData()
        {
#if WINDOWS_BUILD
            // Bicep file is in a workspace folder - return the first one that matches, or else the bicep file's folder
            yield return new object[] { new string[] { "c:\\workspace1" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1" };
            yield return new object[] { new string[] { "c:\\workspace1" }, "c:\\workspace1\\two\\bicepconfig.json", "c:\\workspace1" };
            yield return new object[] { new string[] { "c:\\workspace1" }, "c:\\workspace1\\two\\three\\bicepconfig.json", "c:\\workspace1" };
            yield return new object[] { new string[] { "c:\\workspace1\\two" }, "c:\\workspace1\\two\\three\\bicepconfig.json", "c:\\workspace1\\two" };
            yield return new object[] { new string[] { "c:\\workspace1\\two\\three" }, "c:\\workspace1\\two\\three\\bicepconfig.json", "c:\\workspace1\\two\\three" };
            yield return new object[] { new string[] { "c:\\workspace1", "c:\\workspace2" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1" };
            yield return new object[] { new string[] { "c:\\workspace2", "c:\\workspace1" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1" };
            yield return new object[] {
                new string[] { "c:\\workspace1\\two\\three\four", "c:\\workspace1\\two\\three", "c:\\workspace1\\two", "c:\\workspace1" },
                "c:\\workspace1\\two\\three\\five\\bicepconfig.json",
                "c:\\workspace1\\two\\three" };
            // Bicep file not in any workspace folder - return bicep file's folder
            yield return new object[] { new string[] { "c:\\workspace1" }, "c:\\workspace2\\bicepconfig.json", "c:\\workspace2" };
            yield return new object[] { new string[] { "c:\\workspace1\\two\\three" }, "c:\\workspace1\\two\\bicepconfig.json", "c:\\workspace1\\two" };
            // Case insensitive
            yield return new object[] { new string[] { "c:\\Workspace1" }, "c:\\workspace1\\bicepconfig.json", "c:\\Workspace1" };
            yield return new object[] { new string[] { "C:\\workspace1" }, "c:\\workspace1\\bicepconfig.json", "C:\\workspace1" };
            // Folders partially match
            yield return new object[] { new string[] { "c:\\workspace" }, "c:\\workspace1\\bicepconfig.json", "c:\\workspace1" };
            yield return new object[] { new string[] { "c:\\workspace1" }, "c:\\workspace\\bicepconfig.json", "c:\\workspace" };
#else
            // Bicep file is in a workspace folder - return the first one that matches, or else the bicep file's folder
            yield return new object[] { new string[] { "/workspace1" }, "/workspace1/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace1" }, "/workspace1/two/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace1" }, "/workspace1/two/three/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace1/two" }, "/workspace1/two/three/bicepconfig.json", "/workspace1/two" };
            yield return new object[] { new string[] { "/workspace1/two/three" }, "/workspace1/two/three/bicepconfig.json", "/workspace1/two/three" };
            yield return new object[] { new string[] { "/workspace1/two/three" }, "/workspace1/two/three/four/bicepconfig.json", "/workspace1/two/three" };
            yield return new object[] { new string[] { "/workspace1", "/workspace2" }, "/workspace1/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace2", "/workspace1" }, "/workspace1/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace1/two/three\four", "/workspace1/two/three", "/workspace1/two", "/workspace1" }, "/workspace1/two/three/five/bicepconfig.json", "/workspace1/two/three" };
            // Bicep file not in any workspace folder - return bicep file's folder
            yield return new object[] { new string[] { "/workspace1" }, "/workspace2/bicepconfig.json", "/workspace2" };
            yield return new object[] { new string[] { "/workspace1/two/three" }, "/workspace1/two/bicepconfig.json", "/workspace1/two" };
            // Case sensitive - no match, return bicep file's folder
            yield return new object[] { new string[] { "/Workspace1" }, "/workspace1/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace1" }, "/workspace1/bicepconfig.json", "/workspace1" };
            // Folders partially match
            yield return new object[] { new string[] { "/workspace" }, "/workspace1/bicepconfig.json", "/workspace1" };
            yield return new object[] { new string[] { "/workspace1" }, "/workspace/bicepconfig.json", "/workspace" };
#endif
        }

        [DataTestMethod]
        [DynamicData(nameof(GetWorkspaceFoldersTestData), DynamicDataSourceType.Method)]
        public void WorkspaceFolders(string[] workspaceFolders, string bicepFilePath, string expected)
        {
            var actual = BicepGetRecommendedConfigLocationHandler.GetRecommendedConfigFileLocation(workspaceFolders, bicepFilePath);

            actual.Should().Be(expected);
        }
    }
}
