// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Bicep.Core.FileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Assertions
{
    public static class BaselineHelper
    {
        private static readonly string RepoRoot = GetRepoRoot();

        private const string SetBaseLineSettingName = "SetBaseLine";

        public static bool ShouldSetBaseline(TestContext testContext) =>
            testContext.Properties.Contains(SetBaseLineSettingName) && string.Equals(testContext.Properties[SetBaseLineSettingName] as string, bool.TrueString, StringComparison.OrdinalIgnoreCase);

        public static void SetBaseline(string actualLocation, string expectedLocation)
        {
            actualLocation = PathHelper.ResolveAndNormalizePath(actualLocation, RepoRoot);
            expectedLocation = PathHelper.ResolveAndNormalizePath(expectedLocation, RepoRoot);

            File.Copy(actualLocation, expectedLocation, overwrite: true);
        }

        private static string GetRepoRoot()
        {
            // just using PowerShell as an easy way to redirect streams
            using var ps = PowerShell.Create();
            ps.AddScript("git rev-parse --show-toplevel");

            var output = ps.Invoke();
            if (!ps.HadErrors && output.Count == 1 && output.Single().BaseObject is string path)
            {
                // normalize the path for current platform (git really likes using / on windows)
                return Path.GetFullPath(path);
            }

            throw new InvalidOperationException("Unable to determine the repo root path.");
        }
    }
}
