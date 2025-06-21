// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Types;
public class TypeProvider
    : ITypeProvider
{
    private readonly Assembly[] assemblies;

    public TypeProvider(Assembly[]? assemblies = null)
    {
        this.assemblies = assemblies ?? GetAssembliesInReferenceScope();
    }

    private static Assembly[] GetAssembliesInReferenceScope()
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        return executingAssembly
                .GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Append(executingAssembly)
                .ToArray();
    }

    /// <summary>
    /// Provides resource type discovery for Bicep extensions by scanning loaded assemblies for types
    /// annotated with <see cref="BicepTypeAttribute"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="TypeProvider"/> implements <see cref="ITypeProvider"/> and returns all public or nested public types
    /// decorated with <see cref="BicepTypeAttribute"/> from the current application domain.
    /// This enables dynamic discovery of resource types for use in Bicep extension scenarios.
    /// </remarks>
    public virtual Type[] GetResourceTypes()
    {
        var types = new Dictionary<string, Type>();

        assemblies
            .SelectMany(assembly =>
            {
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch
                {
                    // if the assembly is unloadable return an empty list
                    assemblyTypes = [];
                }
                return assemblyTypes;
            })
            .Where(type =>
            {
                // filter types that have the BicepTypeAttribute
                var bicepType = type.GetCustomAttributes(typeof(BicepTypeAttribute), true).FirstOrDefault();

                return bicepType is not null && (type.IsPublic || type.IsNestedPublic);
            })
            .Select(type => types.TryAdd(type.Name, type))
            .ToList();

        return types.Values.ToArray();
    }
}
