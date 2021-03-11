// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Syntax
{
    [TestClass]
    public class SyntaxHierarchyTests
    {
        [TestMethod]
        public void EmptyHierarchy_GetParent_ShouldThrow()
        {
            var hierarchy = new SyntaxHierarchy();
            Action fail = () => hierarchy.GetParent(TestSyntaxFactory.CreateNull());
            fail.Should().Throw<ArgumentException>().WithMessage("Unable to determine parent of specified node of type 'NullLiteralSyntax' at span '[0:0]' because it has not been indexed.");
        }

        [TestMethod]
        public void EmptyFile_GetProgramParent_ShouldReturnNull()
        {
            var hierarchy = new SyntaxHierarchy();
            var program = ParserHelper.Parse(string.Empty);
            hierarchy.AddRoot(program);

            hierarchy.GetParent(program).Should().BeNull();
        }

        [TestMethod]
        public void NonEmptyFile_GetParent_ShouldReturnExpectedNode()
        {
            var hierarchy = new SyntaxHierarchy();
            var program = ParserHelper.Parse("param foo string\r\nvar bar = 42");

            hierarchy.AddRoot(program);
            hierarchy.GetParent(program).Should().BeNull();

            var nodes = SyntaxAggregator.Aggregate(program, new List<SyntaxBase>(),
                (accumulated, current) =>
                {
                    accumulated.Add(current);
                    return accumulated;
                },
                accumulated => accumulated);

            var paramDecl = nodes.OfType<ParameterDeclarationSyntax>().Single();
            hierarchy.GetParent(paramDecl).Should().BeSameAs(program);

            var varDecl = nodes.OfType<VariableDeclarationSyntax>().Single();
            hierarchy.GetParent(varDecl).Should().BeSameAs(program);

            var newLine = nodes.OfType<Token>().Single(t => t.Type == TokenType.NewLine);
            hierarchy.GetParent(newLine).Should().BeSameAs(program);

            var paramIdSyntax = nodes.OfType<IdentifierSyntax>().Single(id => string.Equals(id.IdentifierName, "foo"));
            hierarchy.GetParent(paramIdSyntax).Should().BeSameAs(paramDecl);

            var varIdSyntax = nodes.OfType<IdentifierSyntax>().Single(id => string.Equals(id.IdentifierName, "bar"));
            hierarchy.GetParent(varIdSyntax).Should().BeSameAs(varDecl);

            var paramTypeSyntax = nodes.OfType<TypeSyntax>().Single();
            hierarchy.GetParent(paramTypeSyntax).Should().BeSameAs(paramDecl);

            var paramTypeToken = nodes.OfType<Token>().Single(t => t.Type == TokenType.Identifier && string.Equals(t.Text, "string"));
            hierarchy.GetParent(paramTypeToken).Should().BeSameAs(paramTypeSyntax);
        }
    }
}
