// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.TypeSystem;

public class DiscriminatedObjectTypeBuilder
{
    private readonly ImmutableHashSet<ObjectType>.Builder members = ImmutableHashSet.CreateBuilder<ObjectType>();
    private readonly Dictionary<string, HashSet<string>> discriminatorCandidates = new();
    private readonly string? requiredDiscriminator;

    public DiscriminatedObjectTypeBuilder(string? requiredDiscriminator = null)
    {
        this.requiredDiscriminator = requiredDiscriminator;
    }

    public bool TryInclude(ObjectType @object)
    {
        if (!members.Add(@object))
        {
            return true;
        }

        if (members.Count == 1)
        {
            var foundViableDiscriminator = false;

            foreach (var (name, property) in @object.Properties)
            {
                if (requiredDiscriminator is not null && name != requiredDiscriminator)
                {
                    continue;
                }

                if (property.Flags.HasFlag(TypePropertyFlags.Required) && property.TypeReference.Type is StringLiteralType { RawStringValue: string discriminatorValue })
                {
                    foundViableDiscriminator = true;
                    discriminatorCandidates.Add(name, new(StringComparer.OrdinalIgnoreCase) { discriminatorValue });
                }
            }

            if (!foundViableDiscriminator)
            {
                return false;
            }
        }
        else
        {
            HashSet<string> noLongerViableDiscriminators = new();
            Dictionary<string, string> stillViableDiscriminators = new();

            foreach (var candidate in discriminatorCandidates.Keys)
            {
                // Eliminate any discriminator candidates for which:
                if (!@object.Properties.TryGetValue(candidate, out var property) || // the property is not defined in @object, or
                    !property.Flags.HasFlag(TypePropertyFlags.Required) || // the property is optional in @object, or
                    property.TypeReference.Type is not StringLiteralType { RawStringValue: string discriminatorValue } || // the property does not have a string literal type in @object, or
                    discriminatorCandidates[candidate].Contains(discriminatorValue)) // the value of the discriminator has already been claimed by another variant
                {
                    noLongerViableDiscriminators.Add(candidate);
                }
                else
                {
                    stillViableDiscriminators[candidate] = discriminatorValue;
                }
            }

            // narrow the list of discriminator candidates
            foreach (var toEliminate in noLongerViableDiscriminators)
            {
                discriminatorCandidates.Remove(toEliminate);
            }
            foreach (var (name, value) in stillViableDiscriminators)
            {
                discriminatorCandidates[name].Add(value);
            }
        }

        return discriminatorCandidates.Count > 0;
    }

    public (ImmutableHashSet<ObjectType> Members, ImmutableHashSet<string> ViableDiscriminators) Build()
        => new(members.ToImmutable(), [.. discriminatorCandidates.Keys]);
}
