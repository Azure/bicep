// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Host.Attributes;

namespace Bicep.Local.Extension.Host.TypeDefinitionBuilder;
public class TypeProvider
    : ITypeProvider
{
    public TypeProvider() { }

    public virtual Type[] GetResourceTypes()
    {
        var types = new Dictionary<string, Type>();

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

                if (bicepType is not null && (type.IsPublic || type.IsNestedPublic))
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
