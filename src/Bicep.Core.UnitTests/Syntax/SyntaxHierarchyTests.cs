// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.UnitTests.Syntax
{
    [TestClass]
    public class SyntaxHierarchyTests
    {
        [TestMethod]
        public void EmptyFile_GetProgramParent_ShouldReturnNull()
        {
            var program = ParserHelper.Parse(string.Empty);
            var hierarchy = SyntaxHierarchy.Build(program);

            hierarchy.GetParent(program).Should().BeNull();
        }

        [TestMethod]
        public void NonEmptyFile_GetParent_ShouldReturnExpectedNode()
        {
            var program = ParserHelper.Parse("param foo string\r\nvar bar = 42");
            var hierarchy = SyntaxHierarchy.Build(program);

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

            var paramTypeSyntax = nodes.OfType<VariableAccessSyntax>().Single();
            hierarchy.GetParent(paramTypeSyntax).Should().BeSameAs(paramDecl);

            var paramTypeIdSyntax = nodes.OfType<IdentifierSyntax>().Single(id => string.Equals(id.IdentifierName, "string"));
            hierarchy.GetParent(paramTypeIdSyntax).Should().BeSameAs(paramTypeSyntax);

            var paramTypeToken = nodes.OfType<Token>().Single(t => t.Type == TokenType.Identifier && string.Equals(t.Text, "string"));
            hierarchy.GetParent(paramTypeToken).Should().BeSameAs(paramTypeIdSyntax);
        }
    }
}
