﻿using System;
using System.Collections.Generic;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;

namespace Bicep.Core.SemanticModel
{
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

        public virtual IEnumerable<ErrorDiagnostic> GetDiagnostics()
        {
            yield break;
        }

        protected ErrorDiagnostic CreateError(IPositionable positionable, DiagnosticBuilder.BuildDelegate errorFunc)
            => errorFunc(DiagnosticBuilder.ForPosition(positionable));
    }
}