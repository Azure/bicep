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

    /// <summary>
    /// Indicates that the access is part of an unbroken access chain following a safe access expression
    /// and will consequently skip evaluation if the safe access expression evaluated to <code>null</code>
    /// </summary>
    ShortCircuitable = 1 << 1,
}
