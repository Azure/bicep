// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedEnvironmentUrlsRule : LinterRuleBase
    {
        public new const string Code = "no-hardcoded-env-urls";

        public ImmutableArray<string>? DisallowedHosts;

        private ImmutableArray<string>? ExcludedHosts;
        private int MinimumHostLength;
        private bool HasHosts;

        private readonly Lazy<Regex> hostRegexLazy;
        private Regex hostRegex => hostRegexLazy.Value;

        private readonly Lazy<Regex> excludedRegexLazy;
        private Regex excludedRegex => excludedRegexLazy.Value;

        public NoHardcodedEnvironmentUrlsRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/no-hardcoded-env-urls"))
        {
            this.hostRegexLazy = new Lazy<Regex>(CreateDisallowedHostRegex);
            this.excludedRegexLazy = new Lazy<Regex>(CreateExcludedHostsRegex);
        }

        public override void Configure(IConfigurationRoot config)
        {
            base.Configure(config);

            this.DisallowedHosts = GetArray(nameof(DisallowedHosts), Array.Empty<string>())
                                    .ToImmutableArray();
            this.MinimumHostLength = this.DisallowedHosts.Value.Min(h => h.Length);
            this.ExcludedHosts = GetArray(nameof(ExcludedHosts), Array.Empty<string>())
                                    .ToImmutableArray();

            this.HasHosts = this.DisallowedHosts?.Any() ?? false;
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found this disallowed host: \"{1}\"", this.Description, values.First());

        /// <summary>
        /// Note that this regex requires the match to be at a subdomain boundary
        /// so that there is not a partial match "mid-word". The subdomain must
        /// be proceeded by a text break (., or slash, or space) or at the beginning
        /// of a line.
        /// </summary>
        /// <returns></returns>
        public Regex CreateDisallowedHostRegex() =>
            new Regex(string.Join('|', this.DisallowedHosts!.Value.Select(h => $@"(?<=\.|'|\s|^|/){Regex.Escape(h)}")),
                        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Regex CreateExcludedHostsRegex() =>
            new Regex(string.Join('|', this.ExcludedHosts!.Value.Select(h => $@"(?<=\.|'|\s|^|/){Regex.Escape(h)}")),
                        RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (HasHosts)
            {
                var visitor = new Visitor(this.hostRegex, this.MinimumHostLength, this.excludedRegex);
                visitor.Visit(model.SourceFile.ProgramSyntax);
                return visitor.DisallowedHostSpans.Select(entry => CreateDiagnosticForSpan(entry.Key, entry.Value));
            }
            return Enumerable.Empty<IDiagnostic>();
        }

        private class Visitor : SyntaxVisitor
        {
            public readonly Dictionary<TextSpan, string> DisallowedHostSpans = new Dictionary<TextSpan, string>();
            private readonly Regex hostRegex;
            private readonly int minHostLen;
            private readonly Regex exclusionRegex;

            public Visitor(Regex disallowedRegex, int minHostLen, Regex exclusionRegex)
            {
                this.hostRegex = disallowedRegex;
                this.minHostLen = minHostLen;
                this.exclusionRegex = exclusionRegex;
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
                            var disallowedMatches = this.hostRegex.Matches(token.Text);

                            if (disallowedMatches.Any())
                            {
                                var exclusionMatches = exclusionRegex.Matches(token.Text);

                                // does this segment have a host match
                                foreach (Match match in disallowedMatches)
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
