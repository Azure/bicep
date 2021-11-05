// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;

namespace Bicep.Core.CodeAnalysis
{
    public class ModuleReferenceOperation : Operation
    {
        public ModuleReferenceOperation(ModuleSymbol symbol, IndexReplacementContext? indexContext)
        {
            Symbol = symbol;
        }

        public ModuleSymbol Symbol { get; }

        public IndexReplacementContext? IndexContext { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleReferenceOperation(this);
    }
}
