// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

public class ExpressionEvaluationResult
{
    private ExpressionEvaluationResult(JToken? value, ImmutableArray<IDiagnostic> diagnostics)
    {
        Value = value;
        Diagnostics = diagnostics;
    }

    public JToken? Value { get; }
    public ImmutableArray<IDiagnostic> Diagnostics { get; }

    public static ExpressionEvaluationResult For(JToken value) => new(value, []);

    public static ExpressionEvaluationResult For(IEnumerable<IDiagnostic> diagnostics) => new(null, [.. diagnostics]);
}
