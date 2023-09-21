// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using FluentAssertions.Primitives;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.IntegrationTests.Assertions
{
    public static class ShowMessageParamsExtensions
    {
        public static ShowMessageParamsAssertions Should(this ShowMessageParams instance)
        {
            return new ShowMessageParamsAssertions(instance);
        }
    }

    public class ShowMessageParamsAssertions : ReferenceTypeAssertions<ShowMessageParams, ShowMessageParamsAssertions>
    {
        public ShowMessageParamsAssertions(ShowMessageParams instance)
            : base(instance)
        {
        }

        protected override string Identifier => "show message event";

        public AndConstraint<ShowMessageParamsAssertions> HaveMessageAndType(string message, MessageType messageType, string because = "", params object[] becauseArgs)
        {
            Subject.Message.Should().Be(message, because, becauseArgs);
            Subject.Type.Should().Be(messageType, because, becauseArgs);

            return new(this);
        }
    }
}
