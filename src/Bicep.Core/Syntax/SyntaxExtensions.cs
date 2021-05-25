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
    }
}

