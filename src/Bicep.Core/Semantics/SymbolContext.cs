// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Registry;
using Bicep.Core.SourceGraph;

namespace Bicep.Core.Semantics
{
    public sealed class SymbolContext : ISymbolContext
    {
        private readonly SemanticModel semanticModel;
        private bool unlocked;

        public SymbolContext(SemanticModel semanticModel, IArtifactFileLookup sourceFileLookup, ISemanticModelLookup modelLookup, IArtifactReferenceFactory artifactReferenceFactory, BicepSourceFile sourceFile)
        {
            this.semanticModel = semanticModel;
            SourceFileLookup = sourceFileLookup;
            ModelLookup = modelLookup;
            ArtifactReferenceFactory = artifactReferenceFactory;
            SourceFile = sourceFile;
        }

        private SemanticModel SemanticModel => WithLockCheck(() => this.semanticModel);

        public ITypeManager TypeManager => SemanticModel.TypeManager;

        public IBinder Binder => SemanticModel.Binder;

        public BicepSourceFile SourceFile { get; }

        public IArtifactFileLookup SourceFileLookup { get; }

        public ISemanticModelLookup ModelLookup { get; }

        public IArtifactReferenceFactory ArtifactReferenceFactory { get; }

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
