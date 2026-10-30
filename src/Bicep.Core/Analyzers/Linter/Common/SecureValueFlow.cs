// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Common;

/// <summary>
/// Helpers for following secure values from an expression to the input it is assigned to.
/// </summary>
/// <remarks>
/// Paths are sequences of segments: a dot-prefixed property name, <see cref="AnyMember"/> for any member, <see cref="AnyIndex"/>
/// for any array element, or <c>[n]</c> for a known position.
/// </remarks>
public static class SecureValueFlow
{
    public const string AnyMember = "*";
    public const string AnyIndex = "[*]";

    private const int MaxDepth = 64;

    /// <summary>
    /// Replaces a known array position at the start of a path with <see cref="AnyIndex"/>.
    /// </summary>
    public static ImmutableArray<string> ForgetPosition(ImmutableArray<string> path)
        => !path.IsEmpty && IsIndexSegment(path[0]) ? path.SetItem(0, AnyIndex) : path;

    /// <summary>
    /// Re-roots a path at an accessed member. A path that contains the accessed member covers all of it, so the
    /// result is empty; a path elsewhere returns null.
    /// </summary>
    public static ImmutableArray<string>? TryStripPrefix(ImmutableArray<string> path, ImmutableArray<string> prefix)
    {
        for (var i = 0; i < prefix.Length; i++)
        {
            if (i == path.Length)
            {
                return [];
            }

            if (!SegmentsMatch(path[i], prefix[i]))
            {
                return null;
            }
        }

        return path.RemoveRange(0, prefix.Length);
    }

    /// <summary>
    /// Whether two path segments can refer to the same member or element.
    /// </summary>
    public static bool SegmentsMatch(string left, string right)
    {
        if (left == AnyMember || right == AnyMember)
        {
            return true;
        }

        return (IsIndexSegment(left), IsIndexSegment(right)) switch
        {
            (true, true) => left == AnyIndex || right == AnyIndex || left == right,
            (false, false) => LanguageConstants.IdentifierComparer.Equals(left, right),
            _ => false,
        };
    }

    /// <summary>
    /// The type of a named member, combining the members of every non-null union member and discriminated variant.
    /// Unlike type-checking a property read, this preserves non-collapsible unions and accepts write-only sinks.
    /// </summary>
    public static TypeSymbol GetMemberType(TypeSymbol type, string name) => type switch
    {
        UnionType union => TypeHelper.CreateTypeUnion(union.Members.Where(member => member.Type is not NullType).Select(member => GetMemberType(member.Type, name))),
        DiscriminatedObjectType discriminated => TypeHelper.CreateTypeUnion(discriminated.UnionMembersByKey.Values.Select(variant => GetMemberType(variant, name))),
        ObjectType @object when @object.Properties.TryGetValue(name, out var property) => property.TypeReference.Type,
        ObjectType { AdditionalProperties: { } additional } => additional.TypeReference.Type,
        _ => LanguageConstants.Any,
    };

    /// <summary>
    /// The type of an element, at a known position when given. A null or empty array has no elements, so its
    /// element type is <c>never</c>.
    /// </summary>
    public static TypeSymbol GetItemType(TypeSymbol type, int? position = null) => type switch
    {
        UnionType union => TypeHelper.CreateTypeUnion(union.Members.Where(member => member.Type is not NullType).Select(member => GetItemType(member.Type, position))),
        TupleType tuple when position is { } index => index < tuple.Items.Length ? tuple.Items[index].Type : LanguageConstants.Any,
        TupleType { Items.Length: 0 } => LanguageConstants.Never,
        ArrayType array => array.Item.Type,
        NullType => LanguageConstants.Never,
        _ => LanguageConstants.Any,
    };

    /// <summary>
    /// Whether the type is secure, ignoring nullability.
    /// </summary>
    public static bool IsSecureType(TypeSymbol type)
        => type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) ||
            (TypeHelper.TryRemoveNullability(type) is { } nonNullable && nonNullable.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure));

    /// <summary>
    /// Enumerates the paths within a type at which a value must be secure.
    /// </summary>
    public static IEnumerable<ImmutableArray<string>> FindSecurePaths(TypeSymbol type)
        => TypeHelper.FindPathsToOutermostSecureTypeComponents(type)
            .DistinctBy(GetPathKey);

    /// <summary>
    /// Whether every value of the given type is secure at the given path. A path is secure only if it is secure in
    /// every non-null union member and every applicable discriminated variant, and a dictionary member or array
    /// element is secure only if every member or position that can supply it is.
    /// </summary>
    /// <param name="absentIsSecure">
    /// How to treat a path the type does not contain. When checking a source value, nothing is supplied there, so
    /// nothing can leak. When checking an input that would receive a value at that path, it is not known to be secure.
    /// </param>
    public static bool IsSecureAtPath(TypeSymbol type, ImmutableArray<string> path, bool absentIsSecure = false)
        => IsSecureAtPath(type, path, absentIsSecure, 0);

    private static bool IsSecureAtPath(TypeSymbol type, ImmutableArray<string> path, bool absentIsSecure, int depth)
    {
        if (depth > MaxDepth)
        {
            return false;
        }

        if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
        {
            return true;
        }

        if (type is UnionType union)
        {
            var members = union.Members.Select(member => member.Type).Where(member => member is not NullType).ToArray();
            return members.Length > 0
                ? members.All(member => IsSecureAtPath(member, path, absentIsSecure, depth + 1))
                : absentIsSecure;
        }

        if (path.IsEmpty)
        {
            return false;
        }

        var head = path[0];
        var tail = path.RemoveAt(0);
        var name = TryGetPropertyName(head);

        switch (type)
        {
            case DiscriminatedObjectType discriminated:
                var applicable = discriminated.UnionMembersByKey.Values
                    .Where(variant => head == AnyMember || (name is not null && variant.Properties.ContainsKey(name)) || variant.HasExplicitAdditionalPropertiesType)
                    .ToArray();
                return applicable.Length > 0
                    ? applicable.All(variant => IsSecureAtPath(variant, path, absentIsSecure, depth + 1))
                    : absentIsSecure;

            case ObjectType @object when head == AnyMember:
                // Any member can supply a dictionary entry, so declared properties must be secure too.
                return (@object is { HasExplicitAdditionalPropertiesType: true, AdditionalProperties: { } anyAdditional }
                        ? IsSecureAtPath(anyAdditional.TypeReference.Type, tail, absentIsSecure, depth + 1)
                        : absentIsSecure) &&
                    @object.Properties.Values.All(property => IsSecureAtPath(property.TypeReference.Type, tail, absentIsSecure, depth + 1));

            case ObjectType @object when name is not null && @object.Properties.TryGetValue(name, out var property):
                return IsSecureAtPath(property.TypeReference.Type, tail, absentIsSecure, depth + 1);

            case ObjectType @object when name is not null:
                return @object is { HasExplicitAdditionalPropertiesType: true, AdditionalProperties: { } additional }
                    ? IsSecureAtPath(additional.TypeReference.Type, tail, absentIsSecure, depth + 1)
                    : absentIsSecure;

            case TupleType tuple when head == AnyIndex:
                return tuple.Items.Length > 0
                    ? tuple.Items.All(item => IsSecureAtPath(item.Type, tail, absentIsSecure, depth + 1))
                    : absentIsSecure;

            case TupleType tuple when TryParseIndex(head) is { } index:
                return index < tuple.Items.Length
                    ? IsSecureAtPath(tuple.Items[index].Type, tail, absentIsSecure, depth + 1)
                    : absentIsSecure;

            case ArrayType array when IsIndexSegment(head):
                return IsSecureAtPath(array.Item.Type, tail, absentIsSecure, depth + 1);

            default:
                return false;
        }
    }

    /// <summary>
    /// A key that identifies a path by its segments, unlike <see cref="FormatPath"/>, which can't distinguish
    /// <c>a.b</c> from a property literally named <c>'a.b'</c>.
    /// </summary>
    public static string GetPathKey(ImmutableArray<string> path)
        => string.Concat(path.Select(segment => $"{segment.Length.ToString(CultureInfo.InvariantCulture)}:{segment}"));

    public static bool IsIndexSegment(string segment) => segment.StartsWith('[');

    public static string FormatPath(ImmutableArray<string> path)
        => string.Concat(path.Select(segment => segment == AnyMember ? ".*" : segment));

    public static string FormatProperty(string name) => $".{name}";

    private static string? TryGetPropertyName(string segment) => segment.StartsWith('.') ? segment[1..] : null;

    public static string FormatIndex(int index) => $"[{index.ToString(CultureInfo.InvariantCulture)}]";

    public static int? TryParseIndex(string segment)
        => segment.Length > 2 && segment[0] == '[' && segment[^1] == ']' &&
            int.TryParse(segment.AsSpan(1, segment.Length - 2), NumberStyles.None, CultureInfo.InvariantCulture, out var index)
            ? index
            : null;
}
