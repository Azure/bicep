// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Intermediate
{
    public record ImportOperation(
        string AliasName,
        NamespaceType NamespaceType,
        Operation? Config) : Operation
    {
        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitImportOperation(this);
    }
}
