// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.SemanticModel
{
    /// <summary>
    /// Represents a name binding failure.
    /// </summary>
    public class ErrorSymbol : Symbol
    {
        private readonly ErrorDiagnostic error;

        public ErrorSymbol(ErrorDiagnostic error) : base(error.Code)
        {
            this.error = error;
        }

        public override void Accept(SymbolVisitor visitor) => visitor.VisitErrorSymbol(this);

        public override SymbolKind Kind => SymbolKind.Error;

        public override IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            yield return this.error;
        }
    }
}

