// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.Utils;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// A type that must be parameterized in order to be used.
/// </summary>
public class TypeTemplate : TypeSymbol
{
    public delegate ResultWithDiagnostic<TypeExpression> InstantiatorDelegate(
        IBinder binder,
        ParameterizedTypeInstantiationSyntaxBase instantiationSyntax,
        ImmutableArray<TypeSymbol> argumentTypes);

    private readonly InstantiatorDelegate instantiator;

    public TypeTemplate(string name, ImmutableArray<TypeParameter> parameters, InstantiatorDelegate instantiator)
        : base($"{name}<{string.Join(", ", parameters)}>")
    {
        Debug.Assert(!parameters.IsEmpty, "Parameterized types must accept at least one argument.");

        UnparameterizedName = name;
        Parameters = parameters;
        MinimumArgumentCount = Parameters.TakeWhile(p => p.Required).Count();
        MaximumArgumentCount = Parameters.Length;
        this.instantiator = instantiator;
    }

    public string UnparameterizedName { get; }

    public ImmutableArray<TypeParameter> Parameters { get; }

    public int MinimumArgumentCount { get; }

    public int MaximumArgumentCount { get; }

    public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

    public override TypeKind TypeKind => TypeKind.TypeReference;

    public override IEnumerable<Diagnostic> GetDiagnostics() => ImmutableArray<Diagnostic>.Empty;

    public TypeParameter? TryGetParameterByIndex(int index) => index < Parameters.Length
        ? Parameters[index]
        : null;

    public ResultWithDiagnostic<TypeExpression> Instantiate(IBinder binder, ParameterizedTypeInstantiationSyntaxBase syntax, IEnumerable<TypeSymbol> arguments)
    {
        var argTypesArray = arguments.ToImmutableArray();

        if (argTypesArray.Length < MinimumArgumentCount || argTypesArray.Length > MaximumArgumentCount)
        {
            return new(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.OpenChevron, syntax.CloseChevron))
                .ArgumentCountMismatch(argTypesArray.Length, MinimumArgumentCount, MaximumArgumentCount));
        }

        for (int i = 0; i < argTypesArray.Length; i++)
        {
            if (Parameters[i].Type is TypeSymbol argumentBound && !TypeValidator.AreTypesAssignable(argTypesArray[i], argumentBound))
            {
                return new(DiagnosticBuilder.ForPosition(syntax.Arguments[i])
                    .ArgumentTypeMismatch(argTypesArray[i], argumentBound));
            }
        }

        return instantiator.Invoke(binder, syntax, argTypesArray);
    }
}
