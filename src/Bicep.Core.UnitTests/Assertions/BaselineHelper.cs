// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;
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
            actualLocation = GetAbsolutePathRelativeToRepoRoot(actualLocation);
            expectedLocation = GetAbsolutePathRelativeToRepoRoot(expectedLocation);

            File.Copy(actualLocation, expectedLocation, overwrite: true);
        }

        public static string GetAbsolutePathRelativeToRepoRoot(string path)
            => PathHelper.ResolveAndNormalizePath(path, RepoRoot);

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
        
        public static string GetAssertionFormatString(bool isBaselineUpdate)
        {
            var output = new StringBuilder();
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            output.Append(@"
Found diffs between actual and expected:
{0}
");

            if (isBaselineUpdate)
            {
                output.Append(@"
Baseline has been updated.
");
            }
            else
            {
                output.Append(@"
View this diff with:
    git diff --color-words --no-index {1} {2}
");

                if (isWindows)
                {
                    output.Append(@"
Overwrite the single baseline:
    xcopy /yq {1} {2}

Overwrite all baselines:
    dotnet test -- 'TestRunParameters.Parameter(name=\""SetBaseLine\"", value=\""true\"")'
");
                }
                else
                {
                    output.Append(@"
Overwrite the single baseline:
    cp {1} {2}

Overwrite all baselines:
    dotnet test -- 'TestRunParameters.Parameter(name=""SetBaseLine"", value=""true"")'
");
                }
            }

            return output.ToString();
        }
    }
}
