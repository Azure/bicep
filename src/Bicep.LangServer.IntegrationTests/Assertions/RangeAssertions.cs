// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class RangeExtensions 
    {
        public static RangeAssertions Should(this Range instance)
        {
            return new RangeAssertions(instance); 
        }
    }

    public class RangeAssertions : ReferenceTypeAssertions<Range, RangeAssertions>
    {
        public RangeAssertions(Range instance)
            : base(instance)
        {
        }

        protected override string Identifier => "range";

        public AndConstraint<RangeAssertions> HaveRange((int line, int character) start, (int line, int character) end, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => Subject.Start)
                .ForCondition(p => p.Line == start.line && p.Character == start.character)
                .FailWith("Expected start to be at [{0}:{1}] but found [{2}:{3}]", _ => start.line, _ => start.character, p => p.Line, p => p.Character)
                .Then
                .Given<Position>(_ => Subject.End)
                .ForCondition(p => p.Line == end.line && p.Character == end.character)
                .FailWith("Expected end to be at [{0}:{1}] but found [{2}:{3}]", _ => end.line, _ => end.character, p => p.Line, p => p.Character);

            return new AndConstraint<RangeAssertions>(this);
        }
    }
}
