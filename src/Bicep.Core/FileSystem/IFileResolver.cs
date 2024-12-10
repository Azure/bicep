// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.FileSystem;

public record FileWithEncoding(
    string Contents,
    Encoding Encoding);

public interface IFileResolver
{
    /// <summary>
    /// Tries to read a file contents to string. If an exception is encountered, returns null and sets a non-null failureMessage.
    /// </summary>
    /// <param name="fileUri">The file URI to read.</param>
    ResultWithDiagnosticBuilder<string> TryRead(Uri fileUri);

    /// <summary>
    /// Reads a file contents up to a certain number of characters.
    /// </summary>
    ResultWithDiagnosticBuilder<string> TryReadAtMostNCharacters(Uri fileUri, Encoding fileEncoding, int n);

    /// <summary>
    /// Tries to read a file as binary data.
    /// </summary>
    ResultWithDiagnosticBuilder<BinaryData> TryReadAsBinaryData(Uri fileUri, int? maxFileSize = null);

    void Write(Uri fileUri, Stream contents);

    /// <summary>
    /// Tries to resolve a child file path relative to a parent module file path.
    /// </summary>
    /// <param name="parentFileUri">The file URI of the parent.</param>
    /// <param name="childFilePath">The file path of the child.</param>
    Uri? TryResolveFilePath(Uri parentFileUri, string childFilePath);

    /// <summary>
    /// Tries to get Directories given a uri and pattern. Both argument and returned URIs MUST have a trailing '/'
    /// </summary>
    /// <param name="fileUri">The base fileUri</param>
    /// <param name="pattern">optional pattern to filter the dirs</param>
    IEnumerable<Uri> GetDirectories(Uri fileUri, string pattern = "");

    /// <summary>
    /// Tries to get Files given a uri and pattern. fileUri MUST have a trailing '/'
    /// </summary>
    /// <param name="fileUri">The base fileUri</param>
    /// <param name="pattern">optional pattern to filter the resulting files</param>
    IEnumerable<Uri> GetFiles(Uri fileUri, string pattern = "");

    /// <summary>
    /// Check whether specified URI's directory exists if specified URI is a file:// URI. fileUri MUST have a trailing '/'
    /// </summary>
    /// <param name="fileUri">The fileUri to test</param>
    bool DirExists(Uri fileUri);

    /// <summary>
    /// Checks if the specified file URI exists.
    /// </summary>
    /// <param name="uri">The URI to test.</param>
    bool FileExists(Uri uri);
}
