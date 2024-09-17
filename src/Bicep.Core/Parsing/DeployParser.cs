// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Syntax;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Parsing
{
    public class DeployParser : BaseParser
    {
        public DeployParser(string text)
            : base(text)
        {
        }

        public override ProgramSyntax Program()
        {
            var children = new List<SyntaxBase>();

            while (!this.IsAtEnd())
            {
                // this produces either a declaration node, skipped tokens node or just a token
                var child = Declaration();
                children.Add(child);

                // if skipped node is returned above, the newline is not consumed
                // if newline token is returned, we must not expect another (could be a beginning of a declaration)
                if (child is StatementSyntax)
                {
                    // declarations must be followed by a newline or the file must end
                    var newLine = this.WithRecoveryNullable(this.NewLineOrEof, RecoveryFlags.ConsumeTerminator, TokenType.NewLine);
                    if (newLine != null)
                    {
                        children.Add(newLine);
                    }
                }
            }

            var endOfFile = reader.Read();
            var programSyntax = new ProgramSyntax(children, endOfFile);

            var parsingErrorVisitor = new ParseDiagnosticsVisitor(this.ParsingErrorTree);
            parsingErrorVisitor.Visit(programSyntax);

            return programSyntax;
        }

        protected override SyntaxBase Declaration(params string[] expectedKeywords) =>
            this.WithRecovery<SyntaxBase>(
                () =>
                {
                    var current = reader.Peek();

                    return current.Type switch
                    {
                        TokenType.NewLine => this.NewLine(),
                        TokenType.Identifier when current.Text is LanguageConstants.DeployKeyword => this.DeployDeclaration(),
                        _ => throw new ExpectedTokenException(current, b => b.UnrecognizedDeployFileDeclaration()),
                    };
                },
                RecoveryFlags.None,
                TokenType.NewLine);

        private DeployDeclarationSyntax DeployDeclaration()
        {
            var keyword = this.ExpectKeyword(LanguageConstants.DeployKeyword);
            var path = this.WithRecovery(() => ThrowIfSkipped(this.InterpolableString, b => b.ExpectedModulePathString()), RecoveryFlags.None, TokenType.NewLine);
            var body = this.WithRecovery(() => this.Object(ExpressionFlags.AllowComplexLiterals), GetSuppressionFlag(path), TokenType.NewLine);

            return new(keyword, path, body);
        }
    }
}