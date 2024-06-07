// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.Json.Serialization;

namespace Bicep.Local.Extension.Mock.Handlers;

[JsonSerializable(typeof(EchoRequest))]
[JsonSerializable(typeof(EchoResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class SerializationContext : JsonSerializerContext { }
