// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Bicep.Types.Index;

namespace Bicep.Local.Extension.Host.TypeSpecBuilder;
public record TypeSpec(string TypesJson, string IndexJson);

public interface ITypeSpecGenerator
{
    TypeSettings Settings { get; }
    TypeSpec GenerateBicepResourceTypes();
}
