// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Emit;

public sealed partial class ExternalInputFunctionReferenceVisitor : AstVisitor
{
    private readonly SemanticModel semanticModel;
    private ParameterAssignmentSymbol? targetParameterAssignment;
    private VariableSymbol? targetVariableDeclaration;
    private readonly ImmutableHashSet<ParameterAssignmentSymbol>.Builder parametersContainingExternalInput;
    private readonly ImmutableHashSet<VariableSymbol>.Builder variablesContainingExternalInput;
    private readonly ImmutableDictionary<FunctionCallSyntaxBase, string>.Builder externalInputReferences;
    private ExternalInputFunctionReferenceVisitor(SemanticModel semanticModel)
    {
        this.semanticModel = semanticModel;
        this.externalInputReferences = ImmutableDictionary.CreateBuilder<FunctionCallSyntaxBase, string>();
        this.parametersContainingExternalInput = ImmutableHashSet.CreateBuilder<ParameterAssignmentSymbol>();
        this.variablesContainingExternalInput = ImmutableHashSet.CreateBuilder<VariableSymbol>();
    }

    public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
    {
        VisitFunctionCallSyntaxInternal(syntax);
        base.VisitFunctionCallSyntax(syntax);
    }

    public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
    {
        VisitFunctionCallSyntaxInternal(syntax);
        base.VisitInstanceFunctionCallSyntax(syntax);
    }

    public static bool FunctionContainsExternalInputReference(SemanticModel model, DeclaredFunctionSymbol targetSymbol)
    {
        var closure = model.Binder.GetReferencedSymbolClosureFor(targetSymbol).Add(targetSymbol);
        var visited = new HashSet<SyntaxBase>();
        foreach (var symbol in closure)
        {
            if (symbol is DeclaredFunctionSymbol declaredFunctionSymbol)
            {
                var functionCalls = SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(declaredFunctionSymbol.DeclaringFunction.Lambda);
                foreach (var functionCall in functionCalls)
                {
                    if (!visited.Contains(functionCall) &&
                        model.GetSymbolInfo(functionCall) is FunctionSymbol functionSymbol &&
                        functionSymbol.FunctionFlags.HasFlag(FunctionFlags.ExternalInput))
                    {
                        return true;
                    }

                    visited.Add(functionCall);
                }
            }
        }

        return false;
    }

    public static ExternalInputReferences CollectExternalInputReferences(SemanticModel model)
    {
        void ProcessDirectReferences(
            ExternalInputFunctionReferenceVisitor visitor,
            IEnumerable<DeclaredSymbol> declaredSymbols)
        {
            foreach (var symbol in declaredSymbols)
            {
                switch (symbol)
                {
                    case ParameterAssignmentSymbol parameterAssignment:
                        visitor.targetParameterAssignment = parameterAssignment;
                        parameterAssignment.DeclaringParameterAssignment.Accept(visitor);
                        visitor.targetParameterAssignment = null;
                        break;

                    case VariableSymbol variableDeclaration:
                        visitor.targetVariableDeclaration = variableDeclaration;
                        variableDeclaration.DeclaringVariable.Accept(visitor);
                        visitor.targetVariableDeclaration = null;
                        break;
                }
            }
        }

        void ProcessSymbolClosures(
            ExternalInputFunctionReferenceVisitor visitor,
            IEnumerable<DeclaredSymbol> declaredSymbols)
        {
            foreach (var symbol in declaredSymbols)
            {
                var symbolClosure = model.Binder.GetReferencedSymbolClosureFor(symbol);
                if (symbolClosure.Overlaps(visitor.parametersContainingExternalInput) ||
                    symbolClosure.Overlaps(visitor.variablesContainingExternalInput))
                {
                    switch (symbol)
                    {
                        case ParameterAssignmentSymbol parameterAssignment:
                            visitor.parametersContainingExternalInput.Add(parameterAssignment);
                            break;

                        case VariableSymbol variableDeclaration:
                            visitor.variablesContainingExternalInput.Add(variableDeclaration);
                            break;
                    }
                }
            }
        }

        var visitor = new ExternalInputFunctionReferenceVisitor(model);

        // Process the parameter assignments and variable declarations to find any direct references
        // to the external input function.
        ProcessDirectReferences(visitor, model.Root.ParameterAssignments);
        ProcessDirectReferences(visitor, model.Root.VariableDeclarations);

        // Process the symbol closures to find any external input references
        // that are not directly referenced in the parameter or variable declarations.
        ProcessSymbolClosures(visitor, model.Root.ParameterAssignments);
        ProcessSymbolClosures(visitor, model.Root.VariableDeclarations);

        if (model.Root.UsingDeclarationSyntax?.Config is { } config)
        {
            // TODO update visitor to improve this
            config.Accept(visitor);
        }

        return new ExternalInputReferences(
            ParametersReferences: visitor.parametersContainingExternalInput.ToImmutable(),
            VariablesReferences: visitor.variablesContainingExternalInput.ToImmutable(),
            ExternalInputIndexMap: visitor.externalInputReferences.ToImmutable()
        );
    }

    private void VisitFunctionCallSyntaxInternal(FunctionCallSyntaxBase functionCallSyntax)
    {
        if (SemanticModelHelper.TryGetFunctionInNamespace(semanticModel, SystemNamespaceType.BuiltInName, functionCallSyntax) is not { } functionCall)
        {
            return;
        }

        var index = this.externalInputReferences.Count;
        string definitionKey;

        if (functionCall.Name.NameEquals(LanguageConstants.ExternalInputBicepFunctionName) &&
            functionCallSyntax.Arguments.Length >= 1 &&
            semanticModel.GetTypeInfo(functionCallSyntax.Arguments[0]) is StringLiteralType stringLiteral)
        {
            definitionKey = GetExternalInputDefinitionName(stringLiteral.RawStringValue, index);
        }
        else if (functionCall.Name.NameEquals(LanguageConstants.ReadCliArgBicepFunctionName))
        {
            definitionKey = GetExternalInputDefinitionName($"sys.cliArg", index);
        }
        else if (functionCall.Name.NameEquals(LanguageConstants.ReadEnvVarBicepFunctionName))
        {
            definitionKey = GetExternalInputDefinitionName($"sys.envVar", index);
        }
        else
        {
            return;
        }

        this.externalInputReferences.TryAdd(functionCall, definitionKey);

        if (this.targetParameterAssignment is not null)
        {
            this.parametersContainingExternalInput.Add(this.targetParameterAssignment);
        }

        if (this.targetVariableDeclaration is not null)
        {
            this.variablesContainingExternalInput.Add(this.targetVariableDeclaration);
        }
    }

    private static string GetExternalInputDefinitionName(string kind, int index)
    {
        // The name of the external input definition is a combination of the kind and the index.
        // e.g. 'sys.cli' becomes 'sys_cli_0'
        var nonAlphanumericPattern = NonAlphanumericPattern();
        var sanitizedKind = nonAlphanumericPattern.Replace(kind, "_");
        return $"{sanitizedKind}_{index}";
    }

    [GeneratedRegex(@"\W", RegexOptions.Compiled)]
    private static partial Regex NonAlphanumericPattern();
}

public record ExternalInputReferences(
    // parameters that contain external input function calls
    ImmutableHashSet<ParameterAssignmentSymbol> ParametersReferences,
    // variables that contain external input function calls
    ImmutableHashSet<VariableSymbol> VariablesReferences,
    // map of external input function calls to unique keys to be used to construct externalInput definition in parameters.json
    ImmutableDictionary<FunctionCallSyntaxBase, string> ExternalInputIndexMap
);
