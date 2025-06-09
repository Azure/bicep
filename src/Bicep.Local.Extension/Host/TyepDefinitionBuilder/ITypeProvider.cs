// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.TypeDefinitionBuilder;
public interface ITypeProvider
{
    Type[] GetResourceTypes();
}
