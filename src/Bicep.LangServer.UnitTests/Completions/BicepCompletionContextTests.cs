﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using Bicep.Core.Syntax;
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
            var syntaxTree = SyntaxTree.Create(new Uri("test://test"), text);

            Action fail = () => BicepCompletionContext.Create(syntaxTree, text.Length + 2);
            fail.Should().Throw<ArgumentException>().WithMessage("The specified offset 14 is outside the span of the specified ProgramSyntax node.");
        }
    }
}
