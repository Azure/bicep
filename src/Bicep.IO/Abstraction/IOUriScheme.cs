// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.IO.Abstraction
{
    public readonly record struct IOUriScheme(string Name)
    {
        public static readonly IOUriScheme Http = new("http");

        public static readonly IOUriScheme Https = new("https");

        public static readonly IOUriScheme File = new("file");

        public readonly string Name { get; } = Name.ToLowerInvariant();

        public static implicit operator string(IOUriScheme scheme) => scheme.ToString();

        public static implicit operator IOUriScheme(string name) => new(name);

        public readonly bool IsHttp => this.Equals(Http);

        public readonly bool IsHttps => this.Equals(Https);

        public readonly bool IsFile => this.Equals(File);

        public override string ToString() => this.Name;
    }
}
