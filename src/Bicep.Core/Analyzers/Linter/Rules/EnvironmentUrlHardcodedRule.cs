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
    public sealed class EnvironmentUrlHardcodedRule : LinterRuleBase
    {
        public new const string Code = "Environment URL hardcoded";

        private ImmutableHashSet<string>? DisallowedHosts;

        public EnvironmentUrlHardcodedRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            diagnosticLevel: Diagnostics.DiagnosticLevel.Warning,
            docUri: "https://github.com/Azure/bicep/tree/main/docs/rules/")
        {
        }

        public override void Configure(IConfigurationRoot config)
        {
            base.Configure(config);
            this.DisallowedHosts = GetArray(nameof(DisallowedHosts), Array.Empty<string>())
                                    .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);
        }

        protected override string GetFormattedMessage(params object[] values)
            => string.Format("{0} -- Found: [{1}]", this.Description, values.First());

        override internal IEnumerable<IBicepAnalyzerDiagnostic> AnalyzeInternal(SemanticModel model)
        {
            if (this.DisallowedHosts != null && this.DisallowedHosts.Any())
            {
                var spansToMark = new Dictionary<TextSpan, List<string>>();
                var visitor = new BCPL1020Visitor(spansToMark, this.DisallowedHosts);
                visitor.Visit(model.SyntaxTree.ProgramSyntax);

                foreach (var kvp in spansToMark)
                {
                    var span = kvp.Key;
                    foreach (var hosts in kvp.Value)
                    {
                        yield return CreateDiagnosticForSpan(span, hosts);
                    }
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
