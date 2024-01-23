// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

public class ResourceDerivedTypeDiagnosticReporter
{
    private readonly Stack<TypeSymbol> processing = new();
    private readonly IFeatureProvider features;
    private readonly IBinder binder;

    public ResourceDerivedTypeDiagnosticReporter(IFeatureProvider features, IBinder binder)
    {
        this.features = features;
        this.binder = binder;
    }

    public IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(TypeSymbol typeSymbol)
    {
        if (processing.Contains(typeSymbol))
        {
            yield break;
        }

        processing.Push(typeSymbol);

        foreach (var diagnostic in typeSymbol switch
        {
            IUnboundResourceDerivedType resourceDerivedType => ReportResourceDerivedTypeDiagnostics(resourceDerivedType),
            TupleType tuple => tuple.Items.SelectMany(ReportResourceDerivedTypeDiagnostics),
            ArrayType array => ReportResourceDerivedTypeDiagnostics(array.Item.Type),
            DiscriminatedObjectType taggedUnion => taggedUnion.UnionMembersByKey.Values.SelectMany(ReportResourceDerivedTypeDiagnostics),
            ObjectType @object => @object.Properties.Values.Select(property => property.TypeReference).Append(@object.AdditionalPropertiesType)
                .SelectMany(ReportResourceDerivedTypeDiagnostics),
            UnionType union => union.Members.SelectMany(ReportResourceDerivedTypeDiagnostics),
            TypeType typeType => ReportResourceDerivedTypeDiagnostics(typeType.Unwrapped),
            LambdaType lambda => lambda.ArgumentTypes.Append(lambda.ReturnType).SelectMany(ReportResourceDerivedTypeDiagnostics),
            _ => Enumerable.Empty<DiagnosticBuilder.DiagnosticBuilderDelegate>(),
        })
        {
            yield return diagnostic;
        }

        processing.Pop();
    }

    private IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(ITypeReference? typeReference)
        => typeReference is not null ? ReportResourceDerivedTypeDiagnostics(typeReference.Type) : Enumerable.Empty<DiagnosticBuilder.DiagnosticBuilderDelegate>();

    private IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(IUnboundResourceDerivedType unbound)
    {
        if (!features.ResourceDerivedTypesEnabled)
        {
            yield return x => x.ResourceDerivedTypesUnsupported();
        }

        // TODO support types derived from resources other than the `az` provider. This will require some refactoring of how provider artifacts are restored
        var bound = binder.NamespaceResolver.GetMatchingResourceTypes(unbound.TypeReference, ResourceTypeGenerationFlags.None)
            .Where(resourceType => LanguageConstants.IdentifierComparer.Equals(resourceType.DeclaringNamespace.ProviderName, AzNamespaceType.BuiltInName))
            .FirstOrDefault();

        if (bound is null || !bound.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(unbound.TypeReference))
        {
            yield return x => x.ResourceTypesUnavailable(unbound.TypeReference);
        }
    }
}
