// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Local.Deploy.Extensibility;

public interface ILocalExtensionFactory
{
    Task<ILocalExtension> Start(IOUri pathToBinary);
}
