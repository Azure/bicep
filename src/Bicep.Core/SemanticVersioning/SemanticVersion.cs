// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bicep.Core.SemanticVersioning;

/// <summary>
/// A version made up of a major, minor and patch number, as published for Bicep releases.
/// </summary>
public sealed partial record SemanticVersion(int Major, int Minor, int Patch) : IComparable<SemanticVersion>
{
    /// <summary>
    /// Parses a version, where the minor and patch components may be omitted ("1.2" is equivalent to "1.2.0", and
    /// "1" to "1.0.0").
    /// </summary>
    public static bool TryParse(string value, [NotNullWhen(true)] out SemanticVersion? result)
    {
        result = null;

        if (VersionPattern().Match(value) is not { Success: true } match)
        {
            return false;
        }

        // A component that is present but too large to represent fails to parse rather than silently wrapping.
        if (!TryParseComponent(match.Groups["major"], out var major) ||
            !TryParseComponent(match.Groups["minor"], out var minor) ||
            !TryParseComponent(match.Groups["patch"], out var patch))
        {
            return false;
        }

        result = new SemanticVersion(major, minor, patch);
        return true;
    }

    public static SemanticVersion Parse(string value)
        => TryParse(value, out var result)
            ? result
            : throw new FormatException($"The provided value '{value}' is not a valid version.");

    /// <summary>
    /// Orders two versions by major, then minor, then patch. Returns a negative number when this version is lower
    /// than the other, zero when they are equal, and a positive number when it is higher.
    /// </summary>
    public int CompareTo(SemanticVersion? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (this.Major != other.Major)
        {
            return this.Major.CompareTo(other.Major);
        }

        if (this.Minor != other.Minor)
        {
            return this.Minor.CompareTo(other.Minor);
        }

        return this.Patch.CompareTo(other.Patch);
    }

    public override string ToString() => $"{this.Major}.{this.Minor}.{this.Patch}";

    /// <summary>
    /// An omitted component defaults to zero. Digits are matched explicitly as "[0-9]".
    /// </summary>
    private static bool TryParseComponent(Group group, out int result)
    {
        if (!group.Success)
        {
            result = 0;
            return true;
        }

        return int.TryParse(group.ValueSpan, NumberStyles.None, CultureInfo.InvariantCulture, out result);
    }

    [GeneratedRegex(@"^(?<major>0|[1-9][0-9]*)(?:\.(?<minor>0|[1-9][0-9]*)(?:\.(?<patch>0|[1-9][0-9]*))?)?$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
    private static partial Regex VersionPattern();
}
