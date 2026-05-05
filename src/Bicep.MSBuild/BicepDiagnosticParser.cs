// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;

namespace Azure.Bicep.MSBuild;

public record BicepDiagnostic(string Origin, string Category, string Code, string Text);

/// <summary>
/// Parses and reconstructs Bicep diagnostics in canonical MSBuild format.
/// </summary>
/// <remarks>This class does not handle the full msbuild canonical diagnostic format. It also extends the msbuild diagnostic format to support info levels.</remarks>
public static class BicepDiagnosticParser
{
    private static readonly Regex DiagnosticFormatRegex = new(@"^(?<origin>(((\d+>)?\s*[a-zA-Z]?:[^:]*)|([^:]*))) : (?<category>error|warning|info) (?<code>[^: ]+): (?<text>.+)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    public static BicepDiagnostic? TryParseDiagnostic(string line)
    {
        var match = DiagnosticFormatRegex.Match(line);
        if (!match.Success)
        {
            return null;
        }
        return new BicepDiagnostic(
            match.Groups["origin"].Value,
            match.Groups["category"].Value,
            match.Groups["code"].Value,
            match.Groups["text"].Value);
    }

    public static string ReconstructDiagnostic(BicepDiagnostic diagnostic)
        => ReconstructDiagnostic(diagnostic.Origin, diagnostic.Category, diagnostic.Code, diagnostic.Text);

    public static string ReconstructDiagnostic(string origin, string category, string code, string text)
        => $"{origin} : {category} {code}: {text}";
}
