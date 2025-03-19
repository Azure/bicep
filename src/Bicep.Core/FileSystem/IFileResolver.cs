// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.FileSystem;

public interface IFileResolver
{
    /// <summary>
    /// Tries to read a file contents to string. If an exception is encountered, returns null and sets a non-null failureMessage.
    /// </summary>
    /// <param name="fileUri">The file URI to read.</param>
    ResultWithDiagnosticBuilder<string> TryRead(Uri fileUri);

    /// <summary>
    /// Tries to resolve a child file path relative to a parent module file path.
    /// </summary>
    /// <param name="parentFileUri">The file URI of the parent.</param>
    /// <param name="childFilePath">The file path of the child.</param>
    Uri? TryResolveFilePath(Uri parentFileUri, string childFilePath);
}
