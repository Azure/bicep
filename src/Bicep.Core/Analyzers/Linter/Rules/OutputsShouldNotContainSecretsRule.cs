// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
            private uint trailingAccessExpressions = 0;

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

                foreach (var pathToSecureComponent in FindPathsToSecureTypeComponents(model.GetTypeInfo(syntax)))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.Name.IdentifierName + pathToSecureComponent);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.Name.Span, foundMessage));
                }

                base.VisitVariableAccessSyntax(syntax);
            }

            public override void VisitPropertyAccessSyntax(PropertyAccessSyntax syntax)
            {
                foreach (var pathToSecureComponent in FindPathsToSecureTypeComponents(model.GetTypeInfo(syntax)))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.ToTextPreserveFormatting() + pathToSecureComponent);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.PropertyName.Span, foundMessage));
                }

                trailingAccessExpressions++;
                base.VisitPropertyAccessSyntax(syntax);
                trailingAccessExpressions--;
            }

            public override void VisitArrayAccessSyntax(ArrayAccessSyntax syntax)
            {
                foreach (var pathToSecureComponent in FindPathsToSecureTypeComponents(model.GetTypeInfo(syntax)))
                {
                    string foundMessage = string.Format(CoreResources.OutputsShouldNotContainSecretsSecureParam, syntax.ToTextPreserveFormatting() + pathToSecureComponent);
                    this.diagnostics.Add(parent.CreateDiagnosticForSpan(diagnosticLevel, syntax.IndexExpression.Span, foundMessage));
                }

                trailingAccessExpressions++;
                base.VisitArrayAccessSyntax(syntax);
                trailingAccessExpressions--;
            }

            private IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type) => FindPathsToSecureTypeComponents(type, "", new());

            private IEnumerable<string> FindPathsToSecureTypeComponents(TypeSymbol type, string path, HashSet<TypeSymbol> visited)
            {
                // types can be recursive. cut out early if we've already seen this type
                if (visited.Contains(type))
                {
                    yield break;
                }

                visited.Add(type);

                if (type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure))
                {
                    yield return path;
                }

                if (type is UnionType union)
                {
                    foreach (var variantPath in union.Members.SelectMany(m => FindPathsToSecureTypeComponents(m.Type, path, visited)))
                    {
                        yield return variantPath;
                    }
                }

                // if the expression being visited is dereferencing a specific property or index of this type, we shouldn't warn if the type under inspection
                // *contains* properties or indices that are flagged as secure. We will have already warned if those have been accessed in the expression, and
                // if they haven't, then the value dereferenced isn't sensitive
                //
                //    param p {
                //      prop: {
                //        @secure()
                //        nestedSecret: string
                //        nestedInnocuousProperty: string
                //      }
                //    }
                //
                //    output objectContainingSecrets object = p                     // <-- should be flagged
                //    output propertyContainingSecrets object = p.prop              // <-- should be flagged
                //    output nestedSecret string = p.prop.nestedSecret              // <-- should be flagged
                //    output siblingOfSecret string = p.prop.nestedInnocuousData    // <-- should NOT be flagged
                if (trailingAccessExpressions == 0)
                {
                    switch (type)
                    {
                        case ObjectType obj:
                            if (obj.AdditionalPropertiesType?.Type is TypeSymbol addlPropsType)
                            {
                                foreach (var dictMemberPath in FindPathsToSecureTypeComponents(addlPropsType, $"{path}.*", visited))
                                {
                                    yield return dictMemberPath;
                                }
                            }

                            foreach (var propertyPath in obj.Properties.SelectMany(p => FindPathsToSecureTypeComponents(p.Value.TypeReference.Type, $"{path}.{p.Key}", visited)))
                            {
                                yield return propertyPath;
                            }
                            break;
                        case TupleType tuple:
                            foreach (var pathFromIndex in tuple.Items.SelectMany((ITypeReference typeAtIndex, int index) => FindPathsToSecureTypeComponents(typeAtIndex.Type, $"{path}[{index}]", visited)))
                            {
                                yield return pathFromIndex;
                            }
                            break;
                        case ArrayType array:
                            foreach (var pathFromElement in FindPathsToSecureTypeComponents(array.Item.Type, $"{path}[*]", visited))
                            {
                                yield return pathFromElement;
                            }
                            break;
                    }
                }

                visited.Remove(type);
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
