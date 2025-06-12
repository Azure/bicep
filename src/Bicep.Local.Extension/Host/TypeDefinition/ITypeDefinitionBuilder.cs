// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Index;

namespace Bicep.Local.Extension.Host.TypeDefinition;

public record TypeDefinition(string TypesJson, string IndexJson);

public interface ITypeDefinitionBuilder
{
    TypeSettings Settings { get; }
    TypeDefinition GenerateBicepResourceTypes();
}
