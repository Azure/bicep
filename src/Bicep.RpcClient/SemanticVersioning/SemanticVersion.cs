// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Bicep.RpcClient.SemanticVersioning;

/// <summary>
/// A Bicep release version consisting of major, minor, and patch components.
/// </summary>
internal sealed record SemanticVersion(int Major, int Minor, int Patch) : IComparable<SemanticVersion>
{
    private static readonly Regex VersionPattern = new(
        @"\A(?<major>0|[1-9][0-9]*)(?:\.(?<minor>0|[1-9][0-9]*)(?:\.(?<patch>0|[1-9][0-9]*))?)?\z",
        RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

    /// <summary>
    /// Parses a version. Omitted minor and patch components default to zero.
    /// </summary>
    public static bool TryParse(string value, out SemanticVersion? result)
    {
        result = null;
        var match = VersionPattern.Match(value);
        if (!match.Success ||
            !TryParseComponent(match.Groups["major"], out var major) ||
            !TryParseComponent(match.Groups["minor"], out var minor) ||
            !TryParseComponent(match.Groups["patch"], out var patch))
        {
            return false;
        }

        result = new(major, minor, patch);
        return true;
    }

    public static SemanticVersion Parse(string value)
        => TryParse(value, out var result) && result is not null
            ? result
            : throw new FormatException($"The provided value '{value}' is not a valid version.");

    public int CompareTo(SemanticVersion? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Major != other.Major)
        {
            return Major.CompareTo(other.Major);
        }

        return Minor != other.Minor ? Minor.CompareTo(other.Minor) : Patch.CompareTo(other.Patch);
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    private static bool TryParseComponent(Group group, out int result)
    {
        result = 0;
        return !group.Success || int.TryParse(group.Value, NumberStyles.None, CultureInfo.InvariantCulture, out result);
    }
}
