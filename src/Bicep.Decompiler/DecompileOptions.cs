// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Decompiler;

public record DecompileOptions(
    bool AllowMissingParamsAndVars = false,
    bool AllowMissingParamsAndVarsInNestedTemplates = false,
    bool IgnoreTrailingInput = true
);

public record DecompileParamOptions(
    bool IgnoreTrailingInput = true,
    bool IncludeUsingDeclaration = true
);
