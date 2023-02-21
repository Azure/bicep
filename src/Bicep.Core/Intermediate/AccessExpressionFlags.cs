// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Bicep.Core.Intermediate;

[Flags]
public enum AccessExpressionFlags
{
    /// <summary>
    /// No flags specified.
    /// </summary>
    None = 0,

    /// <summary>
    /// The access used a safe dereference operator (<code>.?</code> or <code>[?]</code>).
    /// </summary>
    SafeAccess = 1 << 0,
}
