// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules;

/// <summary>
/// Flags secure values passed to module parameters that are not secure. A module's parameter values are stored in
/// clear text in its deployment's inputs, so a secret passed to an insecure parameter can be read back by anyone who
/// can read that deployment.
/// </summary>
/// <remarks>
/// Parameter types come from Bicep's type system. A <c>resourceInput&lt;&gt;</c> type carries secure flags from the
/// resource type, but compiles to a plain <c>object</c> schema whose resource-type link is metadata ARM ignores, so
/// those nested values aren't protected at deployment time. This rule treats them as secure and can miss such leaks.
/// </remarks>
public sealed class NoSecureValuesInInsecureInputsRule : LinterRuleBase
{
    public new const string Code = "no-secure-values-in-insecure-inputs";

    public NoSecureValuesInInsecureInputsRule() : base(
        code: Code,
        description: CoreResources.NoSecureValuesInInsecureInputsRule_Description,
        LinterRuleCategory.Security)
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.NoSecureValuesInInsecureInputsRule_MessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        var analyzer = new Analyzer(model);
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

                var findings = SecureValueFlow.GetInputSlots(model, property.Value, name, sinkType)
                    .Where(slot => slot.SinkType is not ErrorType && !(slot.Prefix.IsEmpty && SecureValueFlow.IsSecureType(slot.SinkType)))
                    .SelectMany(slot => analyzer.GetSecretSources(slot.Value)
                        .Where(source => !IsModuleSecureParameterOnlyFunction(model, source.Syntax))
                        .Where(source => !SecureValueFlow.IsOverwritten(source.Path, slot.OverriddenProperties))
                        .Select(source => (source, path: slot.Prefix.AddRange(source.Path)))
                        .Where(x => !SecureValueFlow.IsSecureAtPath(slot.SinkType, x.path))
                        .Select(x => (x.source.Syntax, Input: slot.Input + SecureValueFlow.FormatPath(x.path), x.source.Found)))
                    .Distinct();

                foreach (var (syntax, input, found) in findings)
                {
                    yield return CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, input, found);
                }
            }
        }
    }

    /// <summary>
    /// Functions like <c>getSecret()</c> can only be assigned directly to a secure module parameter, so the compiler
    /// already reports an error (BCP180) for any use of them that this rule would flag.
    /// </summary>
    private static bool IsModuleSecureParameterOnlyFunction(SemanticModel model, SyntaxBase syntax)
        => syntax is FunctionCallSyntaxBase
            && model.GetSymbolInfo(syntax) is FunctionSymbol function
            && function.FunctionFlags.HasFlag(FunctionFlags.ModuleSecureParameterOnly);

    /// <summary>
    /// A secret within a value, at a path relative to that value.
    /// </summary>
    private record SecretSource(ImmutableArray<string> Path, string Found, SyntaxBase Syntax, bool IsObjectKey = false);

    private sealed class Analyzer(SemanticModel model)
    {
        private readonly Dictionary<VariableSymbol, ImmutableArray<SecretSource>> variableSources = [];
        private readonly Dictionary<LocalVariableSymbol, ImmutableArray<SecretSource>> itemSources = [];

        /// <summary>
        /// Finds every secret a value can contain, and where in the value it is.
        /// </summary>
        public ImmutableArray<SecretSource> GetSecretSources(SyntaxBase syntax)
        {
            IEnumerable<SecretSource> sources = syntax switch
            {
                ParenthesizedExpressionSyntax or NonNullAssertionSyntax or TernaryOperationSyntax or BinaryOperationSyntax { Operator: BinaryOperator.Coalesce }
                    => SecureValueFlow.GetDataLeaves(syntax).SelectMany(leaf => GetSecretSources(leaf)),
                ObjectSyntax @object => SecureValueFlow.GetObjectChildren(model, @object).SelectMany(entry => entry.Child switch
                {
                    ObjectPropertySyntax property when property.TryGetKeyText() is { } name && entry.OverriddenProperties.Contains(name) => [],
                    ObjectPropertySyntax property => GetPropertySources(property),
                    SpreadExpressionSyntax spread => GetSecretSources(spread.Expression)
                        .Where(source => !SecureValueFlow.IsOverwritten(source.Path, entry.OverriddenProperties)),
                    _ => [],
                }),
                ArraySyntax array => GetArraySources(array),
                ForSyntax @for => Prefix(SecureValueFlow.AnyIndex, GetSecretSources(@for.Body)),
                // A projection keeps the shape of its lambda's result, and a filter keeps the items of its input.
                FunctionCallSyntaxBase functionCall when TryGetSystemFunctionArgument(functionCall, "map", 1) is { } mapper && GetLambdaBody(mapper) is { } body
                    => Prefix(SecureValueFlow.AnyIndex, GetSecretSources(body)),
                FunctionCallSyntaxBase functionCall when TryGetSystemFunctionArgument(functionCall, "filter", 0) is { } filtered
                    => AnyPosition(GetSecretSources(filtered)),
                _ when SecureValueFlow.TryGetCombinedValues(model, syntax) is { } combined
                    => combined.SelectMany((value, index) => AnyPosition(GetSecretSources(value)
                        .Where(source => !SecureValueFlow.IsOverwrittenByObjectUnion(model, syntax, index, source.Path)))),
                _ when SecureValueFlow.GetAccessChain(syntax).Last() is VariableAccessSyntax => GetDirectReferenceSources(syntax),
                _ => GetExpressionSources(syntax),
            };

            return [.. sources.DistinctBy(source => (SecureValueFlow.GetPathKey(source.Path), source.Found, source.IsObjectKey))];
        }

        private IEnumerable<SecretSource> GetPropertySources(ObjectPropertySyntax property)
            => Prefix(property.TryGetKeyText() is { } name ? SecureValueFlow.FormatProperty(name) : SecureValueFlow.AnyMember, GetSecretSources(property.Value))
                .Concat(property.Key is StringSyntax { Expressions.Length: > 0 } key
                    ? GetSecretSources(key).Select(source => source with { Path = [], IsObjectKey = true })
                    : []);

        private IEnumerable<SecretSource> GetArraySources(ArraySyntax array)
        {
            int? position = 0;
            foreach (var child in array.Children)
            {
                if (child is ArrayItemSyntax item)
                {
                    var segment = position is { } index ? SecureValueFlow.FormatIndex(index) : SecureValueFlow.AnyIndex;
                    foreach (var source in Prefix(segment, GetSecretSources(item.Value)))
                    {
                        yield return source;
                    }

                    position++;
                }
                else if (child is SpreadExpressionSyntax spread)
                {
                    foreach (var source in AnyPosition(GetSecretSources(spread.Expression)))
                    {
                        yield return source;
                    }

                    position = null;
                }
            }
        }

        private IEnumerable<SecretSource> GetDirectReferenceSources(SyntaxBase syntax)
        {
            var text = syntax.ToString().Trim();
            IEnumerable<SecretSource> typed = SecureValueFlow.IsReadFromSecureValue(model, syntax)
                ? [new([], FoundSecureValue(text), syntax)]
                : SecureValueFlow.FindSecurePaths(SecureValueFlow.GetEffectiveType(model, syntax))
                    .Select(path => new SecretSource(path, FoundSecureValue(text + SecureValueFlow.FormatPath(path)), syntax));

            // A value computed from a secure value (e.g. by interpolation) loses the secure flag, so follow variables,
            // and loop and lambda items back to what they iterate, keeping only what's reachable through this
            // reference's property and index accesses.
            var (root, accessPath) = SecureValueFlow.GetAccessPath(model, syntax);
            var rootSources = root switch
            {
                VariableSymbol variable => GetVariableSources(variable),
                LocalVariableSymbol local => GetItemSources(local),
                _ => [],
            };
            var derived = rootSources
                // Object keys flow with their containing object, but not with a value read from that object.
                .Where(source => !source.IsObjectKey || accessPath.Length <= source.Path.Length)
                .Select(source => SecureValueFlow.TryStripPrefix(source.Path, accessPath) is { } path ? source with { Path = path, Syntax = syntax } : null)
                .OfType<SecretSource>();

            // Prefer the description of the reference itself where both describe the same path.
            return typed.Concat(derived).DistinctBy(source => (SecureValueFlow.GetPathKey(source.Path), source.IsObjectKey));
        }

        /// <summary>
        /// The secrets in each item that a loop or lambda variable receives.
        /// </summary>
        private ImmutableArray<SecretSource> GetItemSources(LocalVariableSymbol local)
        {
            if (!itemSources.TryGetValue(local, out var sources))
            {
                sources = SecureValueFlow.TryGetItemSource(model, local) is { } itemSource
                    ? [.. GetSecretSources(itemSource)
                        .Select(source => source.Path.FirstOrDefault() switch
                        {
                            null => source,
                            var head when SecureValueFlow.IsIndexSegment(head) || head == SecureValueFlow.AnyMember => source with { Path = source.Path.RemoveAt(0) },
                            _ => null,
                        })
                        .OfType<SecretSource>()]
                    : [];
                itemSources[local] = sources;
            }

            return sources;
        }

        private IEnumerable<SecretSource> GetExpressionSources(SyntaxBase syntax)
        {
            // The result's own type may have secure members, e.g. a function returning a user-defined type.
            IEnumerable<SecretSource> typed = SecureValueFlow.IsReadFromSecureValue(model, syntax)
                ? [new([], FoundSecureValue(syntax.ToString().Trim()), syntax)]
                : SecureValueFlow.FindSecurePaths(SecureValueFlow.GetEffectiveType(model, syntax))
                    .Select(path => new SecretSource(path, FoundSecureValue(syntax.ToString().Trim() + SecureValueFlow.FormatPath(path)), syntax));

            // Without a structure to follow, any secret in the expression may end up anywhere in its result.
            var references = ReferenceCollector.Collect(syntax)
                .Where(reference => !ReferenceEquals(reference, syntax))
                .SelectMany(GetDirectReferenceSources)
                .Select(source => source with { Path = [], IsObjectKey = false });

            // The visitor also reports secure references, but without following variables; only take its list*() calls.
            var functions = FindPossibleSecretsVisitor.FindPossibleSecretsInExpression(model, syntax)
                .Where(secret => secret.Syntax is FunctionCallSyntaxBase)
                .Select(secret => new SecretSource([], secret.FoundMessage, secret.Syntax));

            return typed.Concat(references).Concat(functions);
        }

        private ImmutableArray<SecretSource> GetVariableSources(VariableSymbol variable)
        {
            if (model.Binder.TryGetCycle(variable) is not null)
            {
                // A cycle is already an error.
                return [];
            }

            if (!variableSources.TryGetValue(variable, out var sources))
            {
                sources = GetSecretSources(variable.DeclaringVariable.Value);
                variableSources[variable] = sources;
            }

            return sources;
        }

        private SyntaxBase? TryGetSystemFunctionArgument(FunctionCallSyntaxBase functionCall, string name, int index)
            => functionCall.Name.IdentifierName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                index < functionCall.Arguments.Length &&
                SemanticModelHelper.TryGetFunctionInNamespace(model, SystemNamespaceType.BuiltInName, functionCall) is not null
                ? functionCall.Arguments[index].Expression
                : null;
    }

    /// <summary>
    /// Collects the outermost references in an expression: a symbol, or any other expression, followed by property
    /// and index accesses.
    /// </summary>
    private sealed class ReferenceCollector : AstVisitor
    {
        private readonly List<SyntaxBase> references = [];

        public static IReadOnlyList<SyntaxBase> Collect(SyntaxBase syntax)
        {
            var collector = new ReferenceCollector();
            collector.Visit(syntax);
            return collector.references;
        }

        protected override void VisitInternal(SyntaxBase syntax)
        {
            if (syntax is VariableAccessSyntax or AccessExpressionSyntax)
            {
                references.Add(syntax);

                // Anything other than a symbol at the root of an access chain may contain references of its own.
                if (SecureValueFlow.GetAccessChain(syntax).Last() is not VariableAccessSyntax and var root)
                {
                    Visit(root);
                }

                return;
            }

            base.VisitInternal(syntax);
        }

        public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            // The base of an instance function call is a resource or namespace, not data.
            foreach (var argument in syntax.Arguments)
            {
                Visit(argument);
            }
        }
    }

    private static string FoundSecureValue(string text) => string.Format(CoreResources.PossibleSecretMessageSecureParam, text);

    private static IEnumerable<SecretSource> Prefix(string segment, IEnumerable<SecretSource> sources)
        => sources.Select(source => source with { Path = source.Path.Insert(0, segment) });

    /// <summary>
    /// Forgets the positions of array items that may have moved, e.g. after a spread, a filter, or a union.
    /// </summary>
    private static IEnumerable<SecretSource> AnyPosition(IEnumerable<SecretSource> sources)
        => sources.Select(source => source with { Path = SecureValueFlow.ForgetPosition(source.Path) });

    private static SyntaxBase? GetLambdaBody(SyntaxBase syntax) => syntax switch
    {
        LambdaSyntax lambda => lambda.Body,
        TypedLambdaSyntax typedLambda => typedLambda.Body,
        _ => null,
    };
}
