// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Testing.Baselines;

internal static class BaselineUpdate
{
    private const string SetBaselineSettingName = "SetBaseLine";

    public static bool IsEnabled(TestContext testContext) =>
        testContext.Properties.Contains(SetBaselineSettingName) &&
        string.Equals(testContext.Properties[SetBaselineSettingName] as string, bool.TrueString, StringComparison.OrdinalIgnoreCase);

    public static void Apply(string actualPath, string expectedPath)
    {
        actualPath = TestRepository.GetAbsolutePath(actualPath);
        expectedPath = TestRepository.GetAbsolutePath(expectedPath);

        if (Path.GetDirectoryName(expectedPath) is { } parentDirectory)
        {
            Directory.CreateDirectory(parentDirectory);
        }

        File.Copy(actualPath, expectedPath, overwrite: true);
    }

    public static string GetFailureMessage(bool wasApplied)
    {
        var output = new StringBuilder();

        output.Append(@"
Found diffs between actual and expected:
{0}
");

        if (wasApplied)
        {
            output.Append(@"
Baseline {2} has been updated.
");
        }
        else
        {
            output.Append(@"
View this diff with:
    git diff --color-words --no-index {2} {1}
");

            output.Append(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? @"
Overwrite the single baseline:
    xcopy /yq {1} {2}
" : @"
Overwrite the single baseline:
    cp {1} {2}
");

            output.Append(@"
Overwrite all baselines:
    dotnet test -- --filter ""TestCategory=Baseline"" --test-parameter SetBaseLine=true

See https://github.com/Azure/bicep/blob/main/CONTRIBUTING.md#updating-test-baselines for more information on how to fix this error.
");
        }

        return output.ToString();
    }
}
