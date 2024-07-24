// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public sealed class SymbolContext : ISymbolContext
    {
        private readonly SemanticModel semanticModel;
        private bool unlocked;

        public SymbolContext(SemanticModel semanticModel)
        {
            this.semanticModel = semanticModel;
        }

        public ITypeManager TypeManager => WithLockCheck(() => this.semanticModel.TypeManager);

        public IBinder Binder => WithLockCheck(() => this.semanticModel.Binder);

        public BicepSourceFile SourceFile => WithLockCheck(() => this.semanticModel.SourceFile);

        public SemanticModel SemanticModel => WithLockCheck(() => this.semanticModel);

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
