// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bicep.Core.Semantics
{
    public class ImportSpecification
    {
        private const RegexOptions PatternRegexOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant;

        private static readonly Regex SpecificationPattern = new(@"^(?<name>[a-z][a-z0-9]+)@(?<version>[a-z-0-9][a-z0-9.]*)$", PatternRegexOptions);

        private ImportSpecification(string name, string version)
        {
            this.Name = name;
            this.Version = version;
        }

        public string Name { get; }

        public string Version { get; }

        public static ImportSpecification? TryParse(string specificationValue)
        {
            var match = SpecificationPattern.Match(specificationValue);

            if (!match.Success)
            {
                return null;
            }

            var name = match.Groups["name"].Value;
            var version = match.Groups["version"].Value;

            return new(name, version);
        }
    }
}
