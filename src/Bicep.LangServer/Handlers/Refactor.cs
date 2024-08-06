// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Bicep.Core;
using Bicep.Core.CodeAction;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Refactor;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using static Bicep.LanguageServer.Completions.BicepCompletionContext;
using static Google.Protobuf.Reflection.ExtensionRangeOptions.Types;

namespace Bicep.LanguageServer.Handlers
{
    // asdfg Convert var to param

    // Provides code actions/fixes for a range in a Bicep document
    public static class Refactor
    {
        private const int MaxExpressionLengthInAction = 100;

        static string NewLine(SemanticModel semanticModel) => semanticModel.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();

        public static IEnumerable<CodeFix> GetExtractionRefactorings(
            CompilationContext compilationContext,
            Compilation compilation,
            SemanticModel semanticModel,
            List<SyntaxBase> nodesInRange)
        {
            if (SyntaxMatcher.FindLastNodeOfType<ExpressionSyntax, ExpressionSyntax>(nodesInRange) is not (ExpressionSyntax expressionSyntax, _))
            {
                yield break;
            }

            //asdfg            PrintAllTypes(semanticModel);

            string? defaultNewName = null;

            // Semi-intelligent default names for new variable
            if (semanticModel.Binder.GetParent(expressionSyntax) is ObjectPropertySyntax propertySyntax
                && propertySyntax.TryGetKeyText() is string propertyName)
            {
                // `{ objectPropertyName: <<expression>> }` // entire property value expression selected
                //   -> default to the name "objectPropertyName"
                defaultNewName = propertyName;
            }
            else if (expressionSyntax is ObjectPropertySyntax propertySyntax2
                && propertySyntax2.TryGetKeyText() is string propertyName2)
            {
                // `{ <<objectPropertyName>>: expression }` // property itself is selected
                //   -> default to the name "objectPropertyName"
                defaultNewName = propertyName2;

                // The expression we want to replace is the property value, not the property syntax
                var propertyValueSyntax = propertySyntax2.Value as ExpressionSyntax;
                if (propertyValueSyntax != null)
                {
                    expressionSyntax = propertyValueSyntax;
                }
                else
                {
                    yield break;
                }
            }
            else if (expressionSyntax is PropertyAccessSyntax propertyAccessSyntax)
            {
                // `object.topPropertyName.propertyName`
                //   -> default to the name "topPropertyNamePropertyName"
                //
                // `object.topPropertyName.propertyName`
                //   -> default to the name "propertyName"
                //  
                // More than two levels is less likely to be desirable

                string lastPartName = propertyAccessSyntax.PropertyName.IdentifierName;
                var parent = propertyAccessSyntax.BaseExpression;
                string? firstPartName = parent switch
                {
                    PropertyAccessSyntax propertyAccess => propertyAccess.PropertyName.IdentifierName,
                    VariableAccessSyntax variableAccess => variableAccess.Name.IdentifierName,
                    FunctionCallSyntax functionCall => functionCall.Name.IdentifierName,
                    _ => null
                };

                defaultNewName = firstPartName is { } ? firstPartName + lastPartName.UppercaseFirstLetter() : lastPartName;
            }

            if (semanticModel.Binder.GetNearestAncestor<StatementSyntax>(expressionSyntax) is not StatementSyntax statementSyntax)
            {
                yield break;
            }

            var newVarName = FindUnusedName(compilation, expressionSyntax.Span.Position, defaultNewName ?? "newVariable");
            var newParamName = FindUnusedName(compilation, expressionSyntax.Span.Position, defaultNewName ?? "newParameter");

            var varDeclarationSyntax = SyntaxFactory.CreateVariableDeclaration(newVarName, expressionSyntax);
            var varDeclaration = PrettyPrinterV2.PrintValid(varDeclarationSyntax, PrettyPrinterV2Options.Default) + NewLine(semanticModel);

            var statementLineNumber = TextCoordinateConverter.GetPosition(compilationContext.LineStarts, statementSyntax.Span.Position).line;
            var definitionInsertionPosition = TextCoordinateConverter.GetOffset(compilationContext.LineStarts, statementLineNumber, 0);

            yield return new CodeFix(
               $"Create variable for {GetQuotedExpressionText(expressionSyntax)}",
               isPreferred: false,
               CodeFixKind.RefactorExtract,
               new CodeReplacement(expressionSyntax.Span, newVarName),
               new CodeReplacement(new TextSpan(definitionInsertionPosition, 0), varDeclaration));


            // For the new param's type, try to use the declared type if there is one (i.e. the type of
            //   what we're assigning to), otherwise use the actual calculated type of the expression
            var inferredType = semanticModel.GetTypeInfo(expressionSyntax);
            var declaredType = semanticModel.GetDeclaredType(expressionSyntax);
            var newParamType = NullIfErrorOrAnyType(declaredType) ?? NullIfErrorOrAnyType(inferredType);

            yield return CreateExtractParameterCodeFix(
                semanticModel, newParamType, newParamName, definitionInsertionPosition, expressionSyntax, SyntaxForType.Strictness.Medium);
            yield return CreateExtractParameterCodeFix(
                semanticModel, newParamType, newParamName, definitionInsertionPosition, expressionSyntax, SyntaxForType.Strictness.Loose);
            yield return CreateExtractParameterCodeFix(
                semanticModel, newParamType, newParamName, definitionInsertionPosition, expressionSyntax, SyntaxForType.Strictness.Strict);
        }

        private static CodeFix CreateExtractParameterCodeFix(
            SemanticModel semanticModel,
            TypeSymbol? newParamType,
            string newParamName,
            int definitionInsertionPosition,
            ExpressionSyntax expressionSyntax,
            SyntaxForType.Strictness strictness)
        {
            var declaration = CreateNewParameterDeclaration(semanticModel, newParamType, newParamName, expressionSyntax, strictness);

            return new CodeFix(
                $"Create parameter ({Enum.GetName<SyntaxForType.Strictness>(strictness)} for {GetQuotedExpressionText(expressionSyntax)}",
                isPreferred: false,
                CodeFixKind.RefactorExtract,
                new CodeReplacement(expressionSyntax.Span, newParamName),
                new CodeReplacement(new TextSpan(definitionInsertionPosition, 0), declaration));
        }

        private static string CreateNewParameterDeclaration(
            SemanticModel semanticModel,
            TypeSymbol? newParamType,
            string newParamName,
            SyntaxBase defaultValueSyntax,
            SyntaxForType.Strictness strictness)
        {
            var expressionTypeName = SyntaxForType.GetSyntaxForType(newParamType, strictness);
            Trace.WriteLine($"{Enum.GetName<SyntaxForType.Strictness>(strictness)}: {expressionTypeName}"); //asdfg

            //asdfg use syntax nodes properly
            var expressionTypeIdentifier = SyntaxFactory.CreateIdentifierWithTrailingSpace(expressionTypeName);

            var paramDeclarationSyntax = SyntaxFactory.CreateParameterDeclaration(
                newParamName,
                new TypeVariableAccessSyntax(expressionTypeIdentifier),
                defaultValueSyntax);
            var paramDeclaration = PrettyPrinterV2.PrintValid(paramDeclarationSyntax, PrettyPrinterV2Options.Default) + NewLine(semanticModel);
            return paramDeclaration;
        }

        private static TypeSymbol? NullIfErrorOrAnyType(TypeSymbol? type) => type is ErrorType || type is AnyType ? null : type;

        //asdfg
        //private static TypeSymbol? GetLooseTypeString(TypeSymbol? type) => type switch
        //{
        //    StringLiteralType => LanguageConstants.String,
        //    IntegerLiteralType => LanguageConstants.Int,
        //    BooleanLiteralType => LanguageConstants.Bool,
        //    BooleanType => LanguageConstants.Bool,
        //    IntegerType => LanguageConstants.Int,
        //    StringType => LanguageConstants.String,
        //    // If it's a custom object like "{ i: int, o: { i2: int } }", keep it that way.
        //    // Otherwise, e.g. for resource types (for now) or external types like "VirtualMachineExtensionProperties"
        //    //   that aren't recognized in Bicep code, change to Object
        //    ObjectType when !type.Name.StartsWith('{') => LanguageConstants.Object,
        //    TupleType => LanguageConstants.Array,
        //    NullType => LanguageConstants.Null,
        //    _ => null,
        //};


        private static string FindUnusedName(Compilation compilation, int offset, string preferredName) //asdfg
        {
            var activeScopes = ActiveScopesVisitor.GetActiveScopes(compilation.GetEntrypointSemanticModel().Root, offset);
            for (int i = 1; i < int.MaxValue; ++i)
            {
                var tryingName = $"{preferredName}{(i < 2 ? "" : i)}";
                if (!activeScopes.Any(s => s.GetDeclarationsByName(tryingName).Any()))
                {
                    preferredName = tryingName;
                    break;
                }
            }

            return preferredName;
        }

        private static string GetQuotedExpressionText(ExpressionSyntax expressionSyntax)
        {
            return "\""
                + SyntaxStringifier.Stringify(expressionSyntax, newlineReplacement: " ")
                    .TruncateWithEllipses(MaxExpressionLengthInAction)
                    .Trim()
                + "\"";
        }

        //asdfg: remove
        //private static void PrintAllTypes(SemanticModel semanticModel)
        //{
        //    var asdfg = SyntaxCollectorVisitor.Build(semanticModel.Root.Syntax);
        //    foreach (var node1 in asdfg.Where(s => s.Syntax is not Token))
        //    {
        //        //asdfg
        //        var node = node1.Syntax;
        //        Trace.WriteLine($"** {node.GetDebuggerDisplay().ReplaceNewlines(" ").TruncateWithEllipses(150)}");
        //        Trace.WriteLine($"  ... type info: {semanticModel.GetTypeInfo(node).Name}");
        //        Trace.WriteLine($"  ... declared type: {semanticModel.GetDeclaredType(node)?.Name}");
        //    }
        //}

        //asdfg: remove
        public class SyntaxCollectorVisitor : CstVisitor
        {
            public record SyntaxItem(SyntaxBase Syntax, SyntaxItem? Parent, int Depth)
            {
                public IEnumerable<SyntaxCollectorVisitor.SyntaxItem> GetAncestors()
                {
                    var data = this;
                    while (data.Parent is { } parent)
                    {
                        yield return parent;
                        data = parent;
                    }
                }
            }

            private readonly IList<SyntaxItem> syntaxList = new List<SyntaxItem>();
            private SyntaxItem? parent = null;
            private int depth = 0;

            private SyntaxCollectorVisitor()
            {
            }

            public static ImmutableArray<SyntaxItem> Build(SyntaxBase syntax)
            {
                var visitor = new SyntaxCollectorVisitor();
                visitor.Visit(syntax);

                return [.. visitor.syntaxList];
            }

            protected override void VisitInternal(SyntaxBase syntax)
            {
                var syntaxItem = new SyntaxItem(Syntax: syntax, Parent: parent, Depth: depth);
                syntaxList.Add(syntaxItem);

                var prevParent = parent;
                parent = syntaxItem;
                depth++;
                base.VisitInternal(syntax);
                depth--;
                parent = prevParent;
            }
        }
    }
}
