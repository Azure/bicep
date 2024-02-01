// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Bicep.Core.Workspaces;

public record ProviderDescriptorBundle(
    IEnumerable<ResultWithDiagnostic<ResourceTypesProviderDescriptor>> ImplicitProviders,
    ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>> ExplicitProviderLookup);

public class ProviderDescriptorBundleBuilder
{
    public ProviderDescriptorBundleBuilder(IEnumerable<ResultWithDiagnostic<ResourceTypesProviderDescriptor>> builtInProviders, ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>> explicitProviderLookup)
    {
        foreach (var provider in builtInProviders)
        {
            AddImplicitProvider(provider);
        }

        foreach (var (providerDeclaration, providerDescriptor) in explicitProviderLookup)
        {
            AddExplicitProvider(providerDeclaration, providerDescriptor);
        }
    }

    public ProviderDescriptorBundleBuilder() { }

    private readonly HashSet<ResultWithDiagnostic<ResourceTypesProviderDescriptor>> implicitProviders = [];

    private readonly ConcurrentDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>> explicitProviderLookup = new();

    public void AddImplicitProvider(ResultWithDiagnostic<ResourceTypesProviderDescriptor> descriptor)
    {
        implicitProviders.Add(descriptor);
    }

    public void AddExplicitProvider(ProviderDeclarationSyntax providerDeclaration, ResultWithDiagnostic<ResourceTypesProviderDescriptor> descriptor)
    {
        explicitProviderLookup.TryAdd(providerDeclaration, descriptor);
    }

    public ProviderDescriptorBundle Build()
        => new(implicitProviders.ToImmutableArray(), explicitProviderLookup.ToImmutableDictionary(pair => pair.Key, pair => pair.Value));
}
