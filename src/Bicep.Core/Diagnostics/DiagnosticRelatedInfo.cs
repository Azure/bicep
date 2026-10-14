// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Diagnostics;

/// <summary>
/// Represents a related location for a diagnostic.
/// </summary>
/// <param name="FileUri">URI of the related file.</param>
/// <param name="Line">0-based line number in the related file.</param>
/// <param name="Message">Human-readable message describing the relation.</param>
public record DiagnosticRelatedInfo(Uri FileUri, int Line, string Message);