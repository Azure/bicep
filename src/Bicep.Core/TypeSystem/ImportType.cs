// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    public class ImportType : TypeSymbol
    {
        public ImportType(string name)
            : base(name)
        {
        }

        public override TypeKind TypeKind => TypeKind.Import;
    }
}