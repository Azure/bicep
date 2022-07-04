// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
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
        public const string BaselineTestCategory = "Baseline";

        public static bool ShouldSetBaseline(TestContext testContext) =>
            testContext.Properties.Contains(SetBaseLineSettingName) && string.Equals(testContext.Properties[SetBaseLineSettingName] as string, bool.TrueString, StringComparison.OrdinalIgnoreCase);

        public static void SetBaseline(string actualLocation, string expectedLocation)
        {
            actualLocation = GetAbsolutePathRelativeToRepoRoot(actualLocation);
            expectedLocation = GetAbsolutePathRelativeToRepoRoot(expectedLocation);

            if (Path.GetDirectoryName(expectedLocation) is { } parentDir &&
                !Directory.Exists(parentDir))
            {
                Directory.CreateDirectory(parentDir);
            }

            File.Copy(actualLocation, expectedLocation, overwrite: true);
        }

        public static string GetAbsolutePathRelativeToRepoRoot(string path)
            => PathHelper.ResolveAndNormalizePath(path, RepoRoot);

        private static string GetRepoRoot()
        {
            var currentDir = new DirectoryInfo(Environment.CurrentDirectory);

            while (currentDir.Parent is {} parentDir)
            {
                // search upwards for the .git directory. This should only exist at the repository root.
                if (Directory.Exists(Path.Join(currentDir.FullName, ".git")))
                {
                    return currentDir.FullName;
                }

                currentDir = parentDir;
            }

            throw new InvalidOperationException($"Unable to determine the repo root path from directory {Environment.CurrentDirectory}");
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
Baseline {2} has been updated.
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
    dotnet test --filter ""TestCategory=Baseline"" -- 'TestRunParameters.Parameter(name=\""SetBaseLine\"", value=\""true\"")'

See https://github.com/Azure/bicep/blob/main/CONTRIBUTING.md#updating-test-baselines for more information on how to fix this error.
");
                }
                else
                {
                    output.Append(@"
Overwrite the single baseline:
    cp {1} {2}

Overwrite all baselines:
    dotnet test --filter ""TestCategory=Baseline"" -- 'TestRunParameters.Parameter(name=""SetBaseLine"", value=""true"")'

See https://github.com/Azure/bicep/blob/main/CONTRIBUTING.md#updating-test-baselines for more information on how to fix this error.
");
                }
            }

            return output.ToString();
        }
    }
}
