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
using Bicep.Local.Extension.Types.Models;

namespace Bicep.Local.Extension.Types;

public class TypeProvider : ITypeProvider
{
    private readonly Assembly[] assemblies;
    private readonly Type? fallbackType;
    private readonly Lazy<ImmutableArray<ResourceTypeDefinitionDetails>> lazyResourceTypes;

    public TypeProvider(Assembly[]? assemblies = null, FallbackTypeRegistration? fallbackTypeContainer = null)
    {
        this.assemblies = assemblies ?? GetAssembliesInReferenceScope();
        this.fallbackType = fallbackTypeContainer?.FallbackResourceType;

            // lazily cache resource types for potentially subsequent calls
            this.lazyResourceTypes = new Lazy<ImmutableArray<ResourceTypeDefinitionDetails>>(() =>
                        this.assemblies
                            .SelectMany(assembly => assembly.GetTypes())                            
                            .Where(x => x.IsVisible)
                            .Select(x => (Type: x, Attribute: x.GetCustomAttribute<ResourceTypeAttribute>(true)))
                            .Where(x => x.Attribute is not null)
                            .Select(x => new ResourceTypeDefinitionDetails(Type: x.Type, Attribute: x.Attribute!))
                            .ToImmutableArray());
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
    public IEnumerable<ResourceTypeDefinitionDetails> GetResourceTypes(bool throwOnDuplicate = true)
    {
        var resourceTypes = this.lazyResourceTypes.Value;
        
        foreach (var group in resourceTypes.GroupBy(x => x.Attribute.FullName))
        {
            if (throwOnDuplicate && group.Count() > 1)
            {
                throw new InvalidOperationException($"Duplicate resource type names found: {group.Key}. Types: {string.Join(", ", group.Select(x => x.Type.FullName))}");
            }

            yield return group.First();
        }
    }

    /// <summary>
    /// Gets the fallback resource type definition if it is available and marked with the required
    /// ResourceTypeAttribute.
    /// </summary>
    /// <remarks>If a fallback type is specified, this method returns its definition only if it is decorated
    /// with the ResourceTypeAttribute. If the attribute is not present, null is returned.</remarks>
    /// <returns>A ResourceTypeDefinition representing the fallback type if it is valid and has the ResourceTypeAttribute;
    /// otherwise, null.</returns>
    public ResourceTypeDefinitionDetails? GetFallbackType()
    {
        if(this.fallbackType is not null)
        {
            // check that fallback type has ResourceTypeAttribute
            var resourceTypeAttribute = this.fallbackType.GetCustomAttribute<ResourceTypeAttribute>(true);

            // ensure fallback type is public and annotated with ResourceTypeAttribute
            if (this.fallbackType.IsVisible && resourceTypeAttribute is not null)
            {
                return new(Type: this.fallbackType, Attribute: resourceTypeAttribute);
            }                        
        }

        return null;
    }
}
