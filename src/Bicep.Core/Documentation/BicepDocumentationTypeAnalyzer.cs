// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Documentation;

internal static class BicepDocumentationTypeAnalyzer
{
    // Bicep type declarations cannot be cyclic, so a depth limit alone is enough to bound recursion.
    private const int MaxDepth = 20;

    public static string GetTypeName(TypeSymbol type) => Analyze(GetEffectiveType(type), MaxDepth).TypeName;

    public static BicepDocumentationParameter BuildParameter(string name, TypeSymbol type, bool isRequired, string? description, string? defaultValue)
    {
        var effectiveType = GetEffectiveType(type);
        var analysis = Analyze(effectiveType, 0);

        return new BicepDocumentationParameter(
            name,
            analysis.TypeName,
            isRequired,
            effectiveType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
            description,
            defaultValue,
            analysis.AllowedValues,
            analysis.MinValue,
            analysis.MaxValue,
            analysis.MinLength,
            analysis.MaxLength,
            analysis.Pattern,
            analysis.NestedProperties,
            analysis.Discriminator);
    }

    private static BicepDocumentationParameter BuildProperty(string name, NamedTypeProperty property, int depth)
    {
        var effectiveType = GetEffectiveType(property.TypeReference.Type);
        var analysis = Analyze(effectiveType, depth);

        return new BicepDocumentationParameter(
            name,
            analysis.TypeName,
            TypeHelper.IsRequired(property),
            effectiveType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
            property.Description,
            null,
            analysis.AllowedValues,
            analysis.MinValue,
            analysis.MaxValue,
            analysis.MinLength,
            analysis.MaxLength,
            analysis.Pattern,
            analysis.NestedProperties,
            analysis.Discriminator);
    }

    private static TypeSymbol GetEffectiveType(TypeSymbol type) => TypeHelper.TryRemoveNullability(type) ?? type;

    private static TypeAnalysis Analyze(TypeSymbol type, int depth)
    {
        switch (type)
        {
            case UnionType union when union.Members.Length > 0 && union.Members.All(m => IsSimpleLiteral(m.Type)):
                var literalTypes = union.Members.Select(m => m.Type).ToImmutableArray();
                var baseTypeName = literalTypes.Select(t => t.GetType()).Distinct().Count() == 1
                    ? GetLiteralBaseTypeName(literalTypes[0])
                    : union.Name;

                return TypeAnalysis.Simple(baseTypeName) with { AllowedValues = SortLiteralValues(literalTypes) };

            case IntegerType integer:
                return TypeAnalysis.Simple(LanguageConstants.TypeNameInt) with
                {
                    MinValue = integer.MinValue,
                    MaxValue = integer.MaxValue,
                };

            case StringType str:
                return TypeAnalysis.Simple(LanguageConstants.TypeNameString) with
                {
                    MinLength = str.MinLength,
                    MaxLength = str.MaxLength,
                    Pattern = str.Pattern,
                };

            case ArrayType array:
                return TypeAnalysis.Simple(LanguageConstants.ArrayType) with
                {
                    MinLength = array.MinLength,
                    MaxLength = array.MaxLength,
                    AllowedValues = GetLiteralUnionAllowedValues(GetEffectiveType(array.Item.Type)),
                };

            case DiscriminatedObjectType discriminated:
                if (depth >= MaxDepth)
                {
                    return TypeAnalysis.Simple(LanguageConstants.ObjectType);
                }

                var discriminator = BuildDiscriminator(discriminated, depth + 1);

                return TypeAnalysis.Simple(LanguageConstants.ObjectType) with { Discriminator = discriminator };

            case ObjectType obj:
                if (depth >= MaxDepth)
                {
                    return TypeAnalysis.Simple(LanguageConstants.ObjectType);
                }

                var nestedProperties = BuildProperties(obj, depth + 1);

                return TypeAnalysis.Simple(LanguageConstants.ObjectType) with { NestedProperties = nestedProperties };

            default:
                return TypeAnalysis.Simple(type.Name);
        }
    }

    private static ImmutableArray<BicepDocumentationParameter> BuildProperties(ObjectType obj, int depth)
    {
        var properties = obj.Properties
            .Select(kvp => BuildProperty(kvp.Key, kvp.Value, depth))
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(properties, p => p.Name);
    }

    private static BicepDocumentationDiscriminator BuildDiscriminator(DiscriminatedObjectType discriminated, int depth)
    {
        // UnionMembersByKey keys are escaped Bicep string literals; DiscriminatorKeysUnionType exposes the
        // unescaped raw values instead.
        var keyLiterals = discriminated.DiscriminatorKeysUnionType switch
        {
            UnionType union => union.Members.Select(m => m.Type),
            var single => [single],
        };

        var cases = keyLiterals
            .OfType<StringLiteralType>()
            .Where(literal => discriminated.UnionMembersByKey.ContainsKey(literal.Name))
            .Select(literal => new BicepDocumentationDiscriminatorCase(
                literal.RawStringValue,
                BuildProperties(discriminated.UnionMembersByKey[literal.Name], depth)))
            .ToImmutableArray();

        return new BicepDocumentationDiscriminator(discriminated.DiscriminatorKey, BicepDocumentationOrdering.SortByName(cases, c => c.Value));
    }

    private static bool IsSimpleLiteral(TypeSymbol type) => type is StringLiteralType or IntegerLiteralType or BooleanLiteralType;

    private static string GetLiteralBaseTypeName(TypeSymbol literal) => literal switch
    {
        StringLiteralType => LanguageConstants.TypeNameString,
        IntegerLiteralType => LanguageConstants.TypeNameInt,
        _ => LanguageConstants.TypeNameBool,
    };

    private static string GetLiteralRawValue(TypeSymbol literal) => literal switch
    {
        StringLiteralType stringLiteral => stringLiteral.RawStringValue,
        IntegerLiteralType integerLiteral => integerLiteral.Value.ToString(CultureInfo.InvariantCulture),
        _ => literal.Name,
    };

    private static ImmutableArray<string> SortLiteralValues(IEnumerable<TypeSymbol> literalTypes) =>
        literalTypes
            .Select(GetLiteralRawValue)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ThenBy(value => value, StringComparer.Ordinal)
            .ToImmutableArray();

    private static ImmutableArray<string> GetLiteralUnionAllowedValues(TypeSymbol itemType) => itemType switch
    {
        UnionType union when union.Members.Length > 0 && union.Members.All(m => IsSimpleLiteral(m.Type)) =>
            SortLiteralValues(union.Members.Select(m => m.Type)),
        _ => [],
    };

    private sealed record TypeAnalysis(
        string TypeName,
        ImmutableArray<string> AllowedValues,
        long? MinValue,
        long? MaxValue,
        long? MinLength,
        long? MaxLength,
        string? Pattern,
        ImmutableArray<BicepDocumentationParameter> NestedProperties,
        BicepDocumentationDiscriminator? Discriminator)
    {
        public static TypeAnalysis Simple(string typeName) => new(
            typeName,
            [],
            null,
            null,
            null,
            null,
            null,
            [],
            null);
    }
}
