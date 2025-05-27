// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Local.Deploy.Extensibility;

public class GrpcLocalExtensionFactory : ILocalExtensionFactory
{
    Task<ILocalExtension> ILocalExtensionFactory.Start(Uri pathToBinary)
        => GrpcLocalExtension.Start(pathToBinary);
}
