using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReference
    {
        private static readonly Regex ResourceTypePattern = new Regex(@"^(?<namespace>[a-z0-9][a-z0-9\.]*)(/(?<type>[a-z0-9]+))+@(?<version>(\d{4}-\d{2}-\d{2})(-(preview|alpha|beta|rc|privatepreview))?$)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public ResourceTypeReference(string @namespace, IEnumerable<string> types, string apiVersion)
        {
            if (String.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentException("Namespace must not be null, empty or whitespace.");
            }

            if (String.IsNullOrWhiteSpace(apiVersion))
            {
                throw new ArgumentException("API Version must not be null, empty or whitespace.");
            }

            this.Namespace = @namespace;
            this.Types = types.ToImmutableArray();
            if (this.Types.Length <= 0)
            {
                throw new ArgumentException("At least one type must be specified.");
            }

            this.ApiVersion = apiVersion;
        }

        public string Namespace { get; }

        public ImmutableArray<string> Types { get; }

        public string ApiVersion { get; }

        public string FullyQualifiedType => $"{this.Namespace}/{this.Types.ConcatString("/")}";

        public static ResourceTypeReference? TryParse(string? resourceType)
        {
            if (resourceType == null)
            {
                return null;
            }

            var match = ResourceTypePattern.Match(resourceType);
            if (match.Success == false)
            {
                return null;
            }

            var ns = match.Groups["namespace"].Value;
            var types = match.Groups["type"].Captures.Cast<Capture>().Select(c => c.Value);
            var version = match.Groups["version"].Value;

            return new ResourceTypeReference(ns, types, version);
        }
    }
}