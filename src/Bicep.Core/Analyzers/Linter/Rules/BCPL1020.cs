// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Analyzers.Interfaces;
using Bicep.Core.Configuration;
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
    public sealed class BCPL1020 : LinterRuleBase
    {
        private readonly ImmutableHashSet<string> DisallowedHosts;

        public BCPL1020() : base(
            code: "BCPL1020",
            ruleName: "Environment URL hardcoded",
            description: "Environment URLs should not be hardcoded. Access URLs via the environment() function to keep references current.",
            diagnosticLevel: Diagnostics.DiagnosticLevel.Warning,
            docUri: "https://bicep/linter/rules/BCPL1020")// TODO: setup up doc pages
        {
            // create a hashset lookup for hosts
            this.DisallowedHosts = GetConfiguration(nameof(this.DisallowedHosts), Array.Empty<string>())
                                    .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        }

        protected override string GetFormattedMessage(params object[] values)
            => string.Format("{0} -- Found: [{1}]", this.Description, values.First());

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            var spansToMark = new Dictionary<TextSpan, List<string>>();
            var visitor = new BCPL1020Visitor(spansToMark, this.DisallowedHosts);
            visitor.Visit(model.SyntaxTree.ProgramSyntax);

            foreach(var kvp in spansToMark)
            {
                var span = kvp.Key;
                foreach(var hosts in kvp.Value)
                {
                    yield return CreateDiagnosticForSpan(span, hosts);
                }
            }
        }

        private class BCPL1020Visitor : SyntaxVisitor
        {
            private readonly Dictionary<TextSpan, List<string>> hostsFound;
            private readonly ImmutableHashSet<string> disallowedHosts;

            public BCPL1020Visitor(Dictionary<TextSpan, List<string>> hostsFound, ImmutableHashSet<string> disallowedHosts)
            {
                this.hostsFound = hostsFound;
                this.disallowedHosts = disallowedHosts;
            }

            public override void VisitStringSyntax(StringSyntax syntax)
            {
                var disallowed = syntax.SegmentValues.Where(s => this.disallowedHosts.Contains(s));
                if (disallowed.Any())
                {
                    this.hostsFound[syntax.Span] = disallowed.ToList();
                }
                base.VisitStringSyntax(syntax);
            }
        }

    }
}
