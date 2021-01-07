// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.FileSystem
{
    public interface IFileResolver
    {
        /// <summary>
        /// Tries to read a file contents to string. If an exception is encoutered, returns null and sets a non-null failureMessage.
        /// </summary>
        /// <param name="fileUri">The file URI to read.</param>
        /// <param name="fileContents">The contents of the file, if successful.</param>
        /// <param name="failureBuilder">Builder for the failure to return, if unsuccessful.</param>
        bool TryRead(Uri fileUri, [NotNullWhen(true)] out string? fileContents, [NotNullWhen(false)] out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder);

        /// <summary>
        /// Tries to resolve a child file path relative to a parent module file path.
        /// </summary>
        /// <param name="parentFileUri">The file URI of the parent.</param>
        /// <param name="childFilePath">The file path of the child.</param>
        Uri? TryResolveModulePath(Uri parentFileUri, string childFilePath);
        
        // Uri 
        // IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern);
        // IEnumerable<Uri> GetFiles(Uri fileUri, string pattern); 
        // Uri GetParentDirectory(Uri fileUri);
        // bool DirExists(Uri fileUri);
        // bool FileExists(Uri fileUri);
    }
}