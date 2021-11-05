// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.CodeAnalysis
{
    public class ModuleNameOperation : Operation
    {
        public ModuleNameOperation(ModuleSymbol symbol, IndexReplacementContext? indexContext)
        {
            Symbol = symbol;
            IndexContext = indexContext;
        }

        public ModuleSymbol Symbol { get; }

        public IndexReplacementContext? IndexContext { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleNameOperation(this);
    }
}
