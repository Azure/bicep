// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    /// <summary>
    /// A ResourceIdentifier is a RFC3986 URI with absolute path.
    /// </summary>
    public readonly struct ResourceIdentifier : IEquatable<ResourceIdentifier>
    {
        public static class GlobalSettings
        {
            public static bool LocalFilePathCaseSensitive { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            public static StringComparer LocalFilePathComparer => LocalFilePathCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            public static StringComparison LocalFilePathComparison => LocalFilePathCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public ResourceIdentifier(ResourceIdentifierScheme scheme, string? authority, string path, string? query = null, string? fragment = null)
        {
            if (!path.StartsWith('/'))
            {
                throw new ArgumentException("Path must be absolute.", nameof(path));
            }

            this.Scheme = scheme;
            this.Authority = NormalizeAuthority(scheme, authority);
            this.Path = CanonicalizePath(path);
            this.Query = query;
            this.Fragment = fragment;
        }

        public ResourceIdentifierScheme Scheme { get; }

        public string? Authority { get; }

        public string Path { get; }

        public string? Query { get; }

        public string? Fragment { get; }

        public string[] PathSegments => this.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        public bool IsLocalFile => this.Scheme.IsFile && this.Authority == "";

        public StringComparison PathComparison => this.IsLocalFile ? GlobalSettings.LocalFilePathComparison : StringComparison.Ordinal;

        public StringComparer PathComparer => this.IsLocalFile ? GlobalSettings.LocalFilePathComparer : StringComparer.Ordinal;

        public static implicit operator string(ResourceIdentifier identifier) => identifier.ToString();

        public override string ToString() => this.TryGetLocalFilePath() ?? this.ToUriString();

        // See: The "file" URI Scheme (https://datatracker.ietf.org/doc/html/rfc8089).
        // Note that we don't handle user info and the case where the host IP resolves to the local machine.
        public string? TryGetLocalFilePath() => this.IsLocalFile ? new UriBuilder { Scheme = this.Scheme, Host = "", Path = Path }.Uri.LocalPath : null;

        // See: Uniform Resource Identifier (URI): Generic Syntax (https://datatracker.ietf.org/doc/html/rfc3986).
        public string ToUriString() => this.Authority is null ? $"{Scheme}:{Path}" : $"{Scheme}://{Authority}{Path}";

        public static bool operator ==(ResourceIdentifier left, ResourceIdentifier right) => left.Equals(right);

        public static bool operator !=(ResourceIdentifier left, ResourceIdentifier right) => !(left == right);

        public override int GetHashCode()
        {
            var hash = new HashCode();

            // Scheme and Authority are case-insenstive.
            hash.Add(this.Scheme);
            hash.Add(this.Authority, StringComparer.OrdinalIgnoreCase);
            hash.Add(this.Path, this.PathComparer);
            hash.Add(this.Query, StringComparer.Ordinal);
            hash.Add(this.Fragment, StringComparer.Ordinal);

            return hash.ToHashCode();
        }

        public override bool Equals(object? @object) => @object is ResourceIdentifier other && this.Equals(other);

        public bool Equals(ResourceIdentifier other) =>
            this.Scheme.Equals(other.Scheme) &&
            string.Equals(this.Authority, other.Authority, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(this.Query, other.Query, StringComparison.Ordinal) &&
            string.Equals(this.Fragment, other.Fragment, StringComparison.Ordinal) &&
            string.Equals(Path, other.Path, this.PathComparison);

        public string GetPathRelativeTo(ResourceIdentifier other)
        {
            if (!this.Scheme.Equals(other.Scheme) ||
                !string.Equals(this.Authority, other.Authority, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Both ResourceIdentifiers must have the same scheme and authority.");
            }

            var thisPathSegments = this.PathSegments;
            var otherPathSegments = other.PathSegments;
            var commonSegmentIndex = 0;

            while (
                commonSegmentIndex < thisPathSegments.Length &&
                commonSegmentIndex < otherPathSegments.Length &&
                thisPathSegments[commonSegmentIndex].Equals(otherPathSegments[commonSegmentIndex], this.PathComparison))
            {
                commonSegmentIndex++;
            }

            var relativePathSegments = new List<string>();

            for (int i = commonSegmentIndex; i < otherPathSegments.Length; i++)
            {
                relativePathSegments.Add("..");
            }

            for (int i = commonSegmentIndex; i < thisPathSegments.Length; i++)
            {
                relativePathSegments.Add(thisPathSegments[i]);
            }

            var relativePath = string.Join("/", relativePathSegments);

            return this.Path.EndsWith('/') ? relativePath + '/' : relativePath;
        }

        private static string? NormalizeAuthority(ResourceIdentifierScheme scheme, string? authority)
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

            return authority;
        }

        private static string CanonicalizePath(string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var stack = new Stack<string>();

            foreach (var segment in segments)
            {
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
                    stack.Push(segment);
                }
            }

            var canonicalPath = '/' + string.Join("/", stack.Reverse());

            return path.EndsWith('/') ? canonicalPath + "/" : canonicalPath;
        }
    }
}
