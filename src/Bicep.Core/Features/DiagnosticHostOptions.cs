// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Features;

/// <summary>
/// Host-level settings for diagnostic behavior
/// that differs depending on which process is hosting the compiler (e.g. CLI vs. VS Code language server).
/// </summary>
public record DiagnosticHostOptions(bool SuppressVersionMismatchCascade = false)
{
    public static readonly DiagnosticHostOptions Default = new();
}
