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
        private Regex hostRegex;
        private Regex schemaRegex;

        public NoHardcodedEnvironmentUrlsRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/no-hardcoded-env-urls"))
        {

            var schemaMatchStr = ;
            var schemaRegex = 
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
            new Regex(@"https://schema\.", RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.IgnoreCase);

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (this.DisallowedHosts != null && this.DisallowedHosts.Any())
            {
                // "lazy" load the regex but keep around as compiled
                this.hostRegex = this.hostRegex ?? CreateDisallowedHostRegex();
                this.schemaRegex = this.schemaRegex ?? CreateSchemaRegex();

                var visitor = new Visitor(this.DisallowedHosts);
                visitor.Visit(model.SyntaxTree.ProgramSyntax);
                return visitor.DisallowedHostSpans.Select(entry => CreateDiagnosticForSpan(entry.span, entry.host));
            }

            return Enumerable.Empty<IDiagnostic>();
        }

        private class Visitor : SyntaxVisitor
        {
            public readonly List<(string host, TextSpan span)> DisallowedHostSpans = new List<(string host, TextSpan span)>();
            private readonly Regex disallowedHostsRegex;
            private readonly Regex schemaRegex;

            public Visitor(Regex disallowedRegex, Regex schemaRegex)
            {
                this.disallowedHostsRegex = disallowedRegex;
                this.schemaRegex = schemaRegex;
            }

            public override void VisitStringSyntax(StringSyntax syntax)
            {
                **************************************
                //TODO: convert match spans into dignostics
                foreach (Match match in regexMatch.Matches(host))
                {
                    hostCt++;
                    //and see if it's preceeded by a schema.
                    var schemaMatch = schemaRegex.Match(host, match.Index);

                    // schema is found immediately preceeding this host match
                    bool schemaFound = schemaMatch.Success && (schemaMatch.Index + schemaMatch.Length) == match.Index;
                    if (schemaFound)
                    {
                        schemaCt++;
                    }
                }
                var disallowedHost = syntax.SegmentValues.Select(s => FindEnvironmentUrlInString(s))
                    .Where(span => span != null)
                    .FirstOrDefault();
                if (disallowedHost != null)
                {
                    this.DisallowedHostSpans.Add((disallowedHost, syntax.Span));
                }

                base.VisitStringSyntax(syntax);
            }

            private string? FindEnvironmentUrlInString(string str)
            {
                foreach (var host in this.disallowedHosts)
                {
                    var index = str.IndexOf(host, StringComparison.InvariantCultureIgnoreCase);
                    if (index >= 0)
                    {
                        return host;
                    }
                };

                return null;
            }
            ***********************************************
        }

    }
}
