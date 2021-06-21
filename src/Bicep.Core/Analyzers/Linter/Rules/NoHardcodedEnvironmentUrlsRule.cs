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

        private ImmutableHashSet<string>? DisallowedHosts;
        private readonly Lazy<Regex> hostRegexLazy;
        private Regex hostRegex => hostRegexLazy.Value;

        private readonly Lazy<Regex> schemaRegexLazy;
        private Regex schemaRegex => schemaRegexLazy.Value;

        public NoHardcodedEnvironmentUrlsRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/no-hardcoded-env-urls"))
        {
            this.hostRegexLazy = new Lazy<Regex>(CreateDisallowedHostRegex);
            this.schemaRegexLazy = new Lazy<Regex>(CreateSchemaRegex);
        }

        public override void Configure(IConfigurationRoot config)
        {
            base.Configure(config);
            this.DisallowedHosts = GetArray(nameof(DisallowedHosts).ToLower(), Array.Empty<string>())
                                    .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found this disallowed host: \"{1}\"", this.Description, values.First());

        private Regex CreateDisallowedHostRegex() =>
            new Regex(string.Join('|', this.DisallowedHosts), RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Regex CreateSchemaRegex() =>
            new Regex(@"https://schema\.|http://schema\.", RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (this.DisallowedHosts != null && this.DisallowedHosts.Any())
            {
                var visitor = new Visitor(this.hostRegex, this.schemaRegex);
                visitor.Visit(model.SyntaxTree.ProgramSyntax);
                return visitor.DisallowedHostSpans.Select(entry => CreateDiagnosticForSpan(entry.Key, entry.Value));
            }

            return Enumerable.Empty<IDiagnostic>();
        }

        private class Visitor : SyntaxVisitor
        {
            public readonly Dictionary<TextSpan, string> DisallowedHostSpans = new Dictionary<TextSpan, string>();
            private readonly Regex hostRegex;
            private readonly Regex schemaRegex;

            public Visitor(Regex disallowedRegex, Regex schemaRegex)
            {
                this.hostRegex = disallowedRegex;
                this.schemaRegex = schemaRegex;
            }

            public override void VisitStringSyntax(StringSyntax syntax)
            {
                foreach (var segment in syntax.SegmentValues)
                {
                    // does this segment have a host match
                    foreach (Match match in this.hostRegex.Matches(segment))
                    {
                        //and see if the match is preceeded by a schema.
                        var schemaMatch = this.schemaRegex.Match(segment, match.Index);

                        // schema is found immediately preceeding this host match
                        bool schemaFound = schemaMatch.Success && (schemaMatch.Index + schemaMatch.Length) == match.Index;
                        if (!schemaFound)
                        {
                            this.DisallowedHostSpans[syntax.Span] = match.Value;
                        }
                    }
                    base.VisitStringSyntax(syntax);
                }
            }
        }
    }
}
