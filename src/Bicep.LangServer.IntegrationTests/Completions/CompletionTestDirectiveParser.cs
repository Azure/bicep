// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Utils;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Completions
{
    public static class CompletionTestDirectiveParser
    {
        public static IList<CompletionTrigger> GetTriggers(string text)
        {
            var program = ParserHelper.Parse(text);
            var lineStarts = TextCoordinateConverter.GetLineStarts(text);
            var triggers = new List<CompletionTrigger>();
            
            var visitor = new CompletionTriggerCollector(triggers, lineStarts);
            visitor.Visit(program);

            return triggers;
        }

        private sealed class CompletionTriggerCollector : SyntaxVisitor
        {
            private static readonly Regex TriggerPattern = new Regex(@"#\s*completionTest\s*\(\s*(?<char>\d+)\s*(,\s*(?<char>\d+)\s*)*\)\s*->\s*(?<set>\w+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            private readonly IList<CompletionTrigger> triggers;
            private readonly ImmutableArray<int> lineStarts;

            public CompletionTriggerCollector(IList<CompletionTrigger> triggers, ImmutableArray<int> lineStarts)
            {
                this.triggers = triggers;
                this.lineStarts = lineStarts;
            }

            public override void VisitSyntaxTrivia(SyntaxTrivia syntaxTrivia)
            {
                if (syntaxTrivia.Type == SyntaxTriviaType.SingleLineComment || syntaxTrivia.Type == SyntaxTriviaType.MultiLineComment)
                {
                    var matches = TriggerPattern.Matches(syntaxTrivia.Text);
                    foreach (Match? match in matches)
                    {
                        this.ProcessMatch(syntaxTrivia, match!);
                    }
                }

                base.VisitSyntaxTrivia(syntaxTrivia);
            }

            private void ProcessMatch(SyntaxTrivia syntaxTrivia, Match match)
            {
                var setName = match.Groups["set"].Value;

                foreach (Capture? capture in match.Groups["char"].Captures)
                {
                    ProcessIndexCapture(syntaxTrivia, capture!, setName);
                }
            }

            private void ProcessIndexCapture(SyntaxTrivia syntaxTrivia, Capture capture, string setName)
            {
                var charStr = capture.Value;
                if (int.TryParse(charStr, out int charIndex) == false)
                {
                    throw new FormatException($"Comment '{syntaxTrivia.Text}' contains a completion test directive with an invalid character index '{charStr}'. Please specify a valid 32-bit integer.");
                }

                // this is the position of the end of the comment
                var commentEndPosition = PositionHelper.GetPosition(this.lineStarts, syntaxTrivia.GetEndPosition());

                // the trigger should apply to the char index at the next line
                var triggerPosition = new Position(commentEndPosition.Line + 1, charIndex);

                this.triggers.Add(new CompletionTrigger(triggerPosition, setName));
            }
        }
    }
}
