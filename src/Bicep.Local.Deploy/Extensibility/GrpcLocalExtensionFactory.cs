// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Local.Deploy.Extensibility;

public class GrpcLocalExtensionFactory : ILocalExtensionFactory
{
    Task<ILocalExtension> ILocalExtensionFactory.Start(IOUri binaryUri)
        => GrpcLocalExtension.Start(binaryUri);
}
