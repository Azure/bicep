// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Parsing;
using Bicep.Core.Text;

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
            var hasChanges = TryRewriteStrict(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.Assignment, out var assignment);
            hasChanges |= TryRewriteStrict(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterAssignmentSyntax(leadingNodes, keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitParameterAssignmentSyntax(ParameterAssignmentSyntax syntax) => ReplaceCurrent(syntax, VisitParameterAssignmentSyntax);

        protected virtual SyntaxBase VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Path, out var path);
            hasChanges |= TryRewriteStrict(syntax.WithClause, out var withClause);

            if (!hasChanges)
            {
                return syntax;
            }

            return new UsingDeclarationSyntax(leadingNodes, keyword, path, withClause);
        }
        void ISyntaxVisitor.VisitUsingDeclarationSyntax(UsingDeclarationSyntax syntax) => ReplaceCurrent(syntax, VisitUsingDeclarationSyntax);

        protected virtual SyntaxBase ReplaceUsingWithClauseSyntax(UsingWithClauseSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Config, out var config);

            if (!hasChanges)
            {
                return syntax;
            }

            return new UsingWithClauseSyntax(keyword, config);
        }
        void ISyntaxVisitor.VisitUsingWithClauseSyntax(UsingWithClauseSyntax syntax) => ReplaceCurrent(syntax, ReplaceUsingWithClauseSyntax);

        protected virtual SyntaxBase ReplaceExtendsDeclarationSyntax(ExtendsDeclarationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Path, out var path);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ExtendsDeclarationSyntax(leadingNodes, keyword, path);
        }
        void ISyntaxVisitor.VisitExtendsDeclarationSyntax(ExtendsDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceExtendsDeclarationSyntax);

        protected virtual SyntaxBase ReplaceVariableDeclarationSyntax(VariableDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.Type, out var type);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new VariableDeclarationSyntax(leadingNodes, keyword, name, type, assignment, value);
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
        protected virtual SyntaxBase ReplaceAssertDeclarationSyntax(AssertDeclarationSyntax syntax)
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

            return new AssertDeclarationSyntax(leadingNodes, keyword, name, assignment, value);
        }
        void ISyntaxVisitor.VisitAssertDeclarationSyntax(AssertDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceAssertDeclarationSyntax);

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
            hasChanges |= TryRewrite(syntax.Newlines, out var newlines);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ResourceDeclarationSyntax(leadingNodes, keyword, name, type, existingKeyword, assignment, newlines.Cast<Token>().ToImmutableArray(), value);
        }
        void ISyntaxVisitor.VisitResourceDeclarationSyntax(ResourceDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceResourceDeclarationSyntax);

        protected virtual SyntaxBase ReplaceModuleDeclarationSyntax(ModuleDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Path, out var path);
            hasChanges |= TryRewrite(syntax.Assignment, out var assignment);
            hasChanges |= TryRewrite(syntax.Newlines, out var newlines);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ModuleDeclarationSyntax(leadingNodes, keyword, name, path, assignment, newlines.Cast<Token>().ToImmutableArray(), value);
        }
        void ISyntaxVisitor.VisitModuleDeclarationSyntax(ModuleDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceModuleDeclarationSyntax);

        protected virtual SyntaxBase ReplaceTestDeclarationSyntax(TestDeclarationSyntax syntax)
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

            return new TestDeclarationSyntax(leadingNodes, keyword, name, path, assignment, value);
        }
        void ISyntaxVisitor.VisitTestDeclarationSyntax(TestDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceTestDeclarationSyntax);

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

        protected virtual SyntaxBase ReplaceExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax)
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

            return new ExtensionDeclarationSyntax(leadingNodes, keyword, specification, withClause, asClause);
        }
        void ISyntaxVisitor.VisitExtensionDeclarationSyntax(ExtensionDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceExtensionDeclarationSyntax);

        protected virtual SyntaxBase ReplaceExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Alias, out var alias);
            hasChanges |= TryRewriteStrict(syntax.WithClause, out var withClause);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ExtensionConfigAssignmentSyntax(leadingNodes, keyword, alias, withClause);
        }

        void ISyntaxVisitor.VisitExtensionConfigAssignmentSyntax(ExtensionConfigAssignmentSyntax syntax) => ReplaceCurrent(syntax, ReplaceExtensionConfigAssignmentSyntax);

        protected virtual SyntaxBase ReplaceExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Config, out var config);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ExtensionWithClauseSyntax(keyword, config);
        }
        void ISyntaxVisitor.VisitExtensionWithClauseSyntax(ExtensionWithClauseSyntax syntax) => ReplaceCurrent(syntax, ReplaceExtensionWithClauseSyntax);

        protected virtual SyntaxBase ReplaceAliasAsClauseSyntax(AliasAsClauseSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Alias, out var alias);

            if (!hasChanges)
            {
                return syntax;
            }

            return new AliasAsClauseSyntax(keyword, alias);
        }
        void ISyntaxVisitor.VisitAliasAsClauseSyntax(AliasAsClauseSyntax syntax) => ReplaceCurrent(syntax, ReplaceAliasAsClauseSyntax);

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

        protected virtual SyntaxBase ReplaceObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewrite(syntax.Key, out var key);
            hasChanges |= TryRewriteStrict(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectTypePropertySyntax(leadingNodes, key, colon, value);
        }
        void ISyntaxVisitor.VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectTypePropertySyntax);

        protected virtual SyntaxBase ReplaceObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Asterisk, out var asterisk);
            hasChanges |= TryRewriteStrict(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.Value, out var value);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ObjectTypeAdditionalPropertiesSyntax(leadingNodes, asterisk, colon, value);
        }
        void ISyntaxVisitor.VisitObjectTypeAdditionalPropertiesSyntax(ObjectTypeAdditionalPropertiesSyntax syntax) => ReplaceCurrent(syntax, ReplaceObjectTypeAdditionalPropertiesSyntax);

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

        protected virtual SyntaxBase ReplaceNullableTypeSyntax(NullableTypeSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Base, out var expression);
            hasChanges |= TryRewriteStrict(syntax.NullabilityMarker, out var nullabilityMarker);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NullableTypeSyntax(expression, nullabilityMarker);
        }
        void ISyntaxVisitor.VisitNullableTypeSyntax(NullableTypeSyntax syntax) => ReplaceCurrent(syntax, ReplaceNullableTypeSyntax);

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

            return new ProgramSyntax(children, endOfFile);
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

        protected virtual SyntaxBase ReplaceNoneLiteralSyntax(NoneLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.NoneKeyword, out var noneKeyword);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NoneLiteralSyntax(noneKeyword);
        }
        void ISyntaxVisitor.VisitNoneLiteralSyntax(NoneLiteralSyntax syntax) => ReplaceCurrent(syntax, ReplaceNoneLiteralSyntax);

        protected virtual SyntaxBase ReplaceSkippedTriviaSyntax(SkippedTriviaSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Elements, out var elements);

            if (!hasChanges)
            {
                return syntax;
            }

            return new SkippedTriviaSyntax(new TextSpan(0, 0), elements);
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
            hasChanges |= TryRewrite(syntax.NewlinesBeforeQuestion, out var newlinesBeforeQuestion);
            hasChanges |= TryRewriteStrict(syntax.Question, out var question);
            hasChanges |= TryRewrite(syntax.TrueExpression, out var trueExpression);
            hasChanges |= TryRewrite(syntax.NewlinesBeforeColon, out var newlinesBeforeColon);
            hasChanges |= TryRewriteStrict(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.FalseExpression, out var falseExpression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TernaryOperationSyntax(
                conditionExpression,
                newlinesBeforeQuestion.Cast<Token>().ToImmutableArray(),
                question,
                trueExpression,
                newlinesBeforeColon.Cast<Token>().ToImmutableArray(),
                colon,
                falseExpression);
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
            hasChanges |= TryRewriteStrict(syntax.SafeAccessMarker, out var safeAccessMarker);
            hasChanges |= TryRewriteStrict(syntax.FromEndMarker, out var fromEndMarker);
            hasChanges |= TryRewrite(syntax.IndexExpression, out var indexExpression);
            hasChanges |= TryRewriteStrict(syntax.CloseSquare, out var closeSquare);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ArrayAccessSyntax(baseExpression, openSquare, safeAccessMarker, fromEndMarker, indexExpression, closeSquare);
        }
        void ISyntaxVisitor.VisitArrayAccessSyntax(ArrayAccessSyntax syntax) => ReplaceCurrent(syntax, ReplaceArrayAccessSyntax);

        protected virtual SyntaxBase ReplacePropertyAccessSyntax(PropertyAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.Dot, out var dot);
            hasChanges |= TryRewriteStrict(syntax.SafeAccessMarker, out var safeAccessMarker);
            hasChanges |= TryRewriteStrict(syntax.PropertyName, out var propertyName);

            if (!hasChanges)
            {
                return syntax;
            }

            return new PropertyAccessSyntax(baseExpression, dot, safeAccessMarker, propertyName);
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
            hasChanges |= TryRewrite(syntax.OpenNewlines, out var openNewlines);
            hasChanges |= TryRewriteStrict(syntax.ForKeyword, out var forKeyword);
            hasChanges |= TryRewrite(syntax.VariableSection, out var itemVariable);
            hasChanges |= TryRewrite(syntax.InKeyword, out var inKeyword);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);
            hasChanges |= TryRewrite(syntax.Colon, out var colon);
            hasChanges |= TryRewrite(syntax.Body, out var body);
            hasChanges |= TryRewrite(syntax.CloseNewlines, out var closeNewlines);
            hasChanges |= TryRewrite(syntax.CloseSquare, out var closeSquare);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ForSyntax(
                openSquare,
                openNewlines.Cast<Token>().ToImmutableArray(),
                forKeyword,
                itemVariable,
                inKeyword,
                expression,
                colon,
                body,
                closeNewlines.Cast<Token>().ToImmutableArray(),
                closeSquare);
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
            hasChanges |= TryRewrite(syntax.NewlinesBeforeBody, out var newlinesBeforeBody);
            hasChanges |= TryRewrite(syntax.Body, out var body);

            if (!hasChanges)
            {
                return syntax;
            }

            return new LambdaSyntax(variableSection, arrow, newlinesBeforeBody.Cast<Token>().ToImmutableArray(), body);
        }
        void ISyntaxVisitor.VisitLambdaSyntax(LambdaSyntax syntax) => ReplaceCurrent(syntax, ReplaceLambdaSyntax);

        protected virtual SyntaxBase ReplaceNonNullAssertionSyntax(NonNullAssertionSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.AssertionOperator, out var assertionOperator);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NonNullAssertionSyntax(baseExpression, assertionOperator);
        }
        void ISyntaxVisitor.VisitNonNullAssertionSyntax(NonNullAssertionSyntax syntax) => ReplaceCurrent(syntax, ReplaceNonNullAssertionSyntax);

        protected virtual SyntaxBase ReplaceTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenParen, out var openParen);
            hasChanges |= TryRewriteStrict(syntax.Children, out var children);
            hasChanges |= TryRewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypedVariableBlockSyntax(openParen, children, closeParen);
        }
        void ISyntaxVisitor.VisitTypedVariableBlockSyntax(TypedVariableBlockSyntax syntax) => ReplaceCurrent(syntax, ReplaceTypedVariableBlockSyntax);

        protected virtual SyntaxBase ReplaceTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.Type, out var type);
            if (!hasChanges)
            {
                return syntax;
            }

            return new TypedLocalVariableSyntax(name, type);
        }
        void ISyntaxVisitor.VisitTypedLocalVariableSyntax(TypedLocalVariableSyntax syntax) => ReplaceCurrent(syntax, ReplaceTypedLocalVariableSyntax);

        protected virtual SyntaxBase ReplaceTypedLambdaSyntax(TypedLambdaSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.VariableSection, out var variableSection);
            hasChanges |= TryRewrite(syntax.ReturnType, out var returnType);
            hasChanges |= TryRewriteStrict(syntax.Arrow, out var arrow);
            hasChanges |= TryRewrite(syntax.NewlinesBeforeBody, out var newlinesBeforeBody);
            hasChanges |= TryRewrite(syntax.Body, out var body);
            if (!hasChanges)
            {
                return syntax;
            }

            return new TypedLambdaSyntax(variableSection, returnType, arrow, newlinesBeforeBody.Cast<Token>().ToImmutableArray(), body);
        }
        void ISyntaxVisitor.VisitTypedLambdaSyntax(TypedLambdaSyntax syntax) => ReplaceCurrent(syntax, ReplaceTypedLambdaSyntax);

        protected virtual SyntaxBase ReplaceFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewrite(syntax.Lambda, out var lambda);

            if (!hasChanges)
            {
                return syntax;
            }

            return new FunctionDeclarationSyntax(leadingNodes, keyword, name, lambda);
        }
        void ISyntaxVisitor.VisitFunctionDeclarationSyntax(FunctionDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceFunctionDeclarationSyntax);

        protected virtual SyntaxBase ReplaceCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.LeadingNodes, out var leadingNodes);
            hasChanges |= TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewrite(syntax.ImportExpression, out var importExpression);
            hasChanges |= TryRewrite(syntax.FromClause, out var fromClause);

            if (!hasChanges)
            {
                return syntax;
            }

            return new CompileTimeImportDeclarationSyntax(leadingNodes, keyword, importExpression, fromClause);
        }
        void ISyntaxVisitor.VisitCompileTimeImportDeclarationSyntax(CompileTimeImportDeclarationSyntax syntax) => ReplaceCurrent(syntax, ReplaceCompileTimeImportDeclarationSyntax);

        protected virtual SyntaxBase ReplaceImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenBrace, out var openBrace);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseBrace, out var closeBrace);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ImportedSymbolsListSyntax(openBrace, children, closeBrace);
        }
        void ISyntaxVisitor.VisitImportedSymbolsListSyntax(ImportedSymbolsListSyntax syntax) => ReplaceCurrent(syntax, ReplaceImportedSymbolsListSyntax);

        protected virtual SyntaxBase ReplaceImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OriginalSymbolName, out var originalSymbolName);
            hasChanges |= TryRewrite(syntax.AsClause, out var asClause);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ImportedSymbolsListItemSyntax(originalSymbolName, asClause);
        }
        void ISyntaxVisitor.VisitImportedSymbolsListItemSyntax(ImportedSymbolsListItemSyntax syntax) => ReplaceCurrent(syntax, ReplaceImportedSymbolsListItemSyntax);

        protected virtual SyntaxBase ReplaceWildcardImportSyntax(WildcardImportSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Wildcard, out var wildcard);
            hasChanges |= TryRewriteStrict(syntax.AliasAsClause, out var aliasAsClause);

            if (!hasChanges)
            {
                return syntax;
            }

            return new WildcardImportSyntax(wildcard, aliasAsClause);
        }
        void ISyntaxVisitor.VisitWildcardImportSyntax(WildcardImportSyntax syntax) => ReplaceCurrent(syntax, ReplaceWildcardImportSyntax);

        protected virtual SyntaxBase ReplaceCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Keyword, out var keyword);
            hasChanges |= TryRewrite(syntax.Path, out var path);

            if (!hasChanges)
            {
                return syntax;
            }

            return new CompileTimeImportFromClauseSyntax(keyword, path);
        }
        void ISyntaxVisitor.VisitCompileTimeImportFromClauseSyntax(CompileTimeImportFromClauseSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceCompileTimeImportFromClauseSyntax);

        protected virtual SyntaxBase ReplaceParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Name, out var name);
            hasChanges |= TryRewriteStrict(syntax.OpenChevron, out var openChevron);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseChevron, out var closeChevron);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterizedTypeInstantiationSyntax(name, openChevron, children, closeChevron);
        }
        void ISyntaxVisitor.VisitParameterizedTypeInstantiationSyntax(ParameterizedTypeInstantiationSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceParameterizedTypeInstantiationSyntax);

        protected virtual SyntaxBase ReplaceInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.Dot, out var dot);
            hasChanges |= TryRewriteStrict(syntax.PropertyName, out var propertyName);
            hasChanges |= TryRewriteStrict(syntax.OpenChevron, out var openChevron);
            hasChanges |= TryRewrite(syntax.Children, out var children);
            hasChanges |= TryRewriteStrict(syntax.CloseChevron, out var closeChevron);

            if (!hasChanges)
            {
                return syntax;
            }

            return new InstanceParameterizedTypeInstantiationSyntax(baseExpression, dot, propertyName, openChevron, children, closeChevron);
        }
        void ISyntaxVisitor.VisitInstanceParameterizedTypeInstantiationSyntax(InstanceParameterizedTypeInstantiationSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceInstanceParameterizedTypeInstantiationSyntax);

        protected virtual SyntaxBase ReplaceTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.Dot, out var dot);
            hasChanges |= TryRewriteStrict(syntax.PropertyName, out var propertyName);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypePropertyAccessSyntax(baseExpression, dot, propertyName);
        }
        void ISyntaxVisitor.VisitTypePropertyAccessSyntax(TypePropertyAccessSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceTypePropertyAccessSyntax);

        protected virtual SyntaxBase ReplaceTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.Dot, out var dot);
            hasChanges |= TryRewriteStrict(syntax.Asterisk, out var asterisk);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypeAdditionalPropertiesAccessSyntax(baseExpression, dot, asterisk);
        }
        void ISyntaxVisitor.VisitTypeAdditionalPropertiesAccessSyntax(TypeAdditionalPropertiesAccessSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceTypeAdditionalPropertiesAccessSyntax);

        protected virtual SyntaxBase ReplaceTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.OpenSquare, out var openSquare);
            hasChanges |= TryRewrite(syntax.IndexExpression, out var indexExpression);
            hasChanges |= TryRewriteStrict(syntax.CloseSquare, out var closeSquare);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypeArrayAccessSyntax(baseExpression, openSquare, indexExpression, closeSquare);
        }
        void ISyntaxVisitor.VisitTypeArrayAccessSyntax(TypeArrayAccessSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceTypeArrayAccessSyntax);

        protected virtual SyntaxBase ReplaceTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.BaseExpression, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.OpenSquare, out var openBracket);
            hasChanges |= TryRewriteStrict(syntax.Asterisk, out var asterisk);
            hasChanges |= TryRewriteStrict(syntax.CloseSquare, out var closeBracket);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypeItemsAccessSyntax(baseExpression, openBracket, asterisk, closeBracket);
        }
        void ISyntaxVisitor.VisitTypeItemsAccessSyntax(TypeItemsAccessSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceTypeItemsAccessSyntax);

        protected virtual SyntaxBase ReplaceParameterizedTypeArgumentSyntax(ParameterizedTypeArgumentSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParameterizedTypeArgumentSyntax(expression);
        }
        void ISyntaxVisitor.VisitParameterizedTypeArgumentSyntax(ParameterizedTypeArgumentSyntax syntax) => ReplaceCurrent(syntax, ReplaceParameterizedTypeArgumentSyntax);

        protected virtual SyntaxBase ReplaceTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Name, out var name);

            if (!hasChanges)
            {
                return syntax;
            }

            return new TypeVariableAccessSyntax(name);
        }
        void ISyntaxVisitor.VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceTypeVariableAccessSyntax);

        protected virtual SyntaxBase ReplaceStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax)
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

            return new StringTypeLiteralSyntax(stringTokens, expressions, segmentValues);
        }
        void ISyntaxVisitor.VisitStringTypeLiteralSyntax(StringTypeLiteralSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceStringTypeLiteralSyntax);

        protected virtual SyntaxBase ReplaceIntegerTypeLiteralSyntax(IntegerTypeLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Literal, out var literal);

            if (!hasChanges)
            {
                return syntax;
            }

            return new IntegerTypeLiteralSyntax(literal, ulong.Parse(literal.Text));
        }
        void ISyntaxVisitor.VisitIntegerTypeLiteralSyntax(IntegerTypeLiteralSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceIntegerTypeLiteralSyntax);

        protected virtual SyntaxBase ReplaceBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Literal, out var literal);

            if (!hasChanges)
            {
                return syntax;
            }

            return new BooleanTypeLiteralSyntax(literal, bool.Parse(literal.Text));
        }
        void ISyntaxVisitor.VisitBooleanTypeLiteralSyntax(BooleanTypeLiteralSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceBooleanTypeLiteralSyntax);

        protected virtual SyntaxBase ReplaceNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.NullKeyword, out var nullKeyword);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NullTypeLiteralSyntax(nullKeyword);
        }
        void ISyntaxVisitor.VisitNullTypeLiteralSyntax(NullTypeLiteralSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceNullTypeLiteralSyntax);

        protected virtual SyntaxBase ReplaceUnaryTypeOperationSyntax(UnaryTypeOperationSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OperatorToken, out var operatorToken);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new UnaryTypeOperationSyntax(operatorToken, expression);
        }
        void ISyntaxVisitor.VisitUnaryTypeOperationSyntax(UnaryTypeOperationSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceUnaryTypeOperationSyntax);

        protected virtual SyntaxBase ReplaceNonNullableTypeSyntax(NonNullableTypeSyntax syntax)
        {
            var hasChanges = TryRewrite(syntax.Base, out var baseExpression);
            hasChanges |= TryRewriteStrict(syntax.NonNullabilityMarker, out var nonNullabilityMarker);

            if (!hasChanges)
            {
                return syntax;
            }

            return new NonNullableTypeSyntax(baseExpression, nonNullabilityMarker);
        }
        void ISyntaxVisitor.VisitNonNullableTypeSyntax(NonNullableTypeSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceNonNullableTypeSyntax);

        protected virtual SyntaxBase ReplaceParenthesizedTypeSyntax(ParenthesizedTypeSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.OpenParen, out var openParen);
            hasChanges |= TryRewrite(syntax.Expression, out var expression);
            hasChanges |= TryRewrite(syntax.CloseParen, out var closeParen);

            if (!hasChanges)
            {
                return syntax;
            }

            return new ParenthesizedTypeSyntax(openParen, expression, closeParen);
        }
        void ISyntaxVisitor.VisitParenthesizedTypeSyntax(ParenthesizedTypeSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceParenthesizedTypeSyntax);

        protected virtual SyntaxBase ReplaceSpreadExpressionSyntax(SpreadExpressionSyntax syntax)
        {
            var hasChanges = TryRewriteStrict(syntax.Ellipsis, out var ellipsis);
            hasChanges |= TryRewriteStrict(syntax.Expression, out var expression);

            if (!hasChanges)
            {
                return syntax;
            }

            return new SpreadExpressionSyntax(ellipsis, expression);
        }
        void ISyntaxVisitor.VisitSpreadExpressionSyntax(SpreadExpressionSyntax syntax)
            => ReplaceCurrent(syntax, ReplaceSpreadExpressionSyntax);
    }
}
