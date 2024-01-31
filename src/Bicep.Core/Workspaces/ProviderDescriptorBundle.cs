// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Bicep.Core.Workspaces;

public record ProviderDescriptorBundle(
    IEnumerable<ResultWithDiagnostic<ProviderDescriptor>> ImplicitProviders,
    ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ProviderDescriptor>> ExplicitProviderLookup);

public class ProviderDescriptorBundleBuilder
{
    public ProviderDescriptorBundleBuilder(IEnumerable<ResultWithDiagnostic<ProviderDescriptor>> builtInProviders, ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ProviderDescriptor>> explicitProviderLookup)
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

    private readonly HashSet<ResultWithDiagnostic<ProviderDescriptor>> implicitProviders = [];

    private readonly ConcurrentDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ProviderDescriptor>> explicitProviderLookup = new();

    public void AddImplicitProvider(ResultWithDiagnostic<ProviderDescriptor> descriptor)
    {
        implicitProviders.Add(descriptor);
    }

    public void AddExplicitProvider(ProviderDeclarationSyntax providerDeclaration, ResultWithDiagnostic<ProviderDescriptor> descriptor)
    {
        explicitProviderLookup.TryAdd(providerDeclaration, descriptor);
    }

    public ProviderDescriptorBundle Build()
        => new(implicitProviders.ToImmutableArray(), explicitProviderLookup.ToImmutableDictionary(pair => pair.Key, pair => pair.Value));
}
