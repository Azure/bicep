// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Azure.Bicep.Types.Index;

namespace Bicep.Local.Extension.Types;

public record TypeDefinition(string IndexFileContent, ImmutableDictionary<string, string> TypeFileContents);

public interface ITypeDefinitionBuilder
{
    TypeDefinition GenerateTypeDefinition();
}
