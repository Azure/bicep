// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Bicep.Local.Deploy.Types;

public record TypeFiles(string IndexFileContent, ImmutableDictionary<string, string> TypeFileContents);
