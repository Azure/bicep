// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.CodeAnalysis
{
    public class ModuleOutputOperation : Operation
    {
        public ModuleOutputOperation(ModuleSymbol symbol, IndexReplacementContext? indexContext, Operation propertyName)
        {
            Symbol = symbol;
            IndexContext = indexContext;
            PropertyName = propertyName;
        }

        public ModuleSymbol Symbol { get; }

        public IndexReplacementContext? IndexContext { get; }

        public Operation PropertyName { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleOutputOperation(this);
    }
}
