// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Configuration;

/// <summary>
/// A configuration diagnostic and the configuration file it belongs to.
/// <see cref="File"/> is null when no single file caused it (for example, an invalid environment variable).
/// </summary>
public sealed record ConfigurationDiagnostic(IOUri? File, IDiagnostic Diagnostic);
