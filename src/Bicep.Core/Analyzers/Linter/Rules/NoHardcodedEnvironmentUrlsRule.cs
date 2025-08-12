// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class NoHardcodedEnvironmentUrlsRule : LinterRuleBase
    {
        public new const string Code = "no-hardcoded-env-urls";

        // Configuration keys for bicepconfig.json
        public readonly string DisallowedHostsKey = "disallowedHosts";
        public readonly string ExcludedHostsKey = "excludedHosts";

        public NoHardcodedEnvironmentUrlsRule() : base(
            code: Code,
            description: CoreResources.EnvironmentUrlHardcodedRuleDescription,
            LinterRuleCategory.BestPractice)
        {
        }

        public override string FormatMessage(params object[] values)
            => string.Format("{0} Found this disallowed host: \"{1}\"", this.Description, values.First());

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            if (model.SourceFile is BicepParamFile)
            {
                // The environment() function isn't available for .bicepparam files
                return [];
            }

            var disallowedHosts = GetConfigurationValue(model.Configuration.Analyzers, DisallowedHostsKey.ToLowerInvariant(), Array.Empty<string>()).ToImmutableArray();
            var excludedHosts = GetConfigurationValue(model.Configuration.Analyzers, ExcludedHostsKey.ToLowerInvariant(), Array.Empty<string>()).ToImmutableArray();

            if (disallowedHosts.Any())
            {
                var visitor = new Visitor(disallowedHosts, disallowedHosts.Min(h => h.Length), excludedHosts, model);
                visitor.Visit(model.SourceFile.ProgramSyntax);

                return visitor.DisallowedHostSpans.Select(entry => CreateDiagnosticForSpan(diagnosticLevel, entry.Key, entry.Value));
            }

            return [];
        }

        public static IEnumerable<(TextSpan RelativeSpan, string Value)> FindHostnameMatches(string hostname, string srcText)
        {
            // This code is executed VERY frequently, so we need to be ultra-careful making changes here.
            // For a compilation, runtime is ~O(number_of_modules * avg_number_of_viable_string_tokens * number_of_hostnames).
            // In the language service, we recompile on every keypress.

            static bool hasLeadingOrTrailingAlphaNumericChar(string srcText, string hostname, int index)
            {
                if (index > 0 && char.IsLetterOrDigit(srcText[index - 1]))
                {
                    return true;
                }

                if (index + hostname.Length < srcText.Length && char.IsLetterOrDigit(srcText[index + hostname.Length]))
                {
                    return true;
                }

                return false;
            }

            if (hostname.Length == 0)
            {
                // ensure we terminate - the below for-loop uses this value to increment
                yield break;
            }

            int matchIndex;
            for (var startIndex = 0; startIndex <= srcText.Length - hostname.Length; startIndex = matchIndex + hostname.Length)
            {
                matchIndex = srcText.IndexOf(hostname, startIndex, StringComparison.OrdinalIgnoreCase);
                if (matchIndex < 0)
                {
                    // we haven't foud any instances of the hostname
                    yield break;
                }

                // check preceding and trailing chars to verify we're not dealing with a substring
                if (!hasLeadingOrTrailingAlphaNumericChar(srcText, hostname, matchIndex))
                {
                    var matchText = srcText.Substring(matchIndex, hostname.Length);
                    yield return (RelativeSpan: new TextSpan(matchIndex, hostname.Length), Value: matchText);
                }
            }
        }

        private sealed class Visitor : AstVisitor
        {
            public readonly Dictionary<TextSpan, string> DisallowedHostSpans = new();
            private readonly ImmutableArray<string> disallowedHosts;
            private readonly int minHostLen;
            private readonly ImmutableArray<string> excludedHosts;
            private readonly SemanticModel model;

            public Visitor(ImmutableArray<string> disallowedHosts, int minHostLen, ImmutableArray<string> excludedHosts, SemanticModel model)
            {
                this.disallowedHosts = disallowedHosts;
                this.minHostLen = minHostLen;
                this.excludedHosts = excludedHosts;
                this.model = model;
            }

            public static IEnumerable<(TextSpan RelativeSpan, string Value)> RemoveOverlapping(IEnumerable<(TextSpan RelativeSpan, string Value)> matches)
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

            public override void VisitDecoratorSyntax(DecoratorSyntax syntax)
            {
                // Skip @description and @metadata decorators entirely
                if (model.GetSymbolInfo(syntax.Expression) is FunctionSymbol functionSymbol &&
                    functionSymbol.DeclaringObject is NamespaceType namespaceType &&
                    LanguageConstants.IdentifierComparer.Equals(namespaceType.ExtensionName, SystemNamespaceType.BuiltInName) &&
                    (LanguageConstants.IdentifierComparer.Equals(functionSymbol.Name, LanguageConstants.MetadataDescriptionPropertyName) ||
                     LanguageConstants.IdentifierComparer.Equals(functionSymbol.Name, LanguageConstants.ParameterMetadataPropertyName)))
                {
                    return;
                }

                base.VisitDecoratorSyntax(syntax);
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

                            foreach (var (RelativeSpan, Value) in RemoveOverlapping(disallowedMatches))
                            {
                                // exclusion is found containing the host match
                                var hasExclusion = exclusionMatches.Any(exclusionMatch =>
                                    TextSpan.AreOverlapping(exclusionMatch.RelativeSpan, RelativeSpan));

                                if (!hasExclusion)
                                {
                                    // create a span for the specific identified instance
                                    // to allow for multiple instances in a single syntax
                                    this.DisallowedHostSpans[new TextSpan(token.Span.Position + RelativeSpan.Position, RelativeSpan.Length)] = Value;
                                }
                            }
                        }
                    }
                }
                base.VisitStringSyntax(syntax);
            }

            public override void VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
            {
                // Skip resource 'type' strings - there's no reason to perform analysis on them
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Value);
            }

            public override void VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
            {
                // Skip resource 'path' strings - there's no reason to perform analysis on them
                this.VisitNodes(syntax.LeadingNodes);
                this.Visit(syntax.Value);
            }

            public override void VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax)
            {
                // Skip metadata declarations entirely - URLs in metadata should be allowed
                return;
            }
        }
    }
}
