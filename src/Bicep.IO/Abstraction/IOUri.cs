// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// We are only using the following System.IO.Path methods that do not access the
// file system for handling local file path in a cross-platform manner, which ensures
// that the IOUri is not coupled with any concret file system implementation:
// - System.IO.Path.IsPathFullyQualified
// - System.IO.Path.IsPathRooted
// - System.IO.Path.Join
using LocalFilePath = System.IO.Path;

namespace Bicep.IO.Abstraction
{
    /// <summary>
    /// A primitive URI implementation.
    /// </summary>
    /// <remarks>
    /// This implementation is intentionally limited to the subset of
    /// <see href="https://datatracker.ietf.org/doc/html/rfc3986">RFC3986</see> and
    /// <see href="https://datatracker.ietf.org/doc/html/rfc8089">RFC8089</see>
    /// to satisfy the functionality requirements of Bicep.
    /// </remarks>
    public class IOUri : IEquatable<IOUri>
    {
        public static class GlobalSettings
        {
            public static bool LocalFilePathCaseSensitive { get; set; } = OperatingSystem.IsLinux();

            public static StringComparer LocalFilePathComparer => LocalFilePathCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            public static StringComparison LocalFilePathComparison => LocalFilePathCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public IOUri(IOUriScheme scheme, string? authority, string path, string query = "", string fragment = "")
        {
            this.Scheme = scheme;
            this.Authority = NormalizeAuthority(scheme, authority);
            this.Path = NormalizePath(scheme, this.Authority, path);
            this.Query = query;
            this.Fragment = fragment;
        }

        public IOUriScheme Scheme { get; }

        public string? Authority { get; }

        public string Path { get; }

        public string Query { get; }

        public string Fragment { get; }

        public string[] PathSegments => this.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        public bool IsLocalFile => this.Scheme.IsFile && this.Authority == "";

        public StringComparison PathComparison => this.IsLocalFile ? GlobalSettings.LocalFilePathComparison : StringComparison.Ordinal;

        public StringComparer PathComparer => this.IsLocalFile ? GlobalSettings.LocalFilePathComparer : StringComparer.Ordinal;

        public static implicit operator string(IOUri uri) => uri.ToString();

        public static IOUri FromLocalFilePath(string filePath)
        {
            if (!LocalFilePath.IsPathFullyQualified(filePath) && !(filePath.StartsWith('/') || filePath.StartsWith('\\')))
            {
                // Technically speaking, /foo/bar is not a fully qualified file path on Windows. However, our tests use MockFileSystem,
                // which normalizes it to C:\foo\bar, so we allow it in this context. In non-test scenarios, file I/O with such paths
                // on Windows will fail during read/write operations.
                throw new IOException("File path must be absolute.");
            }

            if (OperatingSystem.IsWindows())
            {
                if (FilePathFacts.IsWindowsDosDevicePath(filePath))
                {
                    throw new IOException("Unsupported Windows DOS device path.");
                }
            }

            // System.Uri is RFC compliant when it comes to handle file URIs.
            // It handles all sorts of OS specific edge cases and normalizes path separators.
            filePath = filePath.Replace("%", "%25");
            var fileUri = new UriBuilder { Scheme = IOUriScheme.File, Host = "", Path = filePath }.Uri;

            if (fileUri.IsUnc)
            {
                throw new IOException("Unsupported UNC path.");
            }

            var uriPath = Uri.UnescapeDataString(fileUri.AbsolutePath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !uriPath.StartsWith('/'))
            {
                uriPath = "/" + uriPath;
            }

            return new IOUri(IOUriScheme.File, "", uriPath);
        }

        public override string ToString() => this.TryGetLocalFilePath() ?? this.ToUriString();

        // See: The "file" URI Scheme (https://datatracker.ietf.org/doc/html/rfc8089).
        // Note that we don't handle user info and the case where the host IP resolves to the local machine.
        public string? TryGetLocalFilePath() => this.IsLocalFile ? new UriBuilder { Scheme = this.Scheme, Host = "", Path = EscapePercentSign(this.Path) }.Uri.LocalPath : null;

        public string GetLocalFilePath() => TryGetLocalFilePath() ?? throw new InvalidOperationException("The URI is not a local file path.");

        // See: Uniform Resource Identifier (URI): Generic Syntax (https://datatracker.ietf.org/doc/html/rfc3986).
        public string ToUriString()
        {
            var escapedPath = EscapePercentSign(this.Path);

            return this.Authority is null ? $"{Scheme}:{escapedPath}" : $"{Scheme}://{Authority}{escapedPath}";
        }

        // TODO: Remove after file abstraction migration is complete.
        public Uri ToUri() => new UriBuilder { Scheme = this.Scheme, Host = "", Path = Path.Replace("%", "%25") }.Uri;

        public static bool operator ==(IOUri left, IOUri right) => left.Equals(right);

        public static bool operator !=(IOUri left, IOUri right) => !(left == right);

        public override int GetHashCode()
        {
            var hash = new HashCode();

            // Scheme and Authority are case-insensitive.
            hash.Add(this.Scheme);
            hash.Add(this.Authority, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.Path, this.PathComparer);
            hash.Add(this.Query, StringComparer.Ordinal);
            hash.Add(this.Fragment, StringComparer.Ordinal);

            return hash.ToHashCode();
        }

        public override bool Equals(object? @object) => @object is IOUri other && this.Equals(other);

        public bool Equals(IOUri? other) =>
            other is not null &&
            this.SchemeEquals(other) &&
            this.AuthorityEquals(other) &&
            this.PathEquals(other) &&
            this.QueryEquals(other) &&
            this.FragmentEquals(other);

        public bool IsBaseOf(IOUri other)
        {
            if (!this.SchemeEquals(other) || !this.AuthorityEquals(other))
            {
                return false;
            }

            var thisPathSegments = this.PathSegments;
            var otherPathSegments = other.PathSegments;

            if (thisPathSegments.Length > otherPathSegments.Length)
            {
                return false;
            }

            for (int i = 0; i < thisPathSegments.Length; i++)
            {
                if (!thisPathSegments[i].Equals(otherPathSegments[i], this.PathComparison))
                {
                    return false;
                }
            }

            return true;
        }

        public string GetPathRelativeTo(IOUri other)
        {
            if (this.Equals(other))
            {
                return "";
            }

            if (!this.Scheme.Equals(other.Scheme) || !string.Equals(this.Authority, other.Authority, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Both ResourceIdentifiers must have the same scheme and authority.");
            }

            var thisIsDirectory = this.Path.EndsWith('/');
            var otherIsDirectory = other.Path.EndsWith('/');

            var thisBaseSegments = thisIsDirectory ? this.PathSegments : this.PathSegments[..^1];
            var otherBaseSegments = otherIsDirectory ? other.PathSegments : other.PathSegments[..^1];

            // Find common prefix
            int commonSegmentIndex = 0;
            while (commonSegmentIndex < thisBaseSegments.Length &&
               commonSegmentIndex < otherBaseSegments.Length &&
               thisBaseSegments[commonSegmentIndex].Equals(otherBaseSegments[commonSegmentIndex], this.PathComparison))
            {
                commonSegmentIndex++;
            }

            // Calculate the number of directories to go up from 'other'
            int segmentsToGoUp = otherBaseSegments.Length - commonSegmentIndex;

            var relativePathSegments = new List<string>();

            for (int i = 0; i < segmentsToGoUp; i++)
            {
                relativePathSegments.Add("..");
            }

            // Add the remaining segments from 'this' path
            for (int i = commonSegmentIndex; i < this.PathSegments.Length; i++)
            {
                relativePathSegments.Add(this.PathSegments[i]);
            }

            var relativePath = string.Join("/", relativePathSegments);

            // Append '/' if 'this' is a directory
            if (this.Path.EndsWith('/') && !relativePath.EndsWith('/'))
            {
                relativePath += '/';
            }

            return relativePath;
        }

        public IOUri Resolve(string path)
        {
            if (!path.StartsWith('/'))
            {
                // Relative path.
                path = this.Path.EndsWith('/') ? this.Path + path : this.Path + "/../" + path;
            }

            return this.IsLocalFile ? FromLocalFilePath(path) : new IOUri(this.Scheme, this.Authority, path, this.Query, this.Fragment);
        }

        private static string EscapePercentSign(string value) => value.Replace("%", "%25");

        private static string? NormalizeAuthority(IOUriScheme scheme, string? authority)
        {
            if (scheme.IsHttp || scheme.IsHttps)
            {
                if (string.IsNullOrEmpty(authority))
                {
                    throw new ArgumentException("Authority must be non-empty for HTTP/HTTPS schemes.");
                }
            }

            if (scheme.IsFile)
            {
                if (string.IsNullOrEmpty(authority) ||
                    string.Equals(authority, "localhost", StringComparison.OrdinalIgnoreCase))
                {
                    return "";
                }
            }

            return authority?.ToLowerInvariant();
        }

        private static string NormalizePath(IOUriScheme scheme, string? authority, string path)
        {
            if (authority is not null && !(path.Length == 0 || path.StartsWith('/')))
            {
                // https://datatracker.ietf.org/doc/html/rfc3986#section-3.3
                throw new ArgumentException("Path must be empty or absolute when authority is non-null.");
            }

            if (authority is null && path.StartsWith("//"))
            {
                // https://datatracker.ietf.org/doc/html/rfc3986#section-3.3
                throw new ArgumentException("Path cannot start with '//' when authority is null.");
            }

            if (scheme.IsFile && !path.StartsWith('/'))
            {
                // https://datatracker.ietf.org/doc/html/rfc8089#section-2
                throw new ArgumentException("File path must be absolute.");
            }

            if (path.Length == 1 && path[0] == '/')
            {
                return path;
            }

            var isLocalFile = scheme.IsFile && authority == "";
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var stack = new Stack<string>();

            for (int i = 0; i < segments.Length; i++)
            {
                var segment = segments[i];

                if (segment == ".")
                {
                    continue;
                }

                if (segment == "..")
                {
                    if (stack.Count > 0)
                    {
                        stack.Pop();
                    }
                }
                else
                {
                    if (isLocalFile && OperatingSystem.IsWindows())
                    {
                        var fileName = segment;

                        if (i == segments.Length - 1 && fileName.LastIndexOf('.') is var extensionStartIndex && extensionStartIndex >= 0)
                        {
                            fileName = fileName[..extensionStartIndex];
                        }

                        if (FilePathFacts.IsWindowsReservedFileName(fileName))
                        {
                            throw new IOException("The specified path contains unsupported Windows reserved file name.");
                        }
                    }

                    stack.Push(segment);
                }
            }

            var normalizedPath = string.Join("/", stack.Reverse());

            if (path.StartsWith('/'))
            {
                normalizedPath = '/' + normalizedPath;
            }

            if (path.EndsWith('/'))
            {
                normalizedPath += '/';
            }

            return normalizedPath;
        }

        private bool SchemeEquals(IOUri other) => this.Scheme.Equals(other.Scheme);

        private bool AuthorityEquals(IOUri other) => string.Equals(this.Authority, other.Authority, StringComparison.OrdinalIgnoreCase);

        private bool QueryEquals(IOUri other) => string.Equals(this.Query, other.Query, StringComparison.Ordinal);

        private bool FragmentEquals(IOUri other) => string.Equals(this.Fragment, other.Fragment, StringComparison.Ordinal);

        private bool PathEquals(IOUri other) => string.Equals(this.Path, other.Path, this.PathComparison);
    }
}
