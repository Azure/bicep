// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;

namespace Bicep.Core.Features;

/// <summary>
/// Host-level settings controlling how compiler version constraint (bicep.version) violations are
/// surfaced, which differs depending on which process is hosting the compiler (e.g. CLI vs. VS Code
/// language server).
/// </summary>
public record CompilerVersionCheckOptions(DiagnosticLevel ConstraintViolationLevel = DiagnosticLevel.Error)
{
    public static readonly CompilerVersionCheckOptions Default = new();
}
