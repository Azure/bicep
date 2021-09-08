// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;

namespace Bicep.Core.Syntax
{
    public static class SyntaxExtensions
    {
        public static IReadOnlyList<IDiagnostic> GetParseDiagnostics(this SyntaxBase syntax)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var parseErrorVisitor = new ParseDiagnosticsVisitor(diagnosticWriter);
            parseErrorVisitor.Visit(syntax);

            return diagnosticWriter.GetDiagnostics();
        }

        public static bool HasParseErrors(this SyntaxBase syntax)
            => syntax.GetParseDiagnostics().Any(d => d.Level == DiagnosticLevel.Error);

        public static bool ReferencesResource(this VariableAccessSyntax syntax, ResourceDeclarationSyntax resource)
            => LanguageConstants.IdentifierComparer.Equals(syntax.Name.IdentifierName, resource.Name.IdentifierName);

        public static bool NameEquals(this FunctionCallSyntax funcSyntax, string compareTo)
            => LanguageConstants.IdentifierComparer.Equals(funcSyntax.Name.IdentifierName, compareTo);

        public static DecoratorSyntax? TryGetDecoratorSyntax(this StatementSyntax statement, string decoratorName, string @namespace)
            => statement.Decorators
                .Where(d => d.Expression is FunctionCallSyntax function && function.NameEquals(decoratorName)
                    || (d.Expression is InstanceFunctionCallSyntax { BaseExpression: VariableAccessSyntax namespaceSyntax, Name: IdentifierSyntax nameSyntax } 
                        && namespaceSyntax.Name.IdentifierName == @namespace
                        && LanguageConstants.IdentifierComparer.Equals(nameSyntax.IdentifierName, decoratorName)))
                .FirstOrDefault();
    }
}

