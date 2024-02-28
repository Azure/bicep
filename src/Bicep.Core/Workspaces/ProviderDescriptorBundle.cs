// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers;
using System.Collections.Immutable;

namespace Bicep.Core.Workspaces;

public record ProviderDescriptorBundle(
    ImmutableArray<ResultWithDiagnostic<ResourceTypesProviderDescriptor>> ImplicitProviders,
    ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>> ExplicitProviderLookup);

public class ProviderDescriptorBundleBuilder
{
    public ProviderDescriptorBundleBuilder(ImmutableArray<ResultWithDiagnostic<ResourceTypesProviderDescriptor>> implicitProviders, ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>> explicitProviderLookup)
    {
        this.implicitProviders = implicitProviders.ToBuilder();
        this.explicitProviderLookup = explicitProviderLookup.ToBuilder();
    }

    public ProviderDescriptorBundleBuilder()
    {
        implicitProviders = ImmutableArray.CreateBuilder<ResultWithDiagnostic<ResourceTypesProviderDescriptor>>();
        explicitProviderLookup = ImmutableDictionary.CreateBuilder<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>>();
    }

    private readonly ImmutableArray<ResultWithDiagnostic<ResourceTypesProviderDescriptor>>.Builder implicitProviders;

    private readonly ImmutableDictionary<ProviderDeclarationSyntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor>>.Builder explicitProviderLookup;

    public void AddImplicitProvider(ResultWithDiagnostic<ResourceTypesProviderDescriptor> descriptor) => implicitProviders.Add(descriptor);

    public void AddOrUpdateExplicitProvider(ProviderDeclarationSyntax syntax, ResultWithDiagnostic<ResourceTypesProviderDescriptor> descriptor)
    {
        if (explicitProviderLookup.ContainsKey(syntax))
        {
            explicitProviderLookup.Remove(syntax);
        }
        explicitProviderLookup.Add(syntax, descriptor);
    }
    public ProviderDescriptorBundle Build()
        => new(implicitProviders.ToImmutableArray(), explicitProviderLookup.ToImmutableDictionary(pair => pair.Key, pair => pair.Value));
}
