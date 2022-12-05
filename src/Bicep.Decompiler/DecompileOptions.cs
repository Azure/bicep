// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Decompiler;

public record DecompileOptions
{
    public bool AllowMissingParamsAndVars = false;
    public bool AllowMissingParamsAndVarsInNestedTemplates = false;
}
