// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Globalization;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Common;

/// <summary>
/// Helpers for following secure values from an expression to the input it is assigned to.
/// </summary>
/// <remarks>
/// Paths are sequences of segments: a property name, <see cref="AnyMember"/> for any member, <see cref="AnyIndex"/>
/// for any array element, or <c>[n]</c> for a known position.
/// </remarks>
public static class SecureValueFlow
{
    public const string AnyMember = "*";
    public const string AnyIndex = "[*]";

    private const int MaxDepth = 64;

    /// <summary>
    /// Where a value is placed in a module parameter: at <see cref="Prefix"/> within an input of type
    /// <see cref="SinkType"/>, displayed as <see cref="Input"/>.
    /// </summary>
    public record InputSlot(
        SyntaxBase Value,
        string Input,
        TypeSymbol SinkType,
        ImmutableArray<string> Prefix,
        bool IsObjectKey = false,
        ImmutableHashSet<string>? OverriddenProperties = null);

    /// <summary>
    /// Properties and spreads with the names that later entries definitely replace. Unknown keys and optional
    /// properties cannot prove an earlier value is overwritten.
    /// </summary>
    public static IEnumerable<(SyntaxBase Child, ImmutableHashSet<string> OverriddenProperties)> GetObjectChildren(SemanticModel model, ObjectSyntax syntax)
    {
        var overridden = ImmutableHashSet.Create<string>(LanguageConstants.IdentifierComparer);
        var children = new List<(SyntaxBase, ImmutableHashSet<string>)>();
        foreach (var child in syntax.Children.Reverse())
        {
            children.Add((child, overridden));
            if (child is ObjectPropertySyntax property && property.TryGetKeyText() is { } name)
            {
                overridden = overridden.Add(name);
            }
            else if (child is SpreadExpressionSyntax spread && GetEffectiveType(model, spread.Expression) is ObjectType spreadType)
            {
                overridden = overridden.Union(spreadType.Properties.Values
                    .Where(property => property.Flags.HasFlag(TypePropertyFlags.Required))
                    .Select(property => property.Name));
            }
        }

        children.Reverse();
        return children;
    }

    public static bool IsOverwritten(ImmutableArray<string> path, ImmutableHashSet<string>? overridden)
        => !path.IsEmpty && overridden?.Contains(path[0]) == true;

    /// <summary>
    /// Walks object, array, and loop literals assigned to an input, pairing each value inside them with the type
    /// of the input it lands in. Named properties use their declared type, which selects discriminated variants,
    /// and literal array items use their tuple position until a spread makes positions unknown.
    /// </summary>
    public static IEnumerable<InputSlot> GetInputSlots(SemanticModel model, SyntaxBase value, string input, TypeSymbol sinkType)
    {
        if (IsSecureType(sinkType))
        {
            yield return new(value, input, sinkType, []);
            yield break;
        }

        switch (value)
        {
            case ObjectSyntax @object:
                foreach (var (child, overridden) in GetObjectChildren(model, @object))
                {
                    if (child is ObjectPropertySyntax skippedProperty && skippedProperty.TryGetKeyText() is { } keyName && overridden.Contains(keyName))
                    {
                        continue;
                    }

                    if (child is ObjectPropertySyntax { Key: StringSyntax { Expressions.Length: > 0 } key })
                    {
                        yield return new(key, input, sinkType, [], IsObjectKey: true);
                    }

                    IEnumerable<InputSlot> slots = child switch
                    {
                        ObjectPropertySyntax property when property.TryGetKeyText() is { } name
                            => GetInputSlots(model, property.Value, $"{input}.{name}", model.GetDeclaredType(property) ?? GetMemberType(sinkType, name)),
                        ObjectPropertySyntax property => [new(property.Value, input, sinkType, [AnyMember])],
                        SpreadExpressionSyntax spread => [new(spread.Expression, input, sinkType, [], OverriddenProperties: overridden)],
                        _ => [],
                    };

                    foreach (var slot in slots)
                    {
                        yield return slot;
                    }
                }

                yield break;

            case ArraySyntax array:
                int? position = 0;
                foreach (var child in array.Children)
                {
                    if (child is ArrayItemSyntax item)
                    {
                        var segment = position is { } index ? FormatIndex(index) : AnyIndex;
                        foreach (var slot in GetInputSlots(model, item.Value, input + segment, GetItemType(sinkType, position)))
                        {
                            yield return slot;
                        }

                        position++;
                    }
                    else if (child is SpreadExpressionSyntax spread)
                    {
                        // A spread's items can land at any position from where it starts.
                        yield return new(spread.Expression, input, GetRemainingItemsType(sinkType, position), []);
                        position = null;
                    }
                }

                yield break;

            case ForSyntax @for:
                foreach (var slot in GetInputSlots(model, @for.Body, input + AnyIndex, model.GetDeclaredType(@for.Body) ?? GetItemType(sinkType)))
                {
                    yield return slot;
                }

                yield break;

            default:
                yield return new(value, input, sinkType, []);
                yield break;
        }
    }

    private static TypeSymbol GetRemainingItemsType(TypeSymbol sinkType, int? start)
    {
        if ((TypeHelper.TryRemoveNullability(sinkType) ?? sinkType) is not TupleType tuple)
        {
            return sinkType;
        }

        var remaining = tuple.Items.Skip(start ?? 0).ToArray();
        return remaining.Length > 0 ? new TypedArrayType(TypeHelper.CreateTypeUnion(remaining), default) : LanguageConstants.Array;
    }

    /// <summary>
    /// Splits an expression into the sub-expressions whose values can flow into its result. Both branches of a
    /// ternary and both operands of <c>??</c> are data; a ternary condition is not.
    /// </summary>
    public static IEnumerable<SyntaxBase> GetDataLeaves(SyntaxBase syntax) => syntax switch
    {
        ParenthesizedExpressionSyntax parenthesized => GetDataLeaves(parenthesized.Expression),
        NonNullAssertionSyntax nonNull => GetDataLeaves(nonNull.BaseExpression),
        TernaryOperationSyntax ternary => GetDataLeaves(ternary.TrueExpression).Concat(GetDataLeaves(ternary.FalseExpression)),
        BinaryOperationSyntax { Operator: BinaryOperator.Coalesce } coalesce => GetDataLeaves(coalesce.LeftExpression).Concat(GetDataLeaves(coalesce.RightExpression)),
        _ => [syntax],
    };

    /// <summary>
    /// The arguments of a <c>union</c> or <c>concat</c> call. The result keeps every property and array item of its
    /// arguments, but array items can move, so a path into an argument should go through
    /// <see cref="ForgetPosition"/>.
    /// </summary>
    public static IEnumerable<SyntaxBase>? TryGetCombinedValues(SemanticModel model, SyntaxBase syntax)
        => syntax is FunctionCallSyntaxBase functionCall &&
            (functionCall.Name.IdentifierName.Equals("union", StringComparison.OrdinalIgnoreCase) ||
                functionCall.Name.IdentifierName.Equals("concat", StringComparison.OrdinalIgnoreCase)) &&
            SemanticModelHelper.TryGetFunctionInNamespace(model, SystemNamespaceType.BuiltInName, functionCall) is not null
            ? functionCall.Arguments.Select(argument => argument.Expression)
            : null;

    /// <summary>
    /// Replaces a known array position at the start of a path with <see cref="AnyIndex"/>.
    /// </summary>
    public static ImmutableArray<string> ForgetPosition(ImmutableArray<string> path)
        => !path.IsEmpty && IsIndexSegment(path[0]) ? path.SetItem(0, AnyIndex) : path;

    /// <summary>
    /// The type of the members an object doesn't declare, if it allows any. A closed object type still has an
    /// <c>any</c> fallback, which only lets Bicep warn about, rather than reject, access to an undeclared member.
    /// </summary>
    public static TypeSymbol? TryGetAdditionalPropertiesType(ObjectType @object)
        => @object.AdditionalProperties is { } additional && !additional.Flags.HasFlag(TypePropertyFlags.FallbackProperty)
            ? additional.TypeReference.Type
            : null;

    /// <summary>
    /// Finds the symbol at the root of a chain of property and index accesses, and the path the chain reads from
    /// it. Returns a null symbol for anything else.
    /// </summary>
    public static (Symbol? Root, ImmutableArray<string> Path) GetAccessPath(SemanticModel model, SyntaxBase syntax)
    {
        var path = ImmutableArray.CreateBuilder<string>();
        var current = syntax;
        for (var depth = 0; depth < MaxDepth; depth++)
        {
            switch (current)
            {
                case VariableAccessSyntax access:
                    path.Reverse();
                    return (model.GetSymbolInfo(access), path.ToImmutable());
                case PropertyAccessSyntax propertyAccess:
                    path.Add(propertyAccess.PropertyName.IdentifierName);
                    current = propertyAccess.BaseExpression;
                    break;
                case ArrayAccessSyntax arrayAccess:
                    path.Add(arrayAccess.IndexExpression switch
                    {
                        IntegerLiteralSyntax integer when integer.Value <= int.MaxValue => FormatIndex((int)integer.Value),
                        StringSyntax @string when @string.TryGetLiteralValue() is { } name => name,
                        var index when TypeHelper.SatisfiesCondition(model.GetTypeInfo(index), type => type is IntegerType or IntegerLiteralType) => AnyIndex,
                        _ => AnyMember,
                    });
                    current = arrayAccess.BaseExpression;
                    break;
                case NonNullAssertionSyntax nonNull:
                    current = nonNull.BaseExpression;
                    break;
                case ParenthesizedExpressionSyntax parenthesized:
                    current = parenthesized.Expression;
                    break;
                default:
                    return (null, []);
            }
        }

        return (null, []);
    }

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
    /// Whether the expression reads a property or element from a value whose type is secure, such as
    /// <c>secureObjectParam.?key</c>. Members of a secure container are secure even when their own type is not.
    /// </summary>
    public static bool IsReadFromSecureValue(SemanticModel model, SyntaxBase syntax)
    {
        var current = syntax;
        for (var depth = 0; depth < MaxDepth; depth++)
        {
            SyntaxBase? baseExpression = current switch
            {
                AccessExpressionSyntax access => access.BaseExpression,
                NonNullAssertionSyntax nonNull => nonNull.BaseExpression,
                ParenthesizedExpressionSyntax parenthesized => parenthesized.Expression,
                _ => null,
            };

            if (baseExpression is null)
            {
                return false;
            }

            if (IsSecureType(GetEffectiveType(model, baseExpression)))
            {
                return true;
            }

            current = baseExpression;
        }

        return false;
    }

    /// <summary>
    /// The type of an expression, recovering the item types of loop and lambda variables where the type checker
    /// widens them to <c>any</c>. That happens when iterating a union of arrays, e.g. the common
    /// <c>for x in (optionalArray ?? [])</c>, which would otherwise hide the <c>@secure()</c> properties of each item.
    /// </summary>
    public static TypeSymbol GetEffectiveType(SemanticModel model, SyntaxBase syntax)
    {
        var type = model.GetTypeInfo(syntax);
        return type.TypeKind == TypeKind.Any && TryGetEffectiveType(model, syntax, 0) is { } effective ? effective : type;
    }

    private static TypeSymbol? TryGetEffectiveType(SemanticModel model, SyntaxBase syntax, int depth)
    {
        if (depth > MaxDepth)
        {
            return null;
        }

        switch (syntax)
        {
            case ParenthesizedExpressionSyntax parenthesized:
                return TryGetEffectiveType(model, parenthesized.Expression, depth + 1);
            case NonNullAssertionSyntax nonNull:
                return TryGetEffectiveType(model, nonNull.BaseExpression, depth + 1);
            case VariableAccessSyntax access when model.GetSymbolInfo(access) is LocalVariableSymbol local:
                return TryGetItemSource(model, local) is { } itemSource ? GetIteratedItemType(model, itemSource, depth + 1) : null;
            case PropertyAccessSyntax propertyAccess:
                return TryGetEffectiveType(model, propertyAccess.BaseExpression, depth + 1) is { } propertyBase
                    ? GetMemberType(propertyBase, propertyAccess.PropertyName.IdentifierName)
                    : null;
            case ArrayAccessSyntax arrayAccess:
                if (TryGetEffectiveType(model, arrayAccess.BaseExpression, depth + 1) is not { } indexBase)
                {
                    return null;
                }

                return arrayAccess.IndexExpression switch
                {
                    StringSyntax index when index.TryGetLiteralValue() is { } name => GetMemberType(indexBase, name),
                    IntegerLiteralSyntax integer when integer.Value <= int.MaxValue => GetItemType(indexBase, (int)integer.Value),
                    _ => GetItemType(indexBase),
                };
            default:
                return null;
        }
    }

    private static TypeSymbol? GetIteratedItemType(SemanticModel model, SyntaxBase arraySyntax, int depth)
    {
        // Each data leaf of the iterated expression supplies items; an empty or null leaf supplies none.
        var itemTypes = GetDataLeaves(arraySyntax)
            .Select(leaf => model.GetTypeInfo(leaf) is { TypeKind: not TypeKind.Any } leafType ? leafType : TryGetEffectiveType(model, leaf, depth + 1))
            .Select(leafType => leafType is null ? LanguageConstants.Any : GetItemType(leafType))
            .Where(itemType => itemType.TypeKind != TypeKind.Never)
            .ToArray();

        return itemTypes.Length > 0 ? TypeHelper.CreateTypeUnion(itemTypes) : null;
    }

    /// <summary>
    /// The expression whose items a loop or lambda variable receives, if it receives items.
    /// </summary>
    public static SyntaxBase? TryGetItemSource(SemanticModel model, LocalVariableSymbol local)
    {
        if (model.GetSymbolParent(local) is not LocalScope { DeclaringSyntax: { } scope })
        {
            return null;
        }

        if (local.LocalKind == LocalKind.ForExpressionItemVariable && scope is ForSyntax @for)
        {
            return @for.Expression;
        }

        if (local.LocalKind != LocalKind.LambdaItemVariable ||
            model.Binder.GetParent(scope) is not FunctionArgumentSyntax argument ||
            model.Binder.GetParent(argument) is not FunctionCallSyntaxBase { Arguments.Length: > 0 } functionCall ||
            SemanticModelHelper.TryGetFunctionInNamespace(model, SystemNamespaceType.BuiltInName, functionCall) is null)
        {
            return null;
        }

        var parameterIndex = scope switch
        {
            LambdaSyntax lambda => IndexOf(lambda.GetLocalVariables(), local.DeclaringSyntax),
            TypedLambdaSyntax typedLambda => IndexOf(typedLambda.GetLocalVariables(), local.DeclaringSyntax),
            _ => -1,
        };

        // Which lambda parameters receive items of the first argument.
        var receivesItem = functionCall.Name.IdentifierName.ToLowerInvariant() switch
        {
            "map" or "filter" or "toobject" or "groupby" => parameterIndex == 0,
            "sort" => parameterIndex is 0 or 1,
            "reduce" => parameterIndex == 1,
            _ => false,
        };

        return receivesItem ? functionCall.Arguments[0].Expression : null;
    }

    private static int IndexOf(IEnumerable<SyntaxBase> syntaxes, SyntaxBase syntax)
        => syntaxes.Select((candidate, index) => ReferenceEquals(candidate, syntax) ? index : -1).FirstOrDefault(index => index >= 0, -1);

    /// <summary>
    /// The type of a named member, combining the members of every non-null union member and discriminated variant.
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
        => FindSecurePaths(type, [], new(ReferenceEqualityComparer.Instance), 0)
            .DistinctBy(GetPathKey);

    private static IEnumerable<ImmutableArray<string>> FindSecurePaths(TypeSymbol type, ImmutableArray<string> path, HashSet<TypeSymbol> visiting, int depth)
    {
        if (depth > MaxDepth || !visiting.Add(type))
        {
            yield break;
        }

        try
        {
            if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
            {
                yield return path;
                yield break;
            }

            IEnumerable<ImmutableArray<string>> children = type switch
            {
                UnionType union => union.Members.SelectMany(member => FindSecurePaths(member.Type, path, visiting, depth + 1)),
                DiscriminatedObjectType discriminated => discriminated.UnionMembersByKey.Values.SelectMany(variant => FindSecurePaths(variant, path, visiting, depth + 1)),
                ObjectType @object => @object.Properties.Values
                    .SelectMany(property => FindSecurePaths(property.TypeReference.Type, path.Add(property.Name), visiting, depth + 1))
                    .Concat(@object.AdditionalProperties is { } additional
                        ? FindSecurePaths(additional.TypeReference.Type, path.Add(AnyMember), visiting, depth + 1)
                        : []),
                TupleType tuple => tuple.Items.SelectMany((item, index) => FindSecurePaths(item.Type, path.Add(FormatIndex(index)), visiting, depth + 1)),
                ArrayType array => FindSecurePaths(array.Item.Type, path.Add(AnyIndex), visiting, depth + 1),
                _ => [],
            };

            foreach (var child in children)
            {
                yield return child;
            }
        }
        finally
        {
            visiting.Remove(type);
        }
    }

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

        switch (type)
        {
            case DiscriminatedObjectType discriminated:
                var applicable = discriminated.UnionMembersByKey.Values
                    .Where(variant => head == AnyMember || variant.Properties.ContainsKey(head) || TryGetAdditionalPropertiesType(variant) is not null)
                    .ToArray();
                return applicable.Length > 0
                    ? applicable.All(variant => IsSecureAtPath(variant, path, absentIsSecure, depth + 1))
                    : absentIsSecure;

            case ObjectType @object when head == AnyMember:
                // Any member can supply a dictionary entry, so declared properties must be secure too.
                return (TryGetAdditionalPropertiesType(@object) is { } anyAdditional
                        ? IsSecureAtPath(anyAdditional, tail, absentIsSecure, depth + 1)
                        : absentIsSecure) &&
                    @object.Properties.Values.All(property => IsSecureAtPath(property.TypeReference.Type, tail, absentIsSecure, depth + 1));

            case ObjectType @object when @object.Properties.TryGetValue(head, out var property):
                return IsSecureAtPath(property.TypeReference.Type, tail, absentIsSecure, depth + 1);

            case ObjectType @object:
                return TryGetAdditionalPropertiesType(@object) is { } additional
                    ? IsSecureAtPath(additional, tail, absentIsSecure, depth + 1)
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
    public static string GetPathKey(ImmutableArray<string> path) => string.Join('\0', path);

    public static bool IsIndexSegment(string segment) => segment.StartsWith('[');

    public static string FormatPath(ImmutableArray<string> path)
        => string.Concat(path.Select(segment => IsIndexSegment(segment) ? segment : $".{segment}"));

    public static string FormatIndex(int index) => $"[{index.ToString(CultureInfo.InvariantCulture)}]";

    public static int? TryParseIndex(string segment)
        => segment.Length > 2 && segment[0] == '[' && segment[^1] == ']' &&
            int.TryParse(segment.AsSpan(1, segment.Length - 2), NumberStyles.None, CultureInfo.InvariantCulture, out var index)
            ? index
            : null;
}
