// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class PositionExtensions 
    {
        public static PositionAssertions Should(this Position instance)
        {
            return new PositionAssertions(instance); 
        }
    }

    public class PositionAssertions : ReferenceTypeAssertions<Position, PositionAssertions>
    {
        public PositionAssertions(Position instance)
            : base(instance)
        {
        }

        protected override string Identifier => "position";

        public AndConstraint<PositionAssertions> HavePosition(int line, int character, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .BecauseOf(because, becauseArgs)
                .Given(() => Subject)
                .ForCondition(p => p.Line == line && p.Character == character)
                .FailWith("Expected position line {0} char {1}, but found line {2}, char {3}", _ => line, _ => character, p => p.Line, p => p.Character);

            return new AndConstraint<PositionAssertions>(this);
        }        
    }
}
