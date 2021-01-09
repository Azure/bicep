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
        

        /// <summary>
        /// Tries to get Directories given a uri and pattern. Both argument and returned URIs MUST have a trailing '/'
        /// </summary>
        /// <param name="fileUri">The base fileUri</param>
        /// <param name="pattern">optional pattern to filter the dirs</param>
        IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern);

        /// <summary>
        /// Tries to get Files given a uri and pattern. fileUri MUST have a trailing '/'
        /// </summary>
        /// <param name="fileUri">The base fileUri</param>
        /// <param name="pattern">optional pattern to filter the resulting files</param>
        IEnumerable<Uri> GetFiles(Uri fileUri, string pattern); 

        /// <summary>
        /// Tries to parent URI
        /// </summary>
        /// <param name="fileUri">The base fileUri</param>
        Uri GetParentDirectory(Uri fileUri);

        /// <summary>
        /// Check whether specified URI exsists (depends on URI types). fileUri MUST have a trailing '/'
        /// </summary>
        /// <param name="fileUri">The fileUri to test</param>
        bool DirExists(Uri fileUri);

        /// <summary>
        /// Check whether specified URI exsists (depends on URI types)
        /// </summary>
        /// <param name="fileUri">The fileUri to test</param>
        bool FileExists(Uri fileUri);
    }
}