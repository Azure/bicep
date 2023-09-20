// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Workspaces;

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

        public ITypeManager TypeManager => WithLockCheck(() => this.semanticModel.TypeManager);

        public Compilation Compilation => WithLockCheck(() => this.compilation);

        public IBinder Binder => WithLockCheck(() => this.semanticModel.Binder);

        public BicepSourceFile SourceFile => WithLockCheck(() => this.semanticModel.SourceFile);

        public void Unlock() => this.unlocked = true;

        private T WithLockCheck<T>(Func<T> getFunc)
        {
            if (!this.unlocked)
            {
                throw new InvalidOperationException("Properties of the symbol context should not be accessed until name binding is completed.");
            }

            return getFunc();
        }
    }
}
