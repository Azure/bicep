// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem.Types
{
    public abstract class ObjectLikeType : TypeSymbol
    {
        protected ObjectLikeType(string name, TypeSymbolValidationFlags validationFlags)
            : base(name)
        {
            this.ValidationFlags = validationFlags;
        }

        public override TypeSymbolValidationFlags ValidationFlags { get; }
    }
}
