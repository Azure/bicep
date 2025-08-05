// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.Host;

public interface IResourceHandlerCollection
{
    IResourceHandler? TryGetHandler(string type, string? apiVersion);
}
