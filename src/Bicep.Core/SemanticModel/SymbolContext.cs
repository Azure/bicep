// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.SemanticModel
{
    public sealed class SymbolContext : ISymbolContext
    {
        private readonly ITypeManager typeManager;
        private readonly IReadOnlyDictionary<SyntaxBase, Symbol> bindings;
        private readonly Compilation compilation;
        private bool unlocked;

        public SymbolContext(ITypeManager typeManager, IReadOnlyDictionary<SyntaxBase,Symbol> bindings, Compilation compilation)
        {
            this.typeManager = typeManager;
            this.bindings = bindings;
            this.compilation = compilation;
        }

        public ITypeManager TypeManager
        {
            get
            {
                this.CheckLock();
                return this.typeManager;
            }
        }

        public IReadOnlyDictionary<SyntaxBase, Symbol> Bindings
        {
            get
            {
                this.CheckLock();
                return this.bindings;
            }
        }

        public Compilation Compilation
        {
            get
            {
                this.CheckLock();
                return this.compilation;
            }
        }

        public void Unlock() => this.unlocked = true;

        private void CheckLock()
        {
            if (this.unlocked == false)
            {
                throw new InvalidOperationException("Properties of the symbol context should not be accessed until name binding is completed.");
            }
        }
    }
}