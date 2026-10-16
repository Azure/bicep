// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Text;

namespace Bicep.Core.Diagnostics;

/// <summary>
/// Represents a related location for a diagnostic.
/// </summary>
/// <param name="FileUri">URI of the related file.</param>
/// <param name="Range">Range in the related file, using 0-based lines and columns.</param>
/// <param name="Message">Human-readable message describing the relation.</param>
public record DiagnosticRelatedInformation(Uri FileUri, TextRange Range, string Message);