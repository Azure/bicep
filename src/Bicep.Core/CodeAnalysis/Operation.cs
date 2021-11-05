// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.CodeAnalysis
{
    public abstract class Operation
    {
        protected Operation()
        {
        }

        public abstract void Accept(IOperationVisitor visitor);
    }
}
