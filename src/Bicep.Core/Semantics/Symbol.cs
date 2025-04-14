// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Text;

namespace Bicep.Core.Semantics
{
    [DebuggerDisplay("Name = {" + nameof(Name) + "}, Kind = {" + nameof(Kind) + "}, Type = {GetType().Name}")]
    public abstract class Symbol
    {
        protected Symbol(string name)
        {
            this.Name = name;
        }

        public abstract void Accept(SymbolVisitor visitor);

        public virtual IEnumerable<Symbol> Descendants
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Gets the name of the symbol. Returns an empty string if the symbol is not named.
        /// </summary>
        public string Name { get; }

        public abstract SymbolKind Kind { get; }

        public virtual IEnumerable<IDiagnostic> GetDiagnostics()
        {
            yield break;
        }

        protected Diagnostic CreateDiagnostic(IPositionable positionable, DiagnosticBuilder.DiagnosticBuilderDelegate diagnosticBuilder)
            => diagnosticBuilder(DiagnosticBuilder.ForPosition(positionable));
    }
}
