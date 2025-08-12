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
using Bicep.Local.Extension.Types.Attributes;

namespace Bicep.Local.Extension.Types;

public class TypeProvider : ITypeProvider
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
    /// annotated with <see cref="ResourceTypeAttribute"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="TypeProvider"/> implements <see cref="ITypeProvider"/> and returns all public or nested public types
    /// decorated with <see cref="ResourceTypeAttribute"/> from the current application domain.
    /// This enables dynamic discovery of resource types for use in Bicep extension scenarios.
    /// </remarks>
    public IEnumerable<(Type type, ResourceTypeAttribute attribute)> GetResourceTypes(bool throwOnDuplicate)
    {
        var result = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsVisible)
            .Select(x => (type: x, attribute: x.GetCustomAttribute<ResourceTypeAttribute>(true)!))
            .Where(x => x.attribute is not null)
            .ToImmutableArray();

        foreach (var group in result.GroupBy(x => x.attribute.FullName))
        {
            if (throwOnDuplicate && group.Count() > 1)
            {
                throw new InvalidOperationException($"Duplicate resource type names found: {group.Key}. Types: {string.Join(", ", group.Select(x => x.type.FullName))}");
            }

            yield return group.First();
        }
    }
}
