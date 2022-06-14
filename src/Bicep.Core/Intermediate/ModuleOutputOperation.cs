// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics;

namespace Bicep.Core.Intermediate
{
    public record ModuleOutputOperation(
        ModuleSymbol Symbol,
        IndexReplacementContext? IndexContext,
        Operation PropertyName) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitModuleOutputOperation(this);
    }
}
