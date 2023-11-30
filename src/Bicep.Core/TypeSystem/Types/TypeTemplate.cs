// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Intermediate;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.TypeSystem.Types;

/// <summary>
/// A type that must be parameterized in order to be used.
/// </summary>
public class TypeTemplate : TypeSymbol
{
    public delegate Result<TypeExpression, ErrorDiagnostic> InstantiatorDelegate(
        IBinder binder,
        ParameterizedTypeInstantiationSyntax instantiationSyntax,
        ImmutableArray<TypeSymbol> argumentTypes);

    private readonly InstantiatorDelegate instantiator;

    public TypeTemplate(string name, ImmutableArray<TypeParameter> parameters, InstantiatorDelegate instantiator)
        : base($"Type<{name}<{string.Join(", ", parameters)}>>")
    {
        Debug.Assert(!parameters.IsEmpty, "Parameterized types must accept at least one argument.");

        Parameters = parameters;
        this.instantiator = instantiator;
    }

    public ImmutableArray<TypeParameter> Parameters { get; }

    public override TypeSymbolValidationFlags ValidationFlags => TypeSymbolValidationFlags.PreventAssignment;

    public override TypeKind TypeKind => TypeKind.TypeReference;

    public override IEnumerable<ErrorDiagnostic> GetDiagnostics() => ImmutableArray<ErrorDiagnostic>.Empty;

    public Result<TypeExpression, ErrorDiagnostic> Instantiate(IBinder binder, ParameterizedTypeInstantiationSyntax syntax, IEnumerable<TypeSymbol> argumentTypes)
    {
        var argTypesArray = argumentTypes.ToImmutableArray();

        if (argTypesArray.Length != Parameters.Length)
        {
            return new(DiagnosticBuilder.ForPosition(TextSpan.Between(syntax.OpenChevron, syntax.CloseChevron))
                .ArgumentCountMismatch(argTypesArray.Length, Parameters.Length, Parameters.Length));
        }

        return instantiator.Invoke(binder, syntax, argTypesArray);
    }
}
