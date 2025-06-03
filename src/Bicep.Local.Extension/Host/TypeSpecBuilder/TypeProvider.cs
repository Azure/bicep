// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Attributes;
using Bicep.Local.Extension.Host.Handlers;

namespace Bicep.Local.Extension.Host.TypeSpecBuilder;
public class TypeProvider
    : ITypeProvider
{
    private readonly IImmutableDictionary<string, TypeResourceHandler> resourceHandlers;

    public TypeProvider(IResourceHandlerFactory resourceHandlerFactory)
    {
        resourceHandlers = resourceHandlerFactory?.TypedResourceHandlers
            ?? throw new ArgumentNullException(nameof(resourceHandlerFactory));
    }

    public virtual Type[] GetResourceTypes()
    {
        var types = new Dictionary<string, Type>();

        if (resourceHandlers?.Count() > 0)
        {
            foreach (var resourceHandler in this.resourceHandlers)
            {
                types.TryAdd(resourceHandler.Key, resourceHandler.Value.Type);
            }
        }

        AppDomain
            .CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type =>
            {
                var bicepType = type.GetCustomAttributes(typeof(BicepTypeAttribute), true).FirstOrDefault();

                if (bicepType is not null)
                {
                    return ((BicepTypeAttribute)bicepType).IsActive;
                }

                return false;
            })
            .Select(type => types.TryAdd(type.Name, type))
            .ToList();

        return types.Values.ToArray();
    }
}
