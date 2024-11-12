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
    /// A ResourceIdentifier is a RFC3986 URI with absolute path and without the query and fragment components.
    /// </summary>
    public readonly struct ResourceIdentifier : IEquatable<ResourceIdentifier>
    {
        public static class GlobalSettings
        {
            public static bool FilePathCaseSensitive { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            public static StringComparer FilePathComparer => FilePathCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            public static StringComparison FilePathComparison => FilePathCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        }

        public ResourceIdentifier(string scheme, string authority, string path)
        {
            if (!path.StartsWith('/'))
            {
                throw new ArgumentException("Path must be absolute.", nameof(path));
            }

            this.Scheme = scheme;
            this.Authority = authority;
            this.Path = CanonicalizePath(path);
        }

        public string Scheme { get; }

        public string? Authority { get; }

        public string Path { get; }

        public bool IsFile => this.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase);

        public bool IsLocal => string.IsNullOrEmpty(this.Authority) || this.Authority.Equals("localhost", StringComparison.OrdinalIgnoreCase);

        public static implicit operator string(ResourceIdentifier identifier) => identifier.ToString();

        public override string ToString() => this.TryGetLocalFilePath() ?? this.ToUriString();

        // See: The "file" URI Scheme (https://datatracker.ietf.org/doc/html/rfc8089).
        // Note that we don't handle the case where the host IP resolves to the local machine.
        public string? TryGetLocalFilePath() => this.IsFile && this.IsLocal ? new UriBuilder { Scheme = Scheme, Host = "", Path = Path }.Uri.LocalPath : null;

        // See: Uniform Resource Identifier (URI): Generic Syntax (https://datatracker.ietf.org/doc/html/rfc3986).
        public string ToUriString() => this.Authority is null ? $"{Scheme}:{Path}" : $"{Scheme}://{Authority}{Path}";

        private static string CanonicalizePath(string path)
        {
            var hasTrailingSlash = path.EndsWith('/');
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

            var canonicalPath = string.Join("/", stack.Reverse());

            return hasTrailingSlash
                ? "/" + canonicalPath + "/"
                : "/" + canonicalPath;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            // Scheme and Authority are case-insenstive.
            hash.Add(Scheme, StringComparer.OrdinalIgnoreCase);
            hash.Add(Authority, StringComparer.OrdinalIgnoreCase);

            if (this.IsLocal && this.IsFile)
            {
                hash.Add(Path, GlobalSettings.FilePathComparer);
            }
            else
            {
                hash.Add(Path, StringComparer.Ordinal);
            }

            return hash.ToHashCode();
        }

        public override bool Equals(object? @object) => @object is ResourceIdentifier other && this.Equals(other);

        public bool Equals(ResourceIdentifier other) =>
            string.Equals(Scheme, other.Scheme, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(Authority, other.Authority, StringComparison.OrdinalIgnoreCase) &&
            this.IsLocal && this.IsFile
                ? string.Equals(Path, other.Path, GlobalSettings.FilePathComparison)
                : string.Equals(Path, other.Path, StringComparison.Ordinal);

        public static bool operator ==(ResourceIdentifier left, ResourceIdentifier right) => left.Equals(right);

        public static bool operator !=(ResourceIdentifier left, ResourceIdentifier right) => !(left == right);
    }
}
