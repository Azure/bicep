// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public abstract class SyntaxRewriteVisitor : ISyntaxVisitor
    {
        private SyntaxBase? currentSyntax;

        public TSyntax Rewrite<TSyntax>(TSyntax syntax)
            where TSyntax : SyntaxBase
        {
            if (Rewrite(syntax, out var newSyntax))
            {
                return newSyntax;
            }

            return syntax;
        }

        private bool Rewrite<TSyntax>(TSyntax syntax, out TSyntax newSyntax)
            where TSyntax : SyntaxBase
        {
            currentSyntax = null;
            syntax.Accept(this);

            if (currentSyntax is not TSyntax rewrittenSyntax)
            {
                throw new InvalidOperationException($"Expected {nameof(currentSyntax)} to be of type {typeof(TSyntax)}");
            }

            newSyntax = rewrittenSyntax;
            return !object.ReferenceEquals(newSyntax, syntax);
        }

        private bool Rewrite<TSyntax>(IEnumerable<TSyntax> syntaxes, out IEnumerable<TSyntax> newSyntaxes)
            where TSyntax : SyntaxBase
        {
            var hasChanges = false;
            var newSyntaxList = new List<TSyntax>();
            foreach (var syntax in syntaxes)
            {
                hasChanges |= Rewrite(syntax, out var newSyntax);
                newSyntaxList.Add(newSyntax);
            }

            newSyntaxes = hasChanges ? newSyntaxList : syntaxes;
            return hasChanges;
        }

        private bool RewriteNullable<TSyntax>(TSyntax? syntax, out TSyntax? newSyntax)
            where TSyntax : SyntaxBase
        {
            if (syntax is null)
            {
                newSyntax = null;
                return false;
            }

            var hasChanges = Rewrite(syntax, out var newSyntaxNullable);
            newSyntax = newSyntaxNullable;

            return hasChanges;
        }

        private void ReplaceCurrent<TSyntax>(TSyntax syntax, Func<TSyntax, TSyntax> replaceFunc)
            where TSyntax : SyntaxBase
        {
            if (currentSyntax is not null)
            {
                throw new InvalidOperationException($"Expected {nameof(currentSyntax)} to be null");
            }

            currentSyntax = replaceFunc(syntax);
        }

        protected virtual Token ReplaceToken(Token syntax) => syntax;
        void ISyntaxVisitor.VisitToken(Token syntax) => ReplaceCurrent(syntax, ReplaceToken);

        protected virtual SeparatedSyntaxList ReplaceSeparatedSyntaxList(SeparatedSyntaxList syntax)
        {
            var hasChanges = Rewrite(syntax.Elements, out var elements);
            hasChanges |= Rewrite(syntax.Separators, out var separators);

            if (!hasChanges)
            {
                return syntax;
            }

            return new SeparatedSyntaxList(elements, separators, new TextSpan(0, 0));
        }
        void ISyntaxVisitor.VisitSeparatedSyntaxList(SeparatedSyntaxList syntax) => ReplaceCurrent(syntax, ReplaceSeparatedSyntaxList);

        protected virtual ParameterDeclarationSyntax ReplaceParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Keyword, out var keyword);
            hasChanges |= Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.Type, out var type);
            hasChanges |= RewriteNullable(syntax.Modifier, out var modifier);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterDeclarationSyntax(keyword, name, type, modifier);
        }
        void ISyntaxVisitor.VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceParameterDeclarationSyntax);

        protected virtual ParameterDefaultValueSyntax ReplaceParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.AssignmentToken, out var assignmentToken);
            hasChanges |= Rewrite(syntax.DefaultValue, out var defaultValue);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterDefaultValueSyntax(assignmentToken, defaultValue);
        }
        void ISyntaxVisitor.VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax) => ReplaceCurrent(syntax, ReplaceParameterDefaultValueSyntax);

        protected virtual VariableDeclarationSyntax ReplaceVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Keyword, out var keyword);
            hasChanges |= Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.Assignment, out var assignment);
            hasChanges |= Rewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new VariableDeclarationSyntax(keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceVariableDeclarationSyntax);

        protected virtual TargetScopeSyntax ReplaceTargetScopeSyntax(TargetScopeSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Keyword, out var keyword);
            hasChanges |= Rewrite(syntax.Assignment, out var assignment);
            hasChanges |= Rewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TargetScopeSyntax(keyword, assignment, value);
        }
        void ISyntaxVisitor.VisitTargetScopeSyntax(TargetScopeSyntax syntax) => ReplaceCurrent(syntax, ReplaceTargetScopeSyntax);

        protected virtual ResourceDeclarationSyntax ReplaceResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Keyword, out var keyword);
            hasChanges |= Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.Type, out var type);
            hasChanges |= Rewrite(syntax.Assignment, out var assignment);
            hasChanges |= Rewrite(syntax.Body, out var body);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ResourceDeclarationSyntax(keyword, name, type, assignment, body);
        }
        void ISyntaxVisitor.VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceResourceDeclarationSyntax);

        protected virtual ModuleDeclarationSyntax ReplaceModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Keyword, out var keyword);
            hasChanges |= Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.Path, out var path);
            hasChanges |= Rewrite(syntax.Assignment, out var assignment);
            hasChanges |= Rewrite(syntax.Body, out var body);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ModuleDeclarationSyntax(keyword, name, path, assignment, body);
        }
        void ISyntaxVisitor.VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceModuleDeclarationSyntax);

        protected virtual OutputDeclarationSyntax ReplaceOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Keyword, out var keyword);
            hasChanges |= Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.Type, out var type);
            hasChanges |= Rewrite(syntax.Assignment, out var assignment);
            hasChanges |= Rewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new OutputDeclarationSyntax(keyword, name, type, assignment, value);
        }
        void ISyntaxVisitor.VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceOutputDeclarationSyntax);

        protected virtual IdentifierSyntax ReplaceIdentifierSyntax(IdentifierSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Child, out var child);

            if (!hasChanges)
            {
                return syntax;
            }

            return new IdentifierSyntax(child);
        }
        void ISyntaxVisitor.VisitIdentifierSyntax(IdentifierSyntax syntax) => ReplaceCurrent(syntax, ReplaceIdentifierSyntax);

        protected virtual TypeSyntax ReplaceTypeSyntax(TypeSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Identifier, out var identifier);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypeSyntax(identifier);
        }
        void ISyntaxVisitor.VisitTypeSyntax(TypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceTypeSyntax);

        protected virtual BooleanLiteralSyntax ReplaceBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Literal, out var literal);

            if (!hasChanges)
            {
                return syntax;
            }

            return new BooleanLiteralSyntax(literal, bool.Parse(literal.Text));
        }
        void ISyntaxVisitor.VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceBooleanLiteralSyntax);

        protected virtual StringSyntax ReplaceStringSyntax(StringSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.StringTokens, out var stringTokens);
            hasChanges |= Rewrite(syntax.Expressions, out var expressions);

            if (!hasChanges)
            {
                return syntax;
            }

            var segmentValues = Lexer.TryGetRawStringSegments(stringTokens.ToArray());
            if (segmentValues == null)
            {
                throw new ArgumentException($"Failed to parse string tokens");
            }

            return new StringSyntax(stringTokens, expressions, segmentValues);
        }
        void ISyntaxVisitor.VisitStringSyntax(StringSyntax syntax) => ReplaceCurrent(syntax, ReplaceStringSyntax);

        protected virtual ProgramSyntax ReplaceProgramSyntax(ProgramSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Children, out var children);
            hasChanges |= Rewrite(syntax.EndOfFile, out var endOfFile);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ProgramSyntax(children, endOfFile, Enumerable.Empty<Diagnostic>());
        }
        void ISyntaxVisitor.VisitProgramSyntax(ProgramSyntax syntax) => ReplaceCurrent(syntax, ReplaceProgramSyntax);

        protected virtual NumericLiteralSyntax ReplaceNumericLiteralSyntax(NumericLiteralSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Literal, out var literal);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NumericLiteralSyntax(literal, int.Parse(literal.Text));
        }
        void ISyntaxVisitor.VisitNumericLiteralSyntax(NumericLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceNumericLiteralSyntax);

        protected virtual NullLiteralSyntax ReplaceNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.NullKeyword, out var nullKeyword);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NullLiteralSyntax(nullKeyword);
        }
        void ISyntaxVisitor.VisitNullLiteralSyntax(NullLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceNullLiteralSyntax);

        protected virtual SkippedTriviaSyntax ReplaceSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Elements, out var elements);

            if (!hasChanges)
            {
                return syntax;
            }

            return new SkippedTriviaSyntax(new TextSpan(0, 0), elements, Enumerable.Empty<Diagnostic>());
        }
        void ISyntaxVisitor.VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax) => ReplaceCurrent(syntax, ReplaceSkippedTriviaSyntax);

        protected virtual ObjectSyntax ReplaceObjectSyntax(ObjectSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.OpenBrace, out var openBrace);
            hasChanges |= Rewrite(syntax.Children, out var children);
            hasChanges |= Rewrite(syntax.CloseBrace, out var closeBrace);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectSyntax(openBrace, children, closeBrace);
        }
        void ISyntaxVisitor.VisitObjectSyntax(ObjectSyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectSyntax);

        protected virtual ObjectPropertySyntax ReplaceObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Key, out var key);
            hasChanges |= Rewrite(syntax.Colon, out var colon);
            hasChanges |= Rewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectPropertySyntax(key, colon, value);
        }
        void ISyntaxVisitor.VisitObjectPropertySyntax(ObjectPropertySyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectPropertySyntax);

        protected virtual ArraySyntax ReplaceArraySyntax(ArraySyntax syntax)
        {
            var hasChanges = Rewrite(syntax.OpenBracket, out var openBracket);
            hasChanges |= Rewrite(syntax.Children, out var children);
            hasChanges |= Rewrite(syntax.CloseBracket, out var closeBracket);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArraySyntax(openBracket, children, closeBracket);
        }
        void ISyntaxVisitor.VisitArraySyntax(ArraySyntax syntax) => ReplaceCurrent(syntax, ReplaceArraySyntax);

        protected virtual ArrayItemSyntax ReplaceArrayItemSyntax(ArrayItemSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayItemSyntax(value);
        }
        void ISyntaxVisitor.VisitArrayItemSyntax(ArrayItemSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayItemSyntax);

        protected virtual TernaryOperationSyntax ReplaceTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.ConditionExpression, out var conditionExpression);
            hasChanges |= Rewrite(syntax.Question, out var question);
            hasChanges |= Rewrite(syntax.TrueExpression, out var trueExpression);
            hasChanges |= Rewrite(syntax.Colon, out var colon);
            hasChanges |= Rewrite(syntax.FalseExpression, out var falseExpression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TernaryOperationSyntax(conditionExpression, question, trueExpression, colon, falseExpression);
        }
        void ISyntaxVisitor.VisitTernaryOperationSyntax(TernaryOperationSyntax syntax) => ReplaceCurrent(syntax, ReplaceTernaryOperationSyntax);

        protected virtual BinaryOperationSyntax ReplaceBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.LeftExpression, out var leftExpression);
            hasChanges |= Rewrite(syntax.OperatorToken, out var operatorToken);
            hasChanges |= Rewrite(syntax.RightExpression, out var rightExpression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new BinaryOperationSyntax(leftExpression, operatorToken, rightExpression);
        }
        void ISyntaxVisitor.VisitBinaryOperationSyntax(BinaryOperationSyntax syntax) => ReplaceCurrent(syntax, ReplaceBinaryOperationSyntax);

        protected virtual UnaryOperationSyntax ReplaceUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.OperatorToken, out var operatorToken);
            hasChanges |= Rewrite(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new UnaryOperationSyntax(operatorToken, expression);
        }
        void ISyntaxVisitor.VisitUnaryOperationSyntax(UnaryOperationSyntax syntax) => ReplaceCurrent(syntax, ReplaceUnaryOperationSyntax);

        protected virtual ArrayAccessSyntax ReplaceArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= Rewrite(syntax.OpenSquare, out var openSquare);
            hasChanges |= Rewrite(syntax.IndexExpression, out var indexExpression);
            hasChanges |= Rewrite(syntax.CloseSquare, out var closeSquare);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayAccessSyntax(baseExpression, openSquare, indexExpression, closeSquare);
        }
        void ISyntaxVisitor.VisitArrayAccessSyntax(ArrayAccessSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayAccessSyntax);

        protected virtual PropertyAccessSyntax ReplacePropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= Rewrite(syntax.Dot, out var dot);
            hasChanges |= Rewrite(syntax.PropertyName, out var propertyName);

            if (!hasChanges)
            {
                return syntax;
            }

            return new PropertyAccessSyntax(baseExpression, dot, propertyName);
        }
        void ISyntaxVisitor.VisitPropertyAccessSyntax(PropertyAccessSyntax syntax) => ReplaceCurrent(syntax, ReplacePropertyAccessSyntax);

        protected virtual ParenthesizedExpressionSyntax ReplaceParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.OpenParen, out var openParen);
            hasChanges |= Rewrite(syntax.Expression, out var expression);
            hasChanges |= Rewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }
        void ISyntaxVisitor.VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) => ReplaceCurrent(syntax, ReplaceParenthesizedExpressionSyntax);

        protected virtual FunctionCallSyntax ReplaceFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.OpenParen, out var openParen);
            hasChanges |= Rewrite(syntax.Arguments, out var arguments);
            hasChanges |= Rewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new FunctionCallSyntax(name, openParen, arguments, closeParen);
        }
        void ISyntaxVisitor.VisitFunctionCallSyntax(FunctionCallSyntax syntax) => ReplaceCurrent(syntax, ReplaceFunctionCallSyntax);

        protected virtual InstanceFunctionCallSyntax ReplaceInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= Rewrite(syntax.Dot, out var dot);
            hasChanges |= Rewrite(syntax.Name, out var name);
            hasChanges |= Rewrite(syntax.OpenParen, out var openParen);
            hasChanges |= Rewrite(syntax.Arguments, out var arguments);
            hasChanges |= Rewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new InstanceFunctionCallSyntax(baseExpression, dot, name, openParen, arguments, closeParen);
        }

        void ISyntaxVisitor.VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) => ReplaceCurrent(syntax, ReplaceInstanceFunctionCallSyntax);

        protected virtual FunctionArgumentSyntax ReplaceFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Expression, out var expression);
            hasChanges |= RewriteNullable(syntax.Comma, out var comma);

            if (!hasChanges)
            {
                return syntax;
            }

            return new FunctionArgumentSyntax(expression, comma);
        }
        void ISyntaxVisitor.VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax) => ReplaceCurrent(syntax, ReplaceFunctionArgumentSyntax);

        protected virtual VariableAccessSyntax ReplaceVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var hasChanges = Rewrite(syntax.Name, out var name);

            if (!hasChanges)
            {
                return syntax;
            }

            return new VariableAccessSyntax(name);
        }
        void ISyntaxVisitor.VisitVariableAccessSyntax(VariableAccessSyntax syntax) => ReplaceCurrent(syntax, ReplaceVariableAccessSyntax);
    }
}
