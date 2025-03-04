// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
    /// Indicates that this access expression should short-circuit if its base expression evaluates to null due to a
    /// previous safe access.
    /// </summary>
    /// <remarks>
    /// Indicates that the base expression was parenthesized. In Bicep (as in C#), <code>foo.?bar.baz</code> will not
    /// attempt to dereference <code>.baz</code> if <code>foo.?bar</code> evaluates to <code>null</code>. However,
    /// <code>(foo.?bar).baz</code> will raise an error for attempting to dereference a property of <code>null</code>.
    /// </remarks>
    Chained = 1 << 1,

    /// <summary>
    /// The accessed index should be counted from the end of the array, not its beginning.
    /// </summary>
    FromEnd = 1 << 2,
}
