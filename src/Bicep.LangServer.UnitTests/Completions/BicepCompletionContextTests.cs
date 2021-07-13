// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Utils;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class BicepCompletionContextTests
    {
        [TestMethod]
        public void ZeroMatchingNodes_Create_ShouldThrow()
        {
            const string text = "var foo = 42";
            var compilation = new Compilation(AzResourceTypeProvider.CreateWithAzTypes(), SourceFileGroupingFactory.CreateFromText(text, BicepTestConstants.FileResolver));

            Action fail = () => BicepCompletionContext.Create(compilation, text.Length + 2);
            fail.Should().Throw<ArgumentException>().WithMessage("The specified offset 14 is outside the span of the specified ProgramSyntax node.");
        }
    }
}
