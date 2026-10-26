// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.Common;

public static class LinterResourceTypePatterns
{
    public static readonly Regex ResourceTypeRegex = new(
        "^ [a-z]+\\.[a-z]+ (\\/ [a-z]+)+ $",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
}
