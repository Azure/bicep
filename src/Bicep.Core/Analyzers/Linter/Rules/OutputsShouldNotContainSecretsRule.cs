// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class OutputsShouldNotContainSecretsRule : LinterRuleBase
    {
        public new const string Code = "outputs-should-not-contain-secrets";

        public OutputsShouldNotContainSecretsRule() : base(
            code: Code,
            description: CoreResources.OutputsShouldNotContainSecretsRuleDescription,
            docUri: new Uri($"https://aka.ms/bicep/linter/{Code}")
        )
        {
        }

        public override string FormatMessage(params object[] values)
        {
            return string.Format(
                CoreResources.OutputsShouldNotContainSecretsMessageFormat,
                this.Description,
                values.First());
        }

        public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            var visitor = new OutputVisitor(this, model, diagnosticLevel);
            visitor.Visit(model.SourceFile.ProgramSyntax);
            return visitor.diagnostics;
        }

        private class OutputVisitor : AstVisitor
        {
            public List<IDiagnostic> diagnostics = new();

            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;

            public OutputVisitor(OutputsShouldNotContainSecretsRule parent, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitOutputDeclarationSyntax(OutputDeclarationSyntax syntax)
            {
                // Does the output name contain 'password' (suggesting it contains an actual password)?
                if (syntax.Name.IdentifierName.Contains("password", StringComparison.OrdinalIgnoreCase))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsOutputName, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                }

                var visitor = new OutputValueVisitor(this.parent, diagnostics, model, diagnosticLevel);
                visitor.Visit(syntax);

                // Note: No need to navigate deeper, don't call base
            }
        }

        private class OutputValueVisitor : AstVisitor
        {
            private readonly List<IDiagnostic> diagnostics;
            private readonly OutputsShouldNotContainSecretsRule parent;
            private readonly SemanticModel model;
            private readonly DiagnosticLevel diagnosticLevel;
            private readonly Stack<AccessExpressionSyntax> accessExpressionStack = new();

            public OutputValueVisitor(OutputsShouldNotContainSecretsRule parent, List<IDiagnostic> diagnostics, SemanticModel model, DiagnosticLevel diagnosticLevel)
            {
                this.parent = parent;
                this.model = model;
                this.diagnostics = diagnostics;
                this.diagnosticLevel = diagnosticLevel;
            }

            public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
            {
                // Look for references of secure values, e.g.:
                //
                //   @secure()
                //   param secureParam string
                //   output badResult string = 'this is the value ${secureParam}'

                foreach (var pathToSecureComponent in FindPathsToSecureComponents(model.GetTypeInfo(syntax)))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.Name.IdentifierName + pathToSecureComponent);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Name.Span, foundMessage));
                }

                base.VisitVariableAccessSyntax(syntax);
            }

            public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            {
                foreach (var pathToSecureComponent in FindPathsToSecureComponents(model.GetTypeInfo(syntax)))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.ToTextPreserveFormatting() + pathToSecureComponent);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.PropertyName.Span, foundMessage));
                }

                accessExpressionStack.Push(syntax);
                base.VisitPropertyAccessSyntax(syntax);
                accessExpressionStack.Pop();
            }

            public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
            {
                foreach (var pathToSecureComponent in FindPathsToSecureComponents(model.GetTypeInfo(syntax)))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.ToTextPreserveFormatting() + pathToSecureComponent);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.IndexExpression.Span, foundMessage));
                }

                accessExpressionStack.Push(syntax);
                base.VisitArrayAccessSyntax(syntax);
                accessExpressionStack.Pop();
            }

            private IEnumerable<string> FindPathsToSecureComponents(TypeSymbol type)
                => FindPathsToSecureComponents(type, "", ImmutableHashSet<TypeSymbol>.Empty);

            private IEnumerable<string> FindPathsToSecureComponents(TypeSymbol type, string path, ImmutableHashSet<TypeSymbol> visited)
            {
                // types can be recursive. cut out early if we've already seen this type
                if (visited.Contains(type))
                {
                    yield break;
                }

                if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                {
                    yield return path;
                    // if we encounter a type that has been explicitly flagged as secure, stop visiting its components
                    yield break;
                }

                if (type is UnionType union)
                {
                    foreach (var variantPath in union.Members.SelectMany(m => FindPathsToSecureComponents(m.Type, path, visited.Add(type))))
                    {
                        yield return variantPath;
                    }
                }

                // if the expression being visited is dereferencing a specific property or index of this type, we shouldn't warn if the type under inspection
                // *contains* properties or indices that are flagged as secure. We will have already warned if those have been accessed in the expression, and
                // if they haven't, then the value dereferenced isn't sensitive
                if (accessExpressionStack.Count == 0)
                {
                    switch (type)
                    {
                        case ObjectType obj:
                            if (obj.AdditionalPropertiesType?.Type is TypeSymbol addlPropsType)
                            {
                                foreach (var dictMemberPath in FindPathsToSecureComponents(addlPropsType, $"{path}.*", visited.Add(type)))
                                {
                                    yield return dictMemberPath;
                                }
                            }

                            foreach (var propertyPath in obj.Properties.SelectMany(p => FindPathsToSecureComponents(p.Value.TypeReference.Type, $"{path}.{p.Key}", visited.Add(type))))
                            {
                                yield return propertyPath;
                            }
                            break;
                        case TupleType tuple:
                            foreach (var pathFromIndex in tuple.Items.SelectMany((ITypeReference typeAtIndex, int index) => FindPathsToSecureComponents(typeAtIndex.Type, $"{path}[{index}]", visited.Add(type))))
                            {
                                yield return pathFromIndex;
                            }
                            break;
                        case ArrayType array:
                            foreach (var pathFromElement in FindPathsToSecureComponents(array.Item.Type, $"{path}[*]", visited.Add(type)))
                            {
                                yield return pathFromElement;
                            }
                            break;
                    }
                }
            }

            public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
            {
                // Look for usage of list*() stand-alone function, e.g.:
                //
                //   output badResult object = listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
                //

                if (SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, syntax) is FunctionCallSyntaxBase listFunction
                    && listFunction.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsFunction, syntax.Name.IdentifierName);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                }

                base.VisitFunctionCallSyntax(syntax);
            }

            public override void VisitInstanceFunctionCallSyntax(InstanceFunctionCallSyntax syntax)
            {
                if (syntax.Name.IdentifierName.StartsWithOrdinalInsensitively(LanguageConstants.ListFunctionPrefix))
                {
                    bool isFailure = false;

                    if (model.ResourceMetadata.TryLookup(syntax.BaseExpression) is { })
                    {
                        // It's a usage of a list*() member function for a resource value, e.g.:
                        //
                        //   output badResult object = stg.listKeys().keys[0].value
                        //
                        isFailure = true;
                    }
                    else if (SemanticModelHelper.TryGetFunctionInNamespace(model, AzNamespaceType.BuiltInName, syntax) is FunctionCallSyntaxBase listFunction)
                    {
                        // It's a usage of a built-in list*() function as a member of the built-in "az" module, e.g.:
                        //
                        //   output badResult object = az.listKeys(resourceId('Microsoft.Storage/storageAccounts', 'storageName'), '2021-02-01')
                        //
                        isFailure = true;
                    }

                    if (isFailure)
                    {
                        string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsFunction, syntax.Name.IdentifierName);
                        this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Span, foundMessage));
                    }
                }

                base.VisitInstanceFunctionCallSyntax(syntax);
            }
        }
    }
}
