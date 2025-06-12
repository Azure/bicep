// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.TypeDefinition;
public interface ITypeProvider
{
    Type[] GetResourceTypes();
}
