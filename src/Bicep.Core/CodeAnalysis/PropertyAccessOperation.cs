// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.CodeAnalysis
{
    public class PropertyAccessOperation : Operation
    {
        public PropertyAccessOperation(Operation @base, string propertyName)
        {
            Base = @base;
            PropertyName = propertyName;
        }

        public Operation Base { get; }

        public string PropertyName { get; }

        public override void Accept(IOperationVisitor visitor)
            => visitor.VisitPropertyAccessOperation(this);
    }
}
