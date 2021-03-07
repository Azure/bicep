// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.TypeSystem
{
    class KeyVaultSecretReferenceType : ObjectType
    {
        public KeyVaultSecretReferenceType() : base("keyVaultSecretReference")
        {
        }

        public override TypeKind TypeKind => TypeKind.KeyVaultSecretReference;
    }
}
