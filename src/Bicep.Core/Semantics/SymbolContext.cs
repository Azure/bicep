// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.Core.Semantics
{
    public sealed class SymbolContext : ISymbolContext
    {
        private readonly Compilation compilation;
        private readonly SemanticModel semanticModel;
        private bool unlocked;

        public SymbolContext(Compilation compilation, SemanticModel semanticModel)
        {
            this.compilation = compilation;
            this.semanticModel = semanticModel;
        }

        public ITypeManager TypeManager
        {
            get
            {
                this.CheckLock();
                return this.semanticModel.TypeManager;
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