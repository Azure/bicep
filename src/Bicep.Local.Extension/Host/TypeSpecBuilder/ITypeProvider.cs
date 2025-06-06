// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Host.TypeSpecBuilder;
public interface ITypeProvider
{
    Type[] GetResourceTypes();
}
