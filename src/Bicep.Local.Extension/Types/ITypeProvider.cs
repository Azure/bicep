// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Extension.Types;
public interface ITypeProvider
{
    Type[] GetResourceTypes();
}
