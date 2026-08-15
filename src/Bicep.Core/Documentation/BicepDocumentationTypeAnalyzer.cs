// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Documentation;

internal sealed class BicepDocumentationTypeAnalyzer
{
    // Bounds expansion of large acyclic type graphs.
    private const int MaxDepth = 20;
    private const int MaxExpandedNodeCount = 10_000;
    private readonly Dictionary<TypeSymbol, TypeAnalysis> rootAnalysisCache = new(ReferenceEqualityComparer.Instance);
    private readonly CancellationToken cancellationToken;
    private int remainingNodeCount = MaxExpandedNodeCount;

    public BicepDocumentationTypeAnalyzer(CancellationToken cancellationToken = default)
    {
        this.cancellationToken = cancellationToken;
    }

    public string GetTypeName(TypeSymbol type)
    {
        var effectiveType = GetEffectiveType(type);
        return effectiveType switch
        {
            UnionType union when union.Members.Length > 0 && union.Members.All(member => IsSimpleLiteral(member.Type)) =>
                GetLiteralUnionTypeName(union),
            StringLiteralType or IntegerLiteralType or BooleanLiteralType => GetLiteralBaseTypeName(effectiveType),
            IntegerType => LanguageConstants.TypeNameInt,
            StringType => LanguageConstants.TypeNameString,
            ArrayType => LanguageConstants.ArrayType,
            DiscriminatedObjectType or ObjectType => LanguageConstants.ObjectType,
            _ => effectiveType.Name,
        };
    }

    public BicepDocumentationParameter BuildParameter(string name, TypeSymbol type, bool isRequired, string? description, string? defaultValue)
    {
        var effectiveType = GetEffectiveType(type);
        if (!TryConsumeNode())
        {
            return BuildTruncatedParameter(name, effectiveType, isRequired, description, defaultValue);
        }

        if (!this.rootAnalysisCache.TryGetValue(effectiveType, out var analysis))
        {
            analysis = Analyze(effectiveType, new(ReferenceEqualityComparer.Instance), 0);
            this.rootAnalysisCache[effectiveType] = analysis;
        }

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
            analysis.IsTruncated,
            analysis.NestedProperties,
            analysis.Discriminator);
    }

    private BicepDocumentationParameter BuildProperty(
        string name,
        NamedTypeProperty property,
        HashSet<TypeSymbol> visited,
        int depth)
    {
        var effectiveType = GetEffectiveType(property.TypeReference.Type);
        if (!TryConsumeNode())
        {
            return BuildTruncatedParameter(
                name,
                effectiveType,
                TypeHelper.IsRequired(property),
                property.Description,
                defaultValue: null);
        }

        var analysis = Analyze(effectiveType, visited, depth);

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
            analysis.IsTruncated,
            analysis.NestedProperties,
            analysis.Discriminator);
    }

    private static TypeSymbol GetEffectiveType(TypeSymbol type) => TypeHelper.TryRemoveNullability(type) ?? type;

    private bool TryConsumeNode()
    {
        this.cancellationToken.ThrowIfCancellationRequested();
        if (this.remainingNodeCount == 0)
        {
            return false;
        }

        this.remainingNodeCount--;
        return true;
    }

    private BicepDocumentationParameter BuildTruncatedParameter(
        string name,
        TypeSymbol type,
        bool isRequired,
        string? description,
        string? defaultValue) =>
        new(
            name,
            GetTypeName(type),
            isRequired,
            type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
            description,
            defaultValue,
            [],
            null,
            null,
            null,
            null,
            null,
            true,
            [],
            null);

    private TypeAnalysis Analyze(TypeSymbol type, HashSet<TypeSymbol> visited, int depth)
    {
        switch (type)
        {
            case UnionType union when union.Members.Length > 0 && union.Members.All(m => IsSimpleLiteral(m.Type)):
                var literalTypes = union.Members.Select(m => m.Type).ToImmutableArray();
                return TypeAnalysis.Simple(GetLiteralUnionTypeName(union)) with { AllowedValues = SortLiteralValues(literalTypes) };

            case StringLiteralType or IntegerLiteralType or BooleanLiteralType:
                return TypeAnalysis.Simple(GetLiteralBaseTypeName(type)) with { AllowedValues = [GetLiteralRawValue(type)] };

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
                if (depth >= MaxDepth || !visited.Add(type))
                {
                    return TypeAnalysis.Truncated(LanguageConstants.ArrayType);
                }

                try
                {
                    var itemAnalysis = Analyze(GetEffectiveType(array.Item.Type), visited, depth + 1);
                    return TypeAnalysis.Simple(LanguageConstants.ArrayType) with
                    {
                        MinLength = array.MinLength,
                        MaxLength = array.MaxLength,
                        AllowedValues = GetLiteralAllowedValues(GetEffectiveType(array.Item.Type)),
                        IsTruncated = itemAnalysis.IsTruncated,
                        NestedProperties = itemAnalysis.NestedProperties,
                        Discriminator = itemAnalysis.Discriminator,
                    };
                }
                finally
                {
                    visited.Remove(type);
                }

            case DiscriminatedObjectType discriminated:
                if (depth >= MaxDepth || !visited.Add(type))
                {
                    return TypeAnalysis.Truncated(LanguageConstants.ObjectType);
                }

                try
                {
                    return TypeAnalysis.Simple(LanguageConstants.ObjectType) with
                    {
                        Discriminator = BuildDiscriminator(discriminated, visited, depth + 1),
                    };
                }
                finally
                {
                    visited.Remove(type);
                }

            case ObjectType obj:
                if (depth >= MaxDepth || !visited.Add(type))
                {
                    return TypeAnalysis.Truncated(LanguageConstants.ObjectType);
                }

                try
                {
                    return TypeAnalysis.Simple(LanguageConstants.ObjectType) with
                    {
                        NestedProperties = BuildProperties(obj, visited, depth + 1),
                    };
                }
                finally
                {
                    visited.Remove(type);
                }

            default:
                return TypeAnalysis.Simple(type.Name);
        }
    }

    private ImmutableArray<BicepDocumentationParameter> BuildProperties(
        ObjectType obj,
        HashSet<TypeSymbol> visited,
        int depth)
    {
        var properties = obj.Properties
            .Select(kvp => BuildProperty(kvp.Key, kvp.Value, visited, depth))
            .Concat(obj.HasExplicitAdditionalPropertiesType && obj.AdditionalProperties is { } additionalProperties
                ? [BuildProperty(
                    ">Any_other_property<",
                    new NamedTypeProperty(
                        ">Any_other_property<",
                        additionalProperties.TypeReference,
                        additionalProperties.Flags,
                        additionalProperties.Description),
                    visited,
                    depth)]
                : [])
            .ToImmutableArray();

        return BicepDocumentationOrdering.SortByName(properties, p => p.Name);
    }

    private BicepDocumentationDiscriminator BuildDiscriminator(
        DiscriminatedObjectType discriminated,
        HashSet<TypeSymbol> visited,
        int depth)
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
                BuildProperties(discriminated.UnionMembersByKey[literal.Name], visited, depth)))
            .ToImmutableArray();

        return new BicepDocumentationDiscriminator(discriminated.DiscriminatorKey, BicepDocumentationOrdering.SortByName(cases, c => c.Value));
    }

    private static bool IsSimpleLiteral(TypeSymbol type) => type is StringLiteralType or IntegerLiteralType or BooleanLiteralType;

    private static string GetLiteralUnionTypeName(UnionType union)
    {
        var literalTypes = union.Members.Select(member => member.Type).ToImmutableArray();
        return literalTypes.Select(literal => literal.GetType()).Distinct().Count() == 1
            ? GetLiteralBaseTypeName(literalTypes[0])
            : union.Name;
    }

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

    private static ImmutableArray<string> GetLiteralAllowedValues(TypeSymbol itemType) => itemType switch
    {
        UnionType union when union.Members.Length > 0 && union.Members.All(m => IsSimpleLiteral(m.Type)) =>
            SortLiteralValues(union.Members.Select(m => m.Type)),
        var literal when IsSimpleLiteral(literal) => [GetLiteralRawValue(literal)],
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
        bool IsTruncated,
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
            false,
            [],
            null);

        public static TypeAnalysis Truncated(string typeName) => Simple(typeName) with { IsTruncated = true };
    }
}
