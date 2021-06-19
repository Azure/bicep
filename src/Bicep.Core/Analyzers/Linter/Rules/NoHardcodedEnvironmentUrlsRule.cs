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

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedEnvironmentUrlsRule : LinterRuleBase
    {
        public new const string Code = "no-hardcoded-env-urls";

        private ImmutableHashSet<string>? DisallowedHosts;

        public NoHardcodedEnvironmentUrlsRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            docUri: new Uri("https://aka.ms/bicep/linter/no-hardcoded-env-urls"))
        {
        }

        public override void Configure(IConfigurationRoot config)
        {
            base.Configure(config);
            this.DisallowedHosts = GetArray(nameof(DisallowedHosts).ToLower(), Array.Empty<string>())
                                    .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found this disallowed host: \"{1}\"", this.Description, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (this.DisallowedHosts != null && this.DisallowedHosts.Any())
            {
                var visitor = new Visitor(this.DisallowedHosts);
                visitor.Visit(model.SyntaxTree.ProgramSyntax);
                return visitor.DisallowedHostSpans.Select(entry => CreateDiagnosticForSpan(entry.span, entry.host));
            }

            return Enumerable.Empty<IDiagnostic>();
        }

        private class Visitor : SyntaxVisitor
        {
            public readonly List<(string host, TextSpan span)> DisallowedHostSpans = new List<(string host, TextSpan span)>();

            private readonly ImmutableHashSet<string> disallowedHosts;

            public Visitor(ImmutableHashSet<string> disallowedHosts)
            {
                this.disallowedHosts = disallowedHosts;
            }

            public override void VisitStringSyntax(StringSyntax syntax)
            {
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
        }

    }
}
