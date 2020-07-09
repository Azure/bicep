using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReference
    {
        public ResourceTypeReference(string @namespace, IEnumerable<string> types, string apiVersion)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentException("Namespace must not be null, empty or whitespace.");
            }

            if (string.IsNullOrWhiteSpace(apiVersion))
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
    }
}