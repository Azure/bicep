// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Deploy.Extensibility;

public interface ILocalExtensionFactory
{
    Task<ILocalExtension> Start(Uri pathToBinary);
}
