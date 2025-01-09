// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

//using Bicep.Core.Diagnostics;
//using Bicep.Core.Semantics;
//using FluentAssertions;
//using FluentAssertions.Execution;
//using FluentAssertions.Primitives;
//using Moq;
//using OmniSharp.Extensions.LanguageServer.Protocol.Models;

//namespace Bicep.Core.UnitTests.Assertions
//{
//    public static class CompletionItemsExtensions
//    {
//        public static CompletionItemsAssertions Should(this CompletionList completionList)
//        {
//            return new CompletionItemsAssertions(completionList);
//        }
//    }

//    public class CompletionItemsAssertions : ReferenceTypeAssertions<CompletionList, CompletionItemsAssertions>
//    {
//        public CompletionItemsAssertions(CompletionList completionList)
//            : base(completionList)
//        {
//        }

//        protected override string Identifier => "CompletionList";

//        public AndConstraint<CompletionItem> ContainByLabel(string label, string because = "", params object[] becauseArgs)
//        {
//            var matchingItem = Subject.Items.FirstOrDefault(item => item.Label == label);

//            Execute.Assertion
//                .BecauseOf(because, becauseArgs)
//                .ForCondition(matchingItem != null)
//                .FailWith("Expected to find a CompletionItem with label {0}{reason}, but did not.", label);

//            return new AndConstraint<CompletionItem>(matchingItem!);
//        }
//    }
//}
