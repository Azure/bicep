// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    /// <summary>
    /// A ResourceIdentifier is a URI without the query and fragment components.
    /// </summary>
    public readonly struct ResourceIdentifier : IEquatable<ResourceIdentifier>
    {
        public static class Settings
        {
            public static bool PathCaseSensitive { get; set; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            public static StringComparer PathComparer => PathCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;

            public static StringComparison PathComparison => PathCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
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

        public string Authority { get; }

        public string Path { get; }

        public override string ToString() => $"{Scheme}://{Authority}{Path}";

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

            hash.Add(Scheme, StringComparer.Ordinal);
            hash.Add(Authority, StringComparer.Ordinal);
            hash.Add(Path, Settings.PathComparer);

            return hash.ToHashCode();
        }

        public override bool Equals(object? @object) => @object is ResourceIdentifier other && this.Equals(other);

        public bool Equals(ResourceIdentifier other) =>
            string.Equals(Scheme, other.Scheme, StringComparison.Ordinal) &&
            string.Equals(Authority, other.Authority, StringComparison.Ordinal) &&
            string.Equals(Path, other.Path, Settings.PathComparison);

        public static bool operator ==(ResourceIdentifier left, ResourceIdentifier right) => left.Equals(right);

        public static bool operator !=(ResourceIdentifier left, ResourceIdentifier right) => !(left == right);
    }
}
