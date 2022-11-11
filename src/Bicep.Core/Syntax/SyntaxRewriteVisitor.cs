// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            if (TryRewriteStrict(syntax, out var newSyntax))
            {
                return newSyntax;
            }

            return syntax;
        }

        public TOut? Rewrite<TSyntax, TOut>(TSyntax syntax)
            where TSyntax : SyntaxBase
            where TOut : SyntaxBase
        {
            if (TryRewrite(syntax, out var newSyntax))
            {
                return newSyntax as TOut;
            }

            return null;
        }

        protected virtual SyntaxBase RewriteInternal(SyntaxBase syntax)
        {
            currentSyntax = null;
            syntax.Accept(this);

            if (currentSyntax is null)
            {
                throw new InvalidOperationException($"Expected {nameof(currentSyntax)} to not be null");
            }

            return currentSyntax;
        }

        private bool TryRewrite(SyntaxBase? syntax, [NotNullIfNotNull("syntax")] out SyntaxBase? newSyntax)
            => TryRewriteStrict<SyntaxBase>(syntax, out newSyntax);

        private bool TryRewriteStrict<TSyntax>(TSyntax? syntax, [NotNullIfNotNull("syntax")] out TSyntax? newSyntax)
            where TSyntax : SyntaxBase
        {
            if (syntax is null)
            {
                newSyntax = null;
                return false;
            }

            var newSyntaxUntyped = RewriteInternal(syntax);
            var hasChanges = !object.ReferenceEquals(syntax, newSyntaxUntyped);

            if (newSyntaxUntyped is not TSyntax newSyntaxTyped)
            {
                throw new InvalidOperationException($"Expected {nameof(currentSyntax)} to be of type {typeof(TSyntax)}");
            }

            newSyntax = newSyntaxTyped;
            return hasChanges;
        }

        private bool TryRewrite(IEnumerable<SyntaxBase> syntaxes, out IEnumerable<SyntaxBase> newSyntaxes)
            => TryRewriteStrict<SyntaxBase>(syntaxes, out newSyntaxes);

        private bool TryRewriteStrict<TSyntax>(IEnumerable<TSyntax> syntaxes, out IEnumerable<TSyntax> newSyntaxes)
            where TSyntax : SyntaxBase
        {
            var hasChanges = false;
            var newSyntaxList = new List<TSyntax>();
            foreach (var syntax in syntaxes)
            {
                hasChanges |= TryRewriteStrict(syntax, out var newSyntax);
                newSyntaxList.Add(newSyntax);
            }

            newSyntaxes = hasChanges ? newSyntaxList : syntaxes;
            return hasChanges;
        }

        private void ReplaceCurrent<TSyntax>(TSyntax syntax, Func<TSyntax, SyntaxBase> replaceFunc)
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
            var hasChanges = TryRewrite(syntax.Elements, out var elements);
            hasChanges |= TryRewrite(syntax.Separators, out var separators);

            if (!hasChanges)
            {
                return syntax;
            }

            return new SeparatedSyntaxList(elements, separators, new TextSpan(0, 0));
        }
        void ISyntaxVisitor.VisitSeparatedSyntaxList(SeparatedSyntaxList syntax) => ReplaceCurrent(syntax, ReplaceSeparatedSyntaxList);

        protected virtual SyntaxBase ReplaceMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new MetadataDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitMetadataDeclarationSyntax(MetadataDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceMetadataDeclarationSyntax);

        protected virtual SyntaxBase ReplaceParameterDeclarationSyntax(ParameterDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Type, out var type);
            hasChanges |= TryRewrite(syntax.Modifier, out var modifier);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterDeclarationSyntax(leadingNodes, keyword, name, type, modifier);
        }
        void ISyntaxVisitor.VisitParameterDeclarationSyntax(ParameterDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceParameterDeclarationSyntax);

        protected virtual SyntaxBase ReplaceParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.AssignmentToken, out var assignmentToken);
            hasChanges |= TryRewrite(syntax.DefaultValue, out var defaultValue);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterDefaultValueSyntax(assignmentToken, defaultValue);
        }
        void ISyntaxVisitor.VisitParameterDefaultValueSyntax(ParameterDefaultValueSyntax syntax) => ReplaceCurrent(syntax, ReplaceParameterDefaultValueSyntax);

        protected virtual SyntaxBase VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.Assignment, out var assignment);
            hasChanges |= TryRewriteStrict(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterAssignmentSyntax(keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) => ReplaceCurrent(syntax, VisitParameterAssignmentSyntax);

        protected virtual SyntaxBase VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Path, out var path);

            if (!hasChanges)
            {
                return syntax;
            }

            return new UsingDeclarationSyntax(keyword, path);
        }
        void ISyntaxVisitor.VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax) => ReplaceCurrent(syntax, VisitUsingDeclarationSyntax);

        protected virtual SyntaxBase ReplaceVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new VariableDeclarationSyntax(keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitVariableDeclarationSyntax(VariableDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceVariableDeclarationSyntax);

        protected virtual SyntaxBase ReplaceLocalVariableSyntax(LocalVariableSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Name, out var name);
            if (!hasChanges)
            {
                return syntax;
            }

            return new LocalVariableSyntax(name);
        }

        void ISyntaxVisitor.VisitLocalVariableSyntax(LocalVariableSyntax syntax) => ReplaceCurrent(syntax, ReplaceLocalVariableSyntax);

        protected virtual SyntaxBase ReplaceTargetScopeSyntax(TargetScopeSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TargetScopeSyntax(leadingNodes, keyword, assignment, value);
        }
        void ISyntaxVisitor.VisitTargetScopeSyntax(TargetScopeSyntax syntax) => ReplaceCurrent(syntax, ReplaceTargetScopeSyntax);

        protected virtual SyntaxBase ReplaceResourceDeclarationSyntax(ResourceDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Type, out var type);
            hasChanges |= TryRewriteStrict(syntax.ExistingKeyword, out var existingKeyword);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ResourceDeclarationSyntax(leadingNodes, keyword, name, type, existingKeyword, assignment, value);
        }
        void ISyntaxVisitor.VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceResourceDeclarationSyntax);

        protected virtual SyntaxBase ReplaceModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Path, out var path);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ModuleDeclarationSyntax(leadingNodes, keyword, name, path, assignment, value);
        }
        void ISyntaxVisitor.VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceModuleDeclarationSyntax);

        protected virtual SyntaxBase ReplaceOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Type, out var type);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new OutputDeclarationSyntax(leadingNodes, keyword, name, type, assignment, value);
        }
        void ISyntaxVisitor.VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceOutputDeclarationSyntax);

        protected virtual SyntaxBase ReplaceImportDeclarationSyntax(ImportDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.SpecificationString, out var specification);
            hasChanges |= TryRewriteStrict(syntax.WithClause, out var withClause);
            hasChanges |= TryRewriteStrict(syntax.AsClause, out var asClause);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ImportDeclarationSyntax(leadingNodes, keyword, specification, withClause, asClause);
        }
        void ISyntaxVisitor.VisitImportDeclarationSyntax(ImportDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceImportDeclarationSyntax);

        protected virtual SyntaxBase ReplaceImportWithClauseSyntax(ImportWithClauseSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Config, out var config);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ImportWithClauseSyntax(keyword, config);
        }
        void ISyntaxVisitor.VisitImportWithClauseSyntax(ImportWithClauseSyntax syntax) => ReplaceCurrent(syntax, ReplaceImportWithClauseSyntax);

        protected virtual SyntaxBase ReplaceImportAsClauseSyntax(ImportAsClauseSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Alias, out var alias);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ImportAsClauseSyntax(keyword, alias);
        }
        void ISyntaxVisitor.VisitImportAsClauseSyntax(ImportAsClauseSyntax syntax) => ReplaceCurrent(syntax, ReplaceImportAsClauseSyntax);

        protected virtual SyntaxBase ReplaceIdentifierSyntax(IdentifierSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Child, out var child);

            if (!hasChanges)
            {
                return syntax;
            }

            return new IdentifierSyntax(child);
        }
        void ISyntaxVisitor.VisitIdentifierSyntax(IdentifierSyntax syntax) => ReplaceCurrent(syntax, ReplaceIdentifierSyntax);

        protected virtual SyntaxBase ReplaceResourceTypeSyntax(ResourceTypeSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Type, out var type);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ResourceTypeSyntax(keyword, type);
        }
        void ISyntaxVisitor.VisitResourceTypeSyntax(ResourceTypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceResourceTypeSyntax);

        protected virtual SyntaxBase ReplaceObjectTypeSyntax(ObjectTypeSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenBrace, out var openBrace);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseBrace, out var closeBrace);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectTypeSyntax(openBrace, children, closeBrace);
        }
        void ISyntaxVisitor.VisitObjectTypeSyntax(ObjectTypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectTypeSyntax);

        protected virtual SyntaxBase ReplaceObjectPropertyTypeSyntax(ObjectTypePropertySyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewrite(syntax.Key, out var key);
            hasChanges |= TryRewrite(syntax.OptionalityMarker, out var optionalityMarker);
            hasChanges |= TryRewriteStrict(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectTypePropertySyntax(leadingNodes, key, optionalityMarker, colon, value);
        }
        void ISyntaxVisitor.VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectPropertyTypeSyntax);

        protected virtual SyntaxBase ReplaceTupleTypeSyntax(TupleTypeSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenBracket, out var openBracket);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseBracket, out var closeBracket);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TupleTypeSyntax(openBracket, children, closeBracket);
        }
        void ISyntaxVisitor.VisitTupleTypeSyntax(TupleTypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceTupleTypeSyntax);

        protected virtual SyntaxBase ReplaceTupleTypeItemSyntax(TupleTypeItemSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TupleTypeItemSyntax(leadingNodes, value);
        }
        void ISyntaxVisitor.VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax) => ReplaceCurrent(syntax, ReplaceTupleTypeItemSyntax);

        protected virtual SyntaxBase ReplaceArrayTypeSyntax(ArrayTypeSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Item, out var item);
            hasChanges |= TryRewriteStrict(syntax.OpenBracket, out var openBracket);
            hasChanges |= TryRewriteStrict(syntax.CloseBracket, out var closeBracket);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayTypeSyntax(item, openBracket, closeBracket);
        }
        void ISyntaxVisitor.VisitArrayTypeSyntax(ArrayTypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayTypeSyntax);

        protected virtual SyntaxBase ReplaceArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayTypeMemberSyntax(value);
        }
        void ISyntaxVisitor.VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayTypeMemberSyntax);

        protected virtual SyntaxBase ReplaceUnionTypeSyntax(UnionTypeSyntax syntax)
        {
            if (TryRewrite(syntax.Children, out var children))
            {
                return new UnionTypeSyntax(children);
            }

            return syntax;
        }
        void ISyntaxVisitor.VisitUnionTypeSyntax(UnionTypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceUnionTypeSyntax);

        protected virtual SyntaxBase ReplaceUnionMemberTypeSyntax(UnionTypeMemberSyntax syntax)
        {
            if (TryRewrite(syntax.Value, out var value))
            {
                return new UnionTypeMemberSyntax(value);
            }

            return syntax;
        }
        void ISyntaxVisitor.VisitUnionTypeMemberSyntax(UnionTypeMemberSyntax syntax) => ReplaceCurrent(syntax, ReplaceUnionMemberTypeSyntax);

        protected virtual SyntaxBase ReplaceTypeDeclarationSyntax(TypeDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypeDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitTypeDeclarationSyntax(TypeDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceTypeDeclarationSyntax);

        protected virtual SyntaxBase ReplaceBooleanLiteralSyntax(BooleanLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Literal, out var literal);

            if (!hasChanges)
            {
                return syntax;
            }

            return new BooleanLiteralSyntax(literal, bool.Parse(literal.Text));
        }
        void ISyntaxVisitor.VisitBooleanLiteralSyntax(BooleanLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceBooleanLiteralSyntax);

        protected virtual SyntaxBase ReplaceStringSyntax(StringSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.StringTokens, out var stringTokens);
            hasChanges |= TryRewrite(syntax.Expressions, out var expressions);

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

        protected virtual SyntaxBase ReplaceProgramSyntax(ProgramSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.EndOfFile, out var endOfFile);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ProgramSyntax(children, endOfFile, Enumerable.Empty<IDiagnostic>());
        }
        void ISyntaxVisitor.VisitProgramSyntax(ProgramSyntax syntax) => ReplaceCurrent(syntax, ReplaceProgramSyntax);

        protected virtual SyntaxBase ReplaceIntegerLiteralSyntax(IntegerLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Literal, out var literal);

            if (!hasChanges)
            {
                return syntax;
            }

            return new IntegerLiteralSyntax(literal, ulong.Parse(literal.Text));
        }
        void ISyntaxVisitor.VisitIntegerLiteralSyntax(IntegerLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceIntegerLiteralSyntax);

        protected virtual SyntaxBase ReplaceNullLiteralSyntax(NullLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.NullKeyword, out var nullKeyword);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NullLiteralSyntax(nullKeyword);
        }
        void ISyntaxVisitor.VisitNullLiteralSyntax(NullLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceNullLiteralSyntax);

        protected virtual SyntaxBase ReplaceSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Elements, out var elements);

            if (!hasChanges)
            {
                return syntax;
            }

            return new SkippedTriviaSyntax(new TextSpan(0, 0), elements, Enumerable.Empty<IDiagnostic>());
        }
        void ISyntaxVisitor.VisitSkippedTriviaSyntax(SkippedTriviaSyntax syntax) => ReplaceCurrent(syntax, ReplaceSkippedTriviaSyntax);

        protected virtual SyntaxBase ReplaceObjectSyntax(ObjectSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenBrace, out var openBrace);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseBrace, out var closeBrace);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectSyntax(openBrace, children, closeBrace);
        }
        void ISyntaxVisitor.VisitObjectSyntax(ObjectSyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectSyntax);

        protected virtual SyntaxBase ReplaceObjectPropertySyntax(ObjectPropertySyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Key, out var key);
            hasChanges |= TryRewrite(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectPropertySyntax(key, colon, value);
        }
        void ISyntaxVisitor.VisitObjectPropertySyntax(ObjectPropertySyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectPropertySyntax);

        protected virtual SyntaxBase ReplaceArraySyntax(ArraySyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenBracket, out var openBracket);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseBracket, out var closeBracket);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArraySyntax(openBracket, children, closeBracket);
        }
        void ISyntaxVisitor.VisitArraySyntax(ArraySyntax syntax) => ReplaceCurrent(syntax, ReplaceArraySyntax);

        protected virtual SyntaxBase ReplaceArrayItemSyntax(ArrayItemSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayItemSyntax(value);
        }
        void ISyntaxVisitor.VisitArrayItemSyntax(ArrayItemSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayItemSyntax);

        protected virtual SyntaxBase ReplaceTernaryOperationSyntax(TernaryOperationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.ConditionExpression, out var conditionExpression);
            hasChanges |= TryRewriteStrict(syntax.Question, out var question);
            hasChanges |= TryRewrite(syntax.TrueExpression, out var trueExpression);
            hasChanges |= TryRewriteStrict(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.FalseExpression, out var falseExpression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TernaryOperationSyntax(conditionExpression, question, trueExpression, colon, falseExpression);
        }
        void ISyntaxVisitor.VisitTernaryOperationSyntax(TernaryOperationSyntax syntax) => ReplaceCurrent(syntax, ReplaceTernaryOperationSyntax);

        protected virtual SyntaxBase ReplaceBinaryOperationSyntax(BinaryOperationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeftExpression, out var leftExpression);
            hasChanges |= TryRewriteStrict(syntax.OperatorToken, out var operatorToken);
            hasChanges |= TryRewrite(syntax.RightExpression, out var rightExpression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new BinaryOperationSyntax(leftExpression, operatorToken, rightExpression);
        }
        void ISyntaxVisitor.VisitBinaryOperationSyntax(BinaryOperationSyntax syntax) => ReplaceCurrent(syntax, ReplaceBinaryOperationSyntax);

        protected virtual SyntaxBase ReplaceUnaryOperationSyntax(UnaryOperationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OperatorToken, out var operatorToken);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new UnaryOperationSyntax(operatorToken, expression);
        }
        void ISyntaxVisitor.VisitUnaryOperationSyntax(UnaryOperationSyntax syntax) => ReplaceCurrent(syntax, ReplaceUnaryOperationSyntax);

        protected virtual SyntaxBase ReplaceArrayAccessSyntax(ArrayAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.OpenSquare, out var openSquare);
            hasChanges |= TryRewrite(syntax.IndexExpression, out var indexExpression);
            hasChanges |= TryRewriteStrict(syntax.CloseSquare, out var closeSquare);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayAccessSyntax(baseExpression, openSquare, indexExpression, closeSquare);
        }
        void ISyntaxVisitor.VisitArrayAccessSyntax(ArrayAccessSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayAccessSyntax);

        protected virtual SyntaxBase ReplacePropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.Dot, out var dot);
            hasChanges |= TryRewriteStrict(syntax.PropertyName, out var propertyName);

            if (!hasChanges)
            {
                return syntax;
            }

            return new PropertyAccessSyntax(baseExpression, dot, propertyName);
        }
        void ISyntaxVisitor.VisitPropertyAccessSyntax(PropertyAccessSyntax syntax) => ReplaceCurrent(syntax, ReplacePropertyAccessSyntax);

        protected virtual SyntaxBase ReplaceResourceAccessSyntax(ResourceAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.DoubleColon, out var doubleColon);
            hasChanges |= TryRewriteStrict(syntax.ResourceName, out var propertyName);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ResourceAccessSyntax(baseExpression, doubleColon, propertyName);
        }
        void ISyntaxVisitor.VisitResourceAccessSyntax(ResourceAccessSyntax syntax) => ReplaceCurrent(syntax, ReplaceResourceAccessSyntax);

        protected virtual SyntaxBase ReplaceParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenParen, out var openParen);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);
            hasChanges |= TryRewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParenthesizedExpressionSyntax(openParen, expression, closeParen);
        }
        void ISyntaxVisitor.VisitParenthesizedExpressionSyntax(ParenthesizedExpressionSyntax syntax) => ReplaceCurrent(syntax, ReplaceParenthesizedExpressionSyntax);

        protected virtual SyntaxBase ReplaceFunctionCallSyntax(FunctionCallSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.OpenParen, out var openParen);
            hasChanges |= TryRewriteStrict(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new FunctionCallSyntax(name, openParen, children, closeParen);
        }
        void ISyntaxVisitor.VisitFunctionCallSyntax(FunctionCallSyntax syntax) => ReplaceCurrent(syntax, ReplaceFunctionCallSyntax);

        protected virtual SyntaxBase ReplaceInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.Dot, out var dot);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.OpenParen, out var openParen);
            hasChanges |= TryRewriteStrict(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new InstanceFunctionCallSyntax(baseExpression, dot, name, openParen, children, closeParen);
        }

        void ISyntaxVisitor.VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax) => ReplaceCurrent(syntax, ReplaceInstanceFunctionCallSyntax);

        protected virtual SyntaxBase ReplaceFunctionArgumentSyntax(FunctionArgumentSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new FunctionArgumentSyntax(expression);
        }
        void ISyntaxVisitor.VisitFunctionArgumentSyntax(FunctionArgumentSyntax syntax) => ReplaceCurrent(syntax, ReplaceFunctionArgumentSyntax);

        protected virtual SyntaxBase ReplaceVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Name, out var name);

            if (!hasChanges)
            {
                return syntax;
            }

            return new VariableAccessSyntax(name);
        }
        void ISyntaxVisitor.VisitVariableAccessSyntax(VariableAccessSyntax syntax) => ReplaceCurrent(syntax, ReplaceVariableAccessSyntax);

        protected virtual SyntaxBase ReplaceIfExpressionSyntax(IfConditionSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewrite(syntax.ConditionExpression, out var conditionExpression);
            hasChanges |= TryRewrite(syntax.Body, out var body);

            if (!hasChanges)
            {
                return syntax;
            }

            return new IfConditionSyntax(keyword, conditionExpression, body);
        }
        void ISyntaxVisitor.VisitIfConditionSyntax(IfConditionSyntax syntax) => ReplaceCurrent(syntax, ReplaceIfExpressionSyntax);

        protected virtual SyntaxBase ReplaceForSyntax(ForSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenSquare, out var openSquare);
            hasChanges |= TryRewriteStrict(syntax.ForKeyword, out var forKeyword);
            hasChanges |= TryRewrite(syntax.VariableSection, out var itemVariable);
            hasChanges |= TryRewrite(syntax.InKeyword, out var inKeyword);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);
            hasChanges |= TryRewrite(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.Body, out var body);
            hasChanges |= TryRewrite(syntax.CloseSquare, out var closeSquare);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ForSyntax(openSquare, forKeyword, itemVariable, inKeyword, expression, colon, body, closeSquare);
        }

        void ISyntaxVisitor.VisitForSyntax(ForSyntax syntax) => ReplaceCurrent(syntax, ReplaceForSyntax);

        protected virtual SyntaxBase ReplaceVariableBlockSyntax(VariableBlockSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenParen, out var openParen);
            hasChanges |= TryRewriteStrict(syntax.Children, out var children);
            hasChanges |= TryRewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new VariableBlockSyntax(openParen, children, closeParen);
        }

        void ISyntaxVisitor.VisitVariableBlockSyntax(VariableBlockSyntax syntax) => ReplaceCurrent(syntax, ReplaceVariableBlockSyntax);

        protected virtual SyntaxBase ReplaceDecoratorSyntax(DecoratorSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.At, out var at);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new DecoratorSyntax(at, expression);
        }
        void ISyntaxVisitor.VisitDecoratorSyntax(DecoratorSyntax syntax) => ReplaceCurrent(syntax, ReplaceDecoratorSyntax);

        protected virtual SyntaxBase ReplaceMissingDeclarationSyntax(MissingDeclarationSyntax syntax)
        {
            var hasChange = TryRewriteStrict(syntax.LeadingNodes, out var leadingNodes);

            if (!hasChange)
            {
                return syntax;
            }

            return new MissingDeclarationSyntax(leadingNodes);
        }
        void ISyntaxVisitor.VisitMissingDeclarationSyntax(MissingDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceMissingDeclarationSyntax);

        protected virtual SyntaxBase ReplaceLambdaSyntax(LambdaSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.VariableSection, out var variableSection);
            hasChanges |= TryRewriteStrict(syntax.Arrow, out var arrow);
            hasChanges |= TryRewrite(syntax.Body, out var body);

            if (!hasChanges)
            {
                return syntax;
            }

            return new LambdaSyntax(variableSection, arrow, body);
        }
        void ISyntaxVisitor.VisitLambdaSyntax(LambdaSyntax syntax) => ReplaceCurrent(syntax, ReplaceLambdaSyntax);
    }
}
