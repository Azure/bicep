// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class UseSecureValueForSecureInputsRule : LinterRuleBase
{
    public new const string Code = "use-secure-value-for-secure-inputs";

    public UseSecureValueForSecureInputsRule() : base(
        code: Code,
        description: CoreResources.UseSecureValueForSecureInputsRule_Description,
        LinterRuleCategory.Security)
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.UseSecureValueForSecureInputsRule_MessageFormat, values);

    private static bool ExpectsSecureType(SemanticModel model, SyntaxBase syntax)
        => model.GetDeclaredType(syntax) is { } type && SecureValueFlow.IsSecureType(type);

    private static IEnumerable<ObjectPropertySyntax> GetSecureObjectPropertiesFromTypeInformation(SemanticModel model)
    {
        foreach (var resource in model.DeclaredResources)
        {
            if (resource.Symbol.DeclaringResource.TryGetBody() is not { } resourceBody)
            {
                continue;
            }

            var properties = SyntaxAggregator.AggregateByType<ObjectPropertySyntax>(resourceBody)
                .Where(x => ExpectsSecureType(model, x));

            foreach (var property in properties)
            {
                yield return property;
            }
        }
    }

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var analyzer = new Analyzer(model, ResourceTypeResolver.Create(model));
        foreach (var property in GetSecureObjectPropertiesFromTypeInformation(model))
        {
            if (!analyzer.IsSecureAtPath(property, property.Value, []))
            {
                yield return CreateDiagnosticForSpan(
                    diagnosticLevel,
                    property.Value.Span,
                    [property.TryGetKeyText() ?? string.Empty]);
            }
        }

        foreach (var module in model.Root.ModuleDeclarations)
        {
            if (module.DeclaringModule.TryGetBody()?.TryGetPropertyByName(LanguageConstants.ModuleParamsPropertyName) is not { Value: ObjectSyntax paramsObject })
            {
                continue;
            }

            foreach (var property in paramsObject.Properties)
            {
                if (property.TryGetKeyText() is not { } name || model.GetDeclaredType(property) is not { } sinkType)
                {
                    continue;
                }

                foreach (var slot in SecureValueFlow.GetInputSlots(model, property.Value, name, sinkType).Where(slot => !slot.IsObjectKey))
                {
                    foreach (var securePath in SecureValueFlow.FindSecurePaths(slot.SinkType))
                    {
                        // A value placed inside a secure container must be entirely secure.
                        if (!SecureValueFlow.IsOverwritten(securePath, slot.OverriddenProperties) &&
                            SecureValueFlow.TryStripPrefix(securePath, slot.Prefix) is { } pathInValue &&
                            !analyzer.IsSecureAtPath(property, slot.Value, pathInValue))
                        {
                            yield return CreateDiagnosticForSpan(diagnosticLevel, slot.Value.Span, [slot.Input + SecureValueFlow.FormatPath(securePath)]);
                        }
                    }
                }
            }
        }
    }

    private static bool IsPotentiallyInsecure(TypeSymbol type)
        => type switch
        {
            ErrorType => false,
            NullType => false,
            StringLiteralType { RawStringValue: "" } => false,
            UnionType unionType => unionType.Members.Any(x => IsPotentiallyInsecure(x.Type)),
            _ => !type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure),
        };

    private sealed class Analyzer(SemanticModel model, ResourceTypeResolver resourceTypeResolver)
    {
        private readonly Dictionary<(VariableSymbol, string), bool> variableResults = [];

        /// <summary>
        /// Whether whatever the value supplies at the given path is secure. Paths the value doesn't supply, runtime
        /// values, and empty values are allowed. The container is only used to check for runtime values.
        /// </summary>
        public bool IsSecureAtPath(ObjectPropertySyntax container, SyntaxBase value, ImmutableArray<string> path)
            => SecureValueFlow.GetDataLeaves(value).All(leaf => IsSecureLeafAtPath(container, leaf, path));

        private bool IsSecureLeafAtPath(ObjectPropertySyntax container, SyntaxBase leaf, ImmutableArray<string> path)
        {
            // An object, array, or loop constructed in place supplies only what's placed in it.
            switch (leaf)
            {
                case ObjectSyntax @object:
                    var key = path.IsEmpty ? null : path[0];
                    var memberPath = path.IsEmpty ? path : path.RemoveAt(0);
                    return SecureValueFlow.GetObjectChildren(model, @object).All(entry => entry.Child switch
                    {
                        _ when key is not null && entry.OverriddenProperties.Contains(key) => true,
                        ObjectPropertySyntax property when property.TryGetKeyText() is { } name && entry.OverriddenProperties.Contains(name) => true,
                        ObjectPropertySyntax property when key is null => IsSecureAtPath(container, property.Value, path),
                        ObjectPropertySyntax property when property.TryGetKeyText() is not { } name || SecureValueFlow.SegmentsMatch(name, key)
                            => IsSecureAtPath(container, property.Value, memberPath),
                        ObjectPropertySyntax => true,
                        SpreadExpressionSyntax spread when key is null => IsSecureObjectRemainder(container, spread.Expression, entry.OverriddenProperties),
                        SpreadExpressionSyntax spread => IsSecureAtPath(container, spread.Expression, path),
                        _ => true,
                    });

                case ArraySyntax array when path.IsEmpty || SecureValueFlow.IsIndexSegment(path[0]):
                    return IsSecureArrayAtPath(container, array, path);

                case ForSyntax @for when path.IsEmpty || SecureValueFlow.IsIndexSegment(path[0]):
                    return IsSecureAtPath(container, @for.Body, path.IsEmpty ? path : path.RemoveAt(0));
            }

            // A union or concatenation keeps what each argument supplies within it. The whole result is still checked
            // as one computed value.
            if (!path.IsEmpty && SecureValueFlow.TryGetCombinedValues(model, leaf) is { } combined)
            {
                var combinedPath = SecureValueFlow.ForgetPosition(path);
                return combined.All(value => IsSecureAtPath(container, value, combinedPath));
            }

            // Look through a variable at the values it was computed from. Its inferred type can be secure even when
            // one branch isn't, and a runtime branch shouldn't excuse an insecure one.
            if (SecureValueFlow.GetAccessPath(model, leaf) is (VariableSymbol variable, var accessPath))
            {
                return IsVariableSecureAtPath(container, variable, accessPath.AddRange(path));
            }

            if (SecureValueFlow.IsReadFromSecureValue(model, leaf))
            {
                return true;
            }

            var type = SecureValueFlow.GetEffectiveType(model, leaf);
            if (path.IsEmpty
                ? !IsPotentiallyInsecure(type)
                : type is ErrorType or NullType || SecureValueFlow.IsSecureAtPath(type, path, absentIsSecure: true))
            {
                return true;
            }

            // Runtime values aren't caller inputs. The main thing we are trying to block is hard-coded values,
            // insecure parameters, and values calculated from these.
            return IsRuntimeValue(container, leaf);
        }

        private bool IsSecureObjectRemainder(ObjectPropertySyntax container, SyntaxBase value, ImmutableHashSet<string> overridden)
            => SecureValueFlow.GetDataLeaves(value).All(leaf =>
                TypeHelper.SatisfiesCondition(SecureValueFlow.GetEffectiveType(model, leaf), type => type switch
                {
                    NullType => true,
                    ObjectType @object when !overridden.IsEmpty && !SecureValueFlow.IsSecureType(@object)
                        => @object.Properties.Values
                            .Where(property => !overridden.Contains(property.Name))
                            .All(property => IsSecureAtPath(container, leaf, [property.Name])) &&
                            (SecureValueFlow.TryGetAdditionalPropertiesType(@object) is null || IsSecureAtPath(container, leaf, [SecureValueFlow.AnyMember])),
                    _ => IsSecureAtPath(container, leaf, []),
                }));

        private bool IsSecureArrayAtPath(ObjectPropertySyntax container, ArraySyntax array, ImmutableArray<string> path)
        {
            var target = path.IsEmpty ? null : SecureValueFlow.TryParseIndex(path[0]);
            var itemPath = path.IsEmpty ? path : path.RemoveAt(0);
            int? position = 0;
            foreach (var child in array.Children)
            {
                if (child is ArrayItemSyntax item)
                {
                    if ((target is null || position is null || position == target) && !IsSecureAtPath(container, item.Value, itemPath))
                    {
                        return false;
                    }

                    position++;
                }
                else if (child is SpreadExpressionSyntax spread)
                {
                    // Any item of the spread can land at the position being checked, unless it's before the spread.
                    var spreadPath = path.IsEmpty ? path : itemPath.Insert(0, SecureValueFlow.AnyIndex);
                    if ((target is null || position is null || target >= position) && !IsSecureAtPath(container, spread.Expression, spreadPath))
                    {
                        return false;
                    }

                    position = null;
                }
            }

            return true;
        }

        private bool IsVariableSecureAtPath(ObjectPropertySyntax container, VariableSymbol variable, ImmutableArray<string> path)
        {
            if (model.Binder.TryGetCycle(variable) is not null)
            {
                // A cycle is already an error.
                return true;
            }

            var key = (variable, SecureValueFlow.GetPathKey(path));
            if (!variableResults.TryGetValue(key, out var result))
            {
                result = IsSecureAtPath(container, variable.DeclaringVariable.Value, path);
                variableResults[key] = result;
            }

            return result;
        }

        private bool IsRuntimeValue(ObjectPropertySyntax container, SyntaxBase value)
        {
            var diagWriter = ToListDiagnosticWriter.Create();
            DeployTimeConstantValidator.CheckDeployTimeConstantViolations(container, value, model, diagWriter, resourceTypeResolver);

            return diagWriter.GetDiagnostics().Count > 0;
        }
    }
}
