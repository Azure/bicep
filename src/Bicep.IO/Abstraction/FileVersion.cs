// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.IO.Abstraction;

/// <summary>
/// Identifies one version of a file's content, so that a cache can tell whether a file changed since it was read.
/// </summary>
/// <param name="Timestamp">On disk, the last write time in UTC ticks. In memory, a counter that increases on every write.</param>
/// <param name="Length">The file size in bytes. Compared as well, because some file systems record write times only to the second.</param>
public readonly record struct FileVersion(long Timestamp, long Length);
