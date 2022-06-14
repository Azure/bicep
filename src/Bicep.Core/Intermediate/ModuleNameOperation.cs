// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.Intermediate
{
    public record ModuleNameOperation(
        ModuleSymbol Symbol,
        IndexReplacementContext? IndexContext) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleNameOperation(this);
    }
}
