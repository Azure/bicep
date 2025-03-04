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
    private static readonly StringComparer PointerSegmentComparer = StringComparer.OrdinalIgnoreCase;

    private readonly Stack<TypeSymbol> processing = new();
    private readonly IFeatureProvider features;
    private readonly IBinder binder;

    public ResourceDerivedTypeDiagnosticReporter(IFeatureProvider features, IBinder binder)
    {
        this.features = features;
        this.binder = binder;
    }

    public IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(ITypeReference @ref) => @ref switch
    {
        UnparsableResourceDerivedType unloadable => ReportResourceDerivedTypeDiagnostics(unloadable),
        _ => ReportResourceDerivedTypeDiagnostics(@ref.Type),
    };

    private IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(TypeSymbol typeSymbol)
    {
        if (processing.Contains(typeSymbol))
        {
            yield break;
        }

        processing.Push(typeSymbol);

        foreach (var diagnostic in typeSymbol switch
        {
            IUnresolvedResourceDerivedType resourceDerivedType => ReportResourceDerivedTypeDiagnostics(resourceDerivedType),
            TupleType tuple => tuple.Items.SelectMany(ReportResourceDerivedTypeDiagnostics),
            ArrayType array => ReportResourceDerivedTypeDiagnostics(array.Item),
            DiscriminatedObjectType taggedUnion => taggedUnion.UnionMembersByKey.Values.SelectMany(ReportResourceDerivedTypeDiagnostics),
            ObjectType @object => @object.Properties.Values.Select(property => property.TypeReference).Append(@object.AdditionalProperties?.TypeReference)
                .WhereNotNull()
                .SelectMany(ReportResourceDerivedTypeDiagnostics),
            UnionType union => union.Members.SelectMany(ReportResourceDerivedTypeDiagnostics),
            TypeType typeType => ReportResourceDerivedTypeDiagnostics(typeType.Unwrapped),
            LambdaType lambda => lambda.ArgumentTypes.Concat(lambda.OptionalArgumentTypes).Concat(lambda.ReturnType).SelectMany(ReportResourceDerivedTypeDiagnostics),
            _ => [],
        })
        {
            yield return diagnostic;
        }

        processing.Pop();
    }

    private IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(IUnresolvedResourceDerivedType unbound)
    {
        // TODO support types derived from resources other than the `az` extension. This will require some refactoring of how extension artifacts are restored
        var bound = binder.NamespaceResolver.GetMatchingResourceTypes(unbound.TypeReference, ResourceTypeGenerationFlags.None)
            .Where(resourceType => LanguageConstants.IdentifierComparer.Equals(resourceType.DeclaringNamespace.ExtensionName, AzNamespaceType.BuiltInName))
            .FirstOrDefault();

        if (bound is null || !bound.DeclaringNamespace.ResourceTypeProvider.HasDefinedType(unbound.TypeReference))
        {
            yield return x => x.ResourceTypesUnavailable(unbound.TypeReference);
        }
        else
        {
            var current = bound.Body.Type;
            for (int i = 0; i < unbound.PointerSegments.Length; i++)
            {
                if (PointerSegmentComparer.Equals("properties", unbound.PointerSegments[i]))
                {
                    if (current is not ObjectType @object)
                    {
                        yield return x => x.ObjectRequiredForPropertyAccess(current).WithMaximumDiagnosticLevel(DiagnosticLevel.Warning);
                        break;
                    }

                    var propertyName = unbound.PointerSegments[++i];
                    if (TryGetNamedProperty(@object, propertyName) is not TypeProperty namedProperty)
                    {
                        yield return TypeHelper.GetUnknownPropertyDiagnostic(@object, propertyName, shouldWarn: true);
                        break;
                    }

                    current = namedProperty.TypeReference.Type;
                }
                else if (PointerSegmentComparer.Equals("additionalProperties", unbound.PointerSegments[i]))
                {
                    if (current is not ObjectType @object || @object.AdditionalProperties is null)
                    {
                        yield return x => x.ExplicitAdditionalPropertiesTypeRequiredForAccessThereto(current).WithMaximumDiagnosticLevel(DiagnosticLevel.Warning);
                        break;
                    }

                    current = @object.AdditionalProperties.TypeReference.Type;
                    continue;
                }
                else if (PointerSegmentComparer.Equals("prefixItems", unbound.PointerSegments[i]))
                {
                    if (current is not TupleType tuple)
                    {
                        yield return x => x.TupleRequiredForIndexAccess(current).WithMaximumDiagnosticLevel(DiagnosticLevel.Warning);
                        break;
                    }

                    if (!int.TryParse(unbound.PointerSegments[++i], out int index))
                    {
                        yield return x => x.InvalidInteger().WithMaximumDiagnosticLevel(DiagnosticLevel.Warning);
                        break;
                    }

                    if (index < 0 || tuple.Items.Length <= index)
                    {
                        yield return x => x.IndexOutOfBounds(tuple.Name, tuple.Items.Length, index).WithMaximumDiagnosticLevel(DiagnosticLevel.Warning);
                        break;
                    }

                    current = tuple.Items[i].Type;
                }
                else if (PointerSegmentComparer.Equals("items", unbound.PointerSegments[i]))
                {
                    if (current is not ArrayType array)
                    {
                        yield return x => x.ExplicitItemsTypeRequiredForAccessThereto().WithMaximumDiagnosticLevel(DiagnosticLevel.Warning);
                        break;
                    }

                    current = array.Item.Type;
                }
                else
                {
                    yield return x => x.UnrecognizedResourceDerivedTypePointerSegment(unbound.PointerSegments[i]);
                    break;
                }
            }
        }
    }

    private static TypeProperty? TryGetNamedProperty(ObjectType @object, string propertyName)
    {
        if (@object.Properties.TryGetValue(propertyName, out var property))
        {
            return property;
        }

        if (@object.Properties.Where(p => PointerSegmentComparer.Equals(propertyName, p.Key)).FirstOrDefault() is { } caseInsensitiveMatch)
        {
            return caseInsensitiveMatch.Value;
        }

        return null;
    }

    private IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> ReportResourceDerivedTypeDiagnostics(UnparsableResourceDerivedType unloadable)
    {
        yield return x => x.InvalidResourceTypeIdentifier(unloadable.TypeReferenceString);
    }
}
