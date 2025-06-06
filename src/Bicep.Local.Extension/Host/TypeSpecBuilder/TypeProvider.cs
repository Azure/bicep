// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Azure.Deployments.Core.Extensions;
using Bicep.Local.Extension.Host.Attributes;
using Bicep.Local.Extension.Host.Handlers;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Bicep.Local.Extension.Host.TypeSpecBuilder;
public class TypeProvider
    : ITypeProvider
{
    private readonly FrozenDictionary<string, TypeResourceHandler> resourceHandlers;


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
            .SelectMany(assembly =>
            {
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch
                {
                    // if the asssembly is unloadable return an empty list
                    assemblyTypes = [];
                }
                return assemblyTypes;
            })
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
