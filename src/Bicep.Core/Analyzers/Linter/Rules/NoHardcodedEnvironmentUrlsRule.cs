// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedEnvironmentUrlsRule : LinterRuleBase
    {
        public new const string Code = "no-hardcoded-env-urls";

        private ImmutableArray<string>? disallowedHosts;
        public IEnumerable<string> DisallowedHosts => disallowedHosts.HasValue ? disallowedHosts.Value : Array.Empty<string>();

        private ImmutableArray<string>? excludedHosts;
        public IEnumerable<string> ExcludedHosts => excludedHosts.HasValue ? excludedHosts.Value : Array.Empty<string>();

        private int MinimumHostLength;
        private bool HasHosts;

        public NoHardcodedEnvironmentUrlsRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/no-hardcoded-env-urls"))
        {
        }

        public override void Configure(IConfigurationRoot config)
        {
            base.Configure(config);

            this.disallowedHosts = GetArray(nameof(disallowedHosts), Array.Empty<string>())
                                    .ToImmutableArray();
            this.MinimumHostLength = this.disallowedHosts.Value.Min(h => h.Length);
            this.excludedHosts = GetArray(nameof(excludedHosts), Array.Empty<string>())
                                    .ToImmutableArray();

            this.HasHosts = this.disallowedHosts?.Any() ?? false;
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found this disallowed host: \"{1}\"", this.Description, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (HasHosts)
            {
                var visitor = new Visitor(this.disallowedHosts ?? ImmutableArray<string>.Empty, this.MinimumHostLength, this.excludedHosts ?? ImmutableArray<string>.Empty);
                visitor.Visit(model.SourceFile.ProgramSyntax);
                return visitor.DisallowedHostSpans.Select(entry => CreateDiagnosticForSpan(entry.Key, entry.Value));
            }
            return Enumerable.Empty<IDiagnostic>();
        }

        public class Visitor : SyntaxVisitor
        {
            public readonly Dictionary<TextSpan, string> DisallowedHostSpans = new Dictionary<TextSpan, string>();
            private readonly ImmutableArray<string> disallowedHosts;
            private readonly int minHostLen;
            private readonly ImmutableArray<string> excludedHosts;

            public Visitor(ImmutableArray<string> disallowedHosts, int minHostLen, ImmutableArray<string> excludedHosts)
            {
                this.disallowedHosts = disallowedHosts;
                this.minHostLen = minHostLen;
                this.excludedHosts = excludedHosts;
            }

            private enum matchTypes { exact, subdomain, substring }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="hostname"></param>
            /// <param name="srcText"></param>
            /// <param name="exactSubdomain">set to true for exclusions</param>
            /// <returns></returns>
            public static IEnumerable<(int Index, int Length, string Value)> FindMatches(string hostname, string srcText, bool exactSubdomain)
            {
                var matchIndex = -1;
                var startIndex = 0;

                do
                {
                    matchIndex = srcText.IndexOf(hostname, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (matchIndex > -1)
                    {
                        bool matchIsAtEnd = (matchIndex + hostname.Length) == srcText.Length;

                        //After finding a match we need to know if the end of the match is the logical end of a domain or just part of a string
                        // i.e. azuredatalake.net is should not match azuredatalake.net.com or azuredatalak.netware, etc.
                        // we can only know this by looking for the string to end where the match ends or find the match
                        // immediately followed by a valid separator

                        if (!matchIsLeadingSubstring())                    {
                            var matchType = getMatchType();

                            // return all exact matches
                            // substrings and subdomains are not a match
                            if (matchType == matchTypes.exact)
                            {
                                var matchText = srcText.Substring(matchIndex, hostname.Length);
                                yield return (Index: matchIndex, Length: hostname.Length, Value: matchText);
                            }
                        }
                    }
                    startIndex = matchIndex + hostname.Length;
                } while (matchIndex != -1);

                bool matchIsLeadingSubstring()
                {
                    if(matchIndex + hostname.Length == srcText.Length)
                    {
                        return false;
                    }
                    var trailingChar = srcText[matchIndex+hostname.Length]; // next char after match

                    // is a substring is trailing char is a char or digit
                    return Char.IsLetterOrDigit(trailingChar);
                }

                matchTypes getMatchType()
                {
                    // should call this method unless we know it's not 0, adding this safeguard.
                    if (matchIndex == 0)
                    {
                        return matchTypes.exact;
                    }

                    var prevChar = srcText[matchIndex - 1];

                    // assumption is that if the string is preceded by a . it is a subdomain
                    // otherwise if the preceding char is a letter or number it is a different subdomain
                    if (prevChar == '.')
                    {
                        return matchTypes.subdomain;
                    }

                    if (Char.IsLetterOrDigit(prevChar))
                    {
                        return matchTypes.substring;
                    }

                    // if not a subdomain and not just a substring
                    // then we have another delimiter that makes
                    // this an exact match
                    return matchTypes.exact;
                }
            }

            public override void VisitStringSyntax(StringSyntax syntax)
            {
                // shortcut check by testing length of full span
                if (syntax.Span.Length > minHostLen)
                {
                    foreach (var token in syntax.StringTokens)
                    {
                        // shortcut check by testing length of token
                        if (token.Text.Length >= minHostLen)
                        {
                            var disallowedMatches = disallowedHosts
                                .SelectMany(host => FindMatches(host, token.Text, false))
                                .ToImmutableArray();

                            if (disallowedMatches.Any())
                            {
                                var exclusionMatches = excludedHosts
                                    .SelectMany(host => FindMatches(host, token.Text, true))
                                    .ToImmutableArray();

                                // does this segment have a host match
                                foreach (var match in disallowedMatches)
                                {

                                    // exclusion is found containing the host match
                                    var isExcluded = exclusionMatches.Any(exclusionMatch =>
                                       match.Index > exclusionMatch.Index
                                       && match.Index + match.Length <= exclusionMatch.Index + exclusionMatch.Length);

                                    if (!isExcluded)
                                    {
                                        // create a span for the specific identified instance
                                        // to allow for multiple instances in a single syntax
                                        this.DisallowedHostSpans[new TextSpan(token.Span.Position + match.Index, match.Length)] = match.Value;
                                    }
                                }
                            }
                        }
                    }
                }
                base.VisitStringSyntax(syntax);
            }
        }
    }
}
