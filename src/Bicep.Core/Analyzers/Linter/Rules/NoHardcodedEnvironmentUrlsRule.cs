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

            public static IEnumerable<(TextSpan RelativeSpan, string Value)> FindHostnameMatches(string hostname, string srcText)
            {
                bool isExactDomainMatch(int index)
                {
                    var leadingAlnum = index > 0 && 
                        char.IsLetterOrDigit(srcText[index - 1]);
                    var trailingAlnum = index + hostname.Length < srcText.Length && 
                        char.IsLetterOrDigit(srcText[index + hostname.Length]);

                    return !leadingAlnum && !trailingAlnum;
                }

                if (hostname.Length == 0)
                {
                    // ensure we terminate - the below for-loop uses this value to increment
                    yield break;
                }

                var matchIndex = -1;
                for (var startIndex = 0; startIndex <= srcText.Length - hostname.Length; startIndex = matchIndex + hostname.Length)
                {
                    matchIndex = srcText.IndexOf(hostname, startIndex, StringComparison.OrdinalIgnoreCase);
                    if (matchIndex < 0)
                    {
                        // we haven't foud any instances of the hostname
                        yield break;
                    }

                    // check preceeding and trailing chars to verify we're not dealing with a substring
                    if (isExactDomainMatch(matchIndex))
                    {
                        var matchText = srcText.Substring(matchIndex, hostname.Length);
                        yield return (RelativeSpan: new TextSpan(matchIndex, hostname.Length), Value: matchText);
                    }
                }
            }

            public IEnumerable<(TextSpan RelativeSpan, string Value)> RemoveOverlapping(IEnumerable<(TextSpan RelativeSpan, string Value)> matches)
            {
                TextSpan? prevSpan = null;
                foreach (var match in matches.OrderBy(x => x.RelativeSpan.Position).ThenByDescending(x => x.RelativeSpan.Length))
                {
                    if (prevSpan is not null && TextSpan.AreOverlapping(match.RelativeSpan, prevSpan))
                    {
                        continue;
                    }

                    yield return match;

                    prevSpan = match.RelativeSpan;
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
                                .SelectMany(host => FindHostnameMatches(host, token.Text))
                                .ToImmutableArray();

                            if (!disallowedMatches.Any())
                            {
                                break;
                            }

                            var exclusionMatches = excludedHosts
                                .SelectMany(host => FindHostnameMatches(host, token.Text))
                                .ToImmutableArray();

                            foreach (var match in RemoveOverlapping(disallowedMatches))
                            {
                                // exclusion is found containing the host match
                                var hasExclusion = exclusionMatches.Any(exclusionMatch =>
                                    TextSpan.AreOverlapping(exclusionMatch.RelativeSpan, match.RelativeSpan));

                                if (!hasExclusion)
                                {
                                    // create a span for the specific identified instance
                                    // to allow for multiple instances in a single syntax
                                    this.DisallowedHostSpans[new TextSpan(token.Span.Position + match.RelativeSpan.Position, match.RelativeSpan.Length)] = match.Value;
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
