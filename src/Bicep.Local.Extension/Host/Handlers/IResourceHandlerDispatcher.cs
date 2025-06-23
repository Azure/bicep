// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Frozen;

namespace Bicep.Local.Extension.Host.Handlers;

public record TypeResourceHandler(Type Type, IResourceHandler Handler);

public interface IResourceHandlerDispatcher
{
    FrozenDictionary<string, TypeResourceHandler>? TypedResourceHandlers { get; }
    TypeResourceHandler? GenericResourceHandler { get; }
    TypeResourceHandler GetResourceHandler(string resourceType);
    TypeResourceHandler GetResourceHandler(Type resourceType);
}
