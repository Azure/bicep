// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Intermediate
{
    public abstract record Operation()
    {
        public abstract void Accept(IOperationVisitor visitor);
    }
}
