// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.Host.Extensions;
public static class TypeExtensions
{
    public static bool IsGenericTypedResourceHandler(this Type type)
        => type.GetInterfaces().Any(i => !i.IsGenericType && i == typeof(IResourceHandler));


    public static bool TryGetTypedResourceHandlerInterface(this Type type, [NotNullWhen(true)] out Type? resourceHandlerInterface)
    {
        resourceHandlerInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResourceHandler<>));

        return resourceHandlerInterface is not null;
    }
}
