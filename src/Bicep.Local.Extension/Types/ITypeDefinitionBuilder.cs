// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Index;

namespace Bicep.Local.Extension.Types;

public record TypeDefinition(string TypesJson, string IndexJson);

public interface ITypeDefinitionBuilder
{
    TypeSettings Settings { get; }
    TypeDefinition GenerateBicepResourceTypes();
}
