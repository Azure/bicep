// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Common;

/// <summary>
/// Compares module parameter assignments with their declared types for the two secure-input rules.
/// </summary>
public sealed class ModuleParameterSecurity
{
    private readonly SemanticModel model;
    private readonly ObjectPropertySyntax assignment;
    private readonly bool secureInput;
    private readonly ResourceTypeResolver? resolver;
    // A variable can satisfy one target and violate another. Keep this cache local to the assignment and rule.
    private readonly Dictionary<(VariableSymbol, TypeSymbol), Mismatch[]> variableMismatches = [];

    // The checks below recurse through iterators. They're methods rather than local functions because CA1851
    // takes tens of seconds to analyze recursive local iterators that capture state.
    private ModuleParameterSecurity(SemanticModel model, ObjectPropertySyntax assignment, bool secureInput)
    {
        this.model = model;
        this.assignment = assignment;
        this.secureInput = secureInput;
        resolver = secureInput ? ResourceTypeResolver.Create(model) : null;
    }

    /// <summary>
    /// Enumerates the values assigned to module parameters in this file.
    /// </summary>
    public static IEnumerable<ObjectPropertySyntax> EnumerateAssignments(SemanticModel model)
        => model.Root.ModuleDeclarations
            .SelectMany(module => module.DeclaringModule.TryGetBody()
                ?.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName)?.Value is ObjectSyntax parameters
                    ? parameters.Properties
                    : []);

    /// <summary>
    /// Describes a secure-value mismatch and the source syntax that should be reported.
    /// </summary>
    /// <param name="Path">The parameter or nested member that needs protection.</param>
    /// <param name="Syntax">The source expression or reference to highlight.</param>
    /// <param name="FoundMessage">The description of the possible secret for the leak warning.</param>
    public record Mismatch(string Path, SyntaxBase Syntax, string FoundMessage);

    /// <summary>
    /// Finds mismatches between module parameter assignments and their declared types.
    /// </summary>
    public static IEnumerable<Mismatch> EnumerateMismatches(SemanticModel model, ObjectPropertySyntax assignment, bool secureInput)
    {
        if (model.GetDeclaredType(assignment) is not { } parameterType ||
            assignment.TryGetKeyText() is not { } parameterName)
        {
            return [];
        }

        // Overlapping union alternatives can report the same mismatch more than once.
        return new ModuleParameterSecurity(model, assignment, secureInput)
            .CheckExpression(assignment.Value, parameterType, parameterName, reference: null).Distinct();
    }

    // A finding is reported on the value it comes from, or on the variable reference that led to it, since
    // a variable's declaration can be shared by several modules.
    private IEnumerable<Mismatch> CheckExpression(SyntaxBase value, TypeSymbol target, string path, SyntaxBase? reference)
    {
        // Compiler errors already explain why this value cannot be checked.
        if (model.GetTypeInfo(value) is ErrorType)
        {
            yield break;
        }

        // getSecret() may only be passed directly to a secure parameter.
        if (value is FunctionCallSyntaxBase && model.GetSymbolInfo(value) is FunctionSymbol function &&
            function.FunctionFlags.HasFlag(FunctionFlags.ModuleSecureParameterOnly))
        {
            yield break;
        }

        // Look through syntax that only passes values along or builds them, checking each part against the
        // matching part of the target. Anything else is a leaf, whose type is compared below.
        // With a spread, members can't be matched to syntax, so compare the whole value's type.
        var parts = value switch
        {
            ParenthesizedExpressionSyntax parenthesized =>
                CheckExpression(parenthesized.Expression, target, path, reference),
            NonNullAssertionSyntax nonNull =>
                CheckExpression(nonNull.BaseExpression, target, path, reference),
            VariableAccessSyntax access when model.GetSymbolInfo(access) is VariableSymbol variable =>
                CheckVariable(variable, target, path, reference ?? access),
            TernaryOperationSyntax ternary =>
                CheckExpression(ternary.TrueExpression, target, path, reference)
                    .Concat(CheckExpression(ternary.FalseExpression, target, path, reference)),
            BinaryOperationSyntax { Operator: BinaryOperator.Coalesce } coalesce =>
                CheckExpression(coalesce.LeftExpression, target, path, reference)
                    .Concat(CheckExpression(coalesce.RightExpression, target, path, reference)),
            ObjectSyntax obj when Variants(target) is { } variants =>
                variants.Where(variant => MayMatch(model.GetTypeInfo(obj), variant))
                    .SelectMany(variant => CheckExpression(obj, variant, path, reference)),
            ObjectSyntax obj when !obj.Children.OfType<SpreadExpressionSyntax>().Any() =>
                obj.Properties.SelectMany(property =>
                {
                    var name = property.TryGetKeyText();
                    return CheckExpression(property.Value, Member(target, name), $"{path}.{name ?? "*"}", reference);
                }),
            ArraySyntax array when !array.Children.OfType<SpreadExpressionSyntax>().Any() =>
                array.Items.SelectMany((item, index) =>
                    CheckExpression(item.Value, Item(target, index), $"{path}[{index}]", reference)),
            ForSyntax loop =>
                CheckExpression(loop.Body, Item(target), $"{path}[*]", reference),
            _ => null,
        };

        if (parts is not null)
        {
            foreach (var mismatch in parts)
            {
                yield return mismatch;
            }

            yield break;
        }

        if (resolver is { } resourceTypeResolver && !IsDeployTimeConstant(value, resourceTypeResolver))
        {
            yield break;
        }

        // Let the visitor inspect interpolated values instead of using the string's secure flag.
        // It excludes getSecret(), whose use inside interpolation already produces BCP180.
        var source = value is StringSyntax { Expressions.Length: > 0 } ? LanguageConstants.String : model.GetTypeInfo(value);
        var secrets = FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(model, value).ToList();
        // union() tracks secure member types and can overwrite them with public values. Don't put those
        // discarded secrets back on the result; use the merged type instead.
        var hasTrackedUnionSecrets = SemanticModelHelper.TryGetNamedFunction(model, SystemNamespaceType.BuiltInName, "union", value) is { } union &&
            union.Arguments.Any(argument => TypeHelper.IsOrContainsSecureType(model.GetTypeInfo(argument.Expression)));
        // If a result type doesn't retain the secret, treat the whole result as secure. Composite types that
        // retain secure members are compared member by member below.
        var sourceSecure = secrets.Any(secret => ReferenceEquals(secret.Syntax, value)) ||
            secrets.Count > 0 && !hasTrackedUnionSecrets && !TypeHelper.IsOrContainsSecureType(source);

        // Name the leaf when it is the secure read itself; otherwise name the first secret inside it.
        var secret = value is AccessExpressionSyntax or VariableAccessSyntax ? null : secrets.FirstOrDefault();
        var syntax = reference ?? secret?.Syntax ?? value;
        var foundMessage = secret?.FoundMessage ?? FindPossibleSecretsVisitor.PossibleSecretMessage(value.ToString().Trim());
        foreach (var mismatchPath in Compare(source, target, path, sourceSecure, false, []))
        {
            yield return new(mismatchPath, syntax, foundMessage);
        }
    }

    private IEnumerable<Mismatch> CheckVariable(VariableSymbol variable, TypeSymbol target, string path, SyntaxBase reference)
    {
        var key = (variable, target);
        if (!variableMismatches.TryGetValue(key, out var mismatches))
        {
            // Cache relative paths and declaration-local anchors, not the first caller's destination.
            mismatches = CheckExpression(variable.DeclaringVariable.Value, target, "", reference: null).ToArray();
            variableMismatches.Add(key, mismatches);
        }

        return mismatches.Select(mismatch => mismatch with { Path = path + mismatch.Path, Syntax = reference });
    }

    private bool IsDeployTimeConstant(SyntaxBase value, ResourceTypeResolver resourceTypeResolver)
    {
        var diagnostics = ToListDiagnosticWriter.Create();
        DeployTimeConstantValidator.CheckDeployTimeConstantViolations(assignment, value, model, diagnostics, resourceTypeResolver);
        return diagnostics.GetDiagnostics().Count == 0;
    }

    private IEnumerable<string> Compare(TypeSymbol source, TypeSymbol target, string path, bool sourceSecure, bool targetSecure,
        HashSet<(TypeSymbol, TypeSymbol, bool, bool)> visiting)
    {
        if (source is NullType or ErrorType or StringLiteralType { RawStringValue: "" } ||
            source.TypeKind == TypeKind.Never || target is NullType or ErrorType)
        {
            yield break;
        }

        var pair = (source, target, sourceSecure, targetSecure);
        if (!visiting.Add(pair))
        {
            yield break;
        }

        try
        {
            sourceSecure |= source.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);
            targetSecure |= target.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);

            if (Variants(source) is { } sources)
            {
                // Every source alternative may be passed.
                foreach (var mismatch in sources.SelectMany(member => Compare(member, target, path, sourceSecure, targetSecure, visiting)))
                {
                    yield return mismatch;
                }
            }
            else if (Variants(target) is { } targets)
            {
                // Only target alternatives the source could match.
                foreach (var mismatch in targets.Where(member => MayMatch(source, member))
                    .SelectMany(member => Compare(source, member, path, sourceSecure, targetSecure, visiting)))
                {
                    yield return mismatch;
                }
            }
            else if (sourceSecure)
            {
                // A secure source satisfies a secure input, and leaks into an insecure one.
                if (!secureInput && !targetSecure)
                {
                    yield return path;
                }
            }
            else if (!secureInput && targetSecure)
            {
                // A secure target protects its contents from the leak rule.
                yield break;
            }
            else if (source is TupleType tuple)
            {
                foreach (var mismatch in tuple.Items.SelectMany((item, index) =>
                    Compare(item.Type, Item(target, index), $"{path}[{index}]", sourceSecure, targetSecure, visiting)))
                {
                    yield return mismatch;
                }
            }
            else if (source is ArrayType sourceArray)
            {
                foreach (var mismatch in Compare(sourceArray.Item.Type, Item(target), $"{path}[*]",
                    sourceSecure, targetSecure, visiting))
                {
                    yield return mismatch;
                }
            }
            else if (target is ArrayType targetArray)
            {
                foreach (var mismatch in Compare(source, targetArray.Item.Type, $"{path}[*]", sourceSecure, targetSecure, visiting))
                {
                    yield return mismatch;
                }
            }
            else if (source is ObjectType || target is ObjectType)
            {
                IEnumerable<string> sourceProperties = (source as ObjectType)?.Properties.Keys ?? [];
                IEnumerable<string> targetProperties = (target as ObjectType)?.Properties.Keys ?? [];
                foreach (var property in sourceProperties.Union(targetProperties))
                {
                    foreach (var mismatch in Compare(Member(source, property), Member(target, property), $"{path}.{property}",
                        sourceSecure, targetSecure, visiting))
                    {
                        yield return mismatch;
                    }
                }

                if (source is ObjectType { HasExplicitAdditionalPropertiesType: true, AdditionalProperties: { } additional })
                {
                    foreach (var mismatch in Compare(additional.TypeReference.Type,
                        Member(target, null), $"{path}.*", sourceSecure, targetSecure, visiting))
                    {
                        yield return mismatch;
                    }
                }
                else if (source is not ObjectType && secureInput && targetSecure)
                {
                    // A value with no known properties, such as any, given to a secure object.
                    yield return path;
                }
            }
            else if (secureInput && targetSecure)
            {
                yield return path;
            }
        }
        finally
        {
            visiting.Remove(pair);
        }
    }

    private static IEnumerable<TypeSymbol>? Variants(TypeSymbol type) => type switch
    {
        UnionType union => union.Members.Select(member => member.Type),
        DiscriminatedObjectType discriminated => discriminated.UnionMembersByKey.Values,
        _ => null,
    };

    private static bool MayMatch(TypeSymbol source, TypeSymbol target)
        // AreTypesAssignable does not compare object properties, so check the declared fields too.
        => TypeValidator.AreTypesAssignable(source, target) &&
            (source is not ObjectType sourceObject || target is not ObjectType targetObject ||
                targetObject.Properties.Values.All(property =>
                    !sourceObject.Properties.TryGetValue(property.Name, out var supplied) ||
                    TypeValidator.AreTypesAssignable(supplied.TypeReference.Type, property.TypeReference.Type)));

    private static TypeSymbol Member(TypeSymbol type, string? name) => type switch
    {
        // A secure container makes each value read from it secure.
        _ when type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) => type,
        UnionType union => TypeHelper.CreateTypeUnion(union.Members.Where(member => member.Type is not NullType).Select(member => Member(member.Type, name))),
        DiscriminatedObjectType discriminated => TypeHelper.CreateTypeUnion(discriminated.UnionMembersByKey.Values.Select(member => Member(member, name))),
        ObjectType obj when name is not null && obj.Properties.TryGetValue(name, out var property) => property.TypeReference.Type,
        ObjectType { HasExplicitAdditionalPropertiesType: true, AdditionalProperties: { } additional } => additional.TypeReference.Type,
        // A missing property is impossible on a closed object; an unknown source shape is still possible.
        ObjectType => LanguageConstants.Never,
        _ => LanguageConstants.Any,
    };

    private static TypeSymbol Item(TypeSymbol type, int? index = null) => type switch
    {
        // A secure container makes each value read from it secure.
        _ when type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) => type,
        TupleType tuple when index is { } i && i < tuple.Items.Length => tuple.Items[i].Type,
        _ => TypeHelper.GetArrayItemType(type),
    };
}
