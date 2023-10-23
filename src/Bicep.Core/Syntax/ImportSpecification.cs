// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;

namespace Bicep.Core.Syntax
{
    public class ImportSpecification : ISymbolNameSource
    {
        // The setting below adds syntax highlighting for regex.
        // language=regex
        private const string NamePattern = "[a-zA-Z][a-zA-Z0-9]+";

        private const string BicepRegistryAddressPattern = @"br[:\/]\S+";

        // Regex copied from https://semver.org/.
        // language=regex
        private const string SemanticVersionPattern = @"(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?";

        private static readonly Regex BuiltInSpecificationPattern = new(
            @$"^(?<name>{NamePattern})@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private static readonly Regex FromRegistrySpecificationPattern = new(
            @$"^(?<address>{BicepRegistryAddressPattern})@(?<version>{SemanticVersionPattern})$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private static readonly Regex RepositoryNamePattern = new(
            @"^\S*[:\/](?<name>az)$",
            RegexOptions.ECMAScript | RegexOptions.Compiled);

        private ImportSpecification(string bicepRegistryAddress, string name, string version, bool isValid, TextSpan span)
        {
            BicepRegistryAddress = bicepRegistryAddress;
            Name = name;
            Version = version;
            IsValid = isValid;
            Span = span;
        }

        private string BicepRegistryAddress { get; }

        public string Name { get; }

        public string Version { get; }

        public bool IsValid { get; }

        public TextSpan Span { get; }

        public static ImportSpecification From(SyntaxBase specificationSyntax)
        {
            if (specificationSyntax is StringSyntax stringSyntax && stringSyntax.TryGetLiteralValue() is { } value)
            {
                return CreateFromStringSyntax(stringSyntax, value);
            }

            return new ImportSpecification(
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                LanguageConstants.ErrorName,
                false,
                specificationSyntax.Span);
        }

        public SyntaxBase ToPath()
        {
            if (!this.IsValid)
            {
                return new SkippedTriviaSyntax(this.Span, Enumerable.Empty<SyntaxBase>());
            }
            return SyntaxFactory.CreateStringLiteral(this.BicepRegistryAddress);
        }

        // copy from https://github.com/Azure/bicep/blob/main/src/Bicep.Core/Registry/OciArtifactRegistry.cs#L339-L378
        public static string GetArtifactDirectoryPath(ArtifactReference reference, string cacheRootDirectory)
        {
            if (reference is not OciArtifactReference providerReference)
            {
                throw new ArgumentException($"Reference type '{reference.GetType().Name}' is not supported.");
            }
            // cachePath is already set to %userprofile%\.bicep\br or ~/.bicep/br by default depending on OS
            // we need to split each component of the reference into a sub directory to fit within the max file name length limit on linux and mac

            // TODO: Need to deal with IDNs (internationalized domain names)
            // domain names can only contain alphanumeric chars, _, -, and numbers (example.azurecr.io or example.azurecr.io:443)
            // IPV4 addresses only contain dots and numbers (127.0.0.1 or 127.0.0.1:5000)
            // IPV6 addresses are hex digits with colons (2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF or [2001:db8:3333:4444:CCCC:DDDD:EEEE:FFFF]:5000)
            // the only problematic character is the colon, which we will replace with $ which is not allowed in any of the possible registry values
            // we will also normalize casing for registries since they are case-insensitive
            var registry = providerReference.Registry.Replace(':', '$').ToLowerInvariant();

            // modules can have mixed hierarchy depths so we will flatten a repository into a single directory name
            // however to do this we must get rid of slashes (not a valid file system char on windows and a directory separator on linux/mac)
            // the replacement char must one that is not valid in a repository string but is valid in common type systems
            var repository = providerReference.Repository.Replace('/', '$');

            string? tagOrDigest;
            if (providerReference.Tag is not null)
            {
                // tags are case-sensitive with length up to 128
                tagOrDigest = TagEncoder.Encode(providerReference.Tag);
            }
            else if (providerReference.Digest is not null)
            {
                // digests are strings like "sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4"
                // and are already guaranteed to be lowercase
                // the only problematic character is the : which we will replace with a #
                // however the encoding we choose must not be ambiguous with the tag encoding
                tagOrDigest = providerReference.Digest.Replace(':', '#');
            }
            else
            {
                throw new InvalidOperationException("Module reference is missing both tag and digest.");
            }

            //var packageDir = WebUtility.UrlEncode(reference.UnqualifiedReference);
            return Path.Combine(cacheRootDirectory, registry, repository, tagOrDigest);
        }

        private static ImportSpecification CreateFromStringSyntax(StringSyntax stringSyntax, string value)
        {
            if (BuiltInSpecificationPattern.Match(value) is { } builtInMatch && builtInMatch.Success)
            {
                var name = builtInMatch.Groups["name"].Value;
                var span = new TextSpan(stringSyntax.Span.Position + 1, name.Length);
                var version = builtInMatch.Groups["version"].Value;
                // built-in providers (e.g. kubernetes@1.0.0 or sys@1.0.0) are allowed as long as the name is not 'az'
                return new(name, name, version, name != AzNamespaceType.BuiltInName, span);
            }

            if (FromRegistrySpecificationPattern.Match(value) is { } registryMatch && registryMatch.Success)
            {
                // NOTE(asilverman): The regex for the registry pattern is intentionally loose since it will be validated by the module resolver.
                var address = registryMatch.Groups["address"].Value;
                var version = registryMatch.Groups["version"].Value;

                var span = new TextSpan(stringSyntax.Span.Position + 1, address.Length);
                // NOTE(asilverman): I normalize the artifact address to the way we represent module addresses, see https://github.com/Azure/bicep/issues/12202
                var unexpandedArtifactAddress = $"{address}:{version}";
                var name = RepositoryNamePattern.Match(address).Groups["name"].Value;
                // NOTE(asilverman): Only a repo name of az is allowed for now. This shall be relaxed once we generalize dynamic type loading for other provider packages.
                return new(unexpandedArtifactAddress, name, version, name == AzNamespaceType.BuiltInName, span);
            }
            return new(
                 LanguageConstants.ErrorName,
                 LanguageConstants.ErrorName,
                 LanguageConstants.ErrorName,
                 false,
                 stringSyntax.Span);
        }
    }
}
