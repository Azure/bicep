// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

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

        public static bool NameEquals(this FunctionCallSyntax funcSyntax, string compareTo)
            => LanguageConstants.IdentifierComparer.Equals(funcSyntax.Name.IdentifierName, compareTo);

        public static bool NameEquals(this IdentifierSyntax identifier, string compareTo)
            => LanguageConstants.IdentifierComparer.Equals(identifier.IdentifierName, compareTo);

        private static TypeProperty? TryGetTypeProperty(SemanticModel model, SyntaxBase objectSyntax, string propertyName)
        {
            // Cannot use assigned type here because it won't handle the case where the property value
            // is an array accesss or a string interpolation.
            return model.TypeManager.GetDeclaredType(objectSyntax) switch
            {
                ObjectType { Properties: var properties }
                    when properties.TryGetValue(propertyName, out var typeProperty) => typeProperty,
                DiscriminatedObjectType { DiscriminatorKey: var discriminatorKey, DiscriminatorProperty: var typeProperty }
                    when LanguageConstants.IdentifierComparer.Equals(propertyName, discriminatorKey) => typeProperty,
                _ => null,
            };
        }

        public static TypeProperty? TryGetTypeProperty(this ObjectPropertySyntax syntax, SemanticModel model)
        {
            if (syntax.TryGetKeyText() is not string propertyName || model.Binder.GetParent(syntax) is not ObjectSyntax objectSyntax)
            {
                return null;
            }

            return TryGetTypeProperty(model, objectSyntax, propertyName);
        }
    }
}

