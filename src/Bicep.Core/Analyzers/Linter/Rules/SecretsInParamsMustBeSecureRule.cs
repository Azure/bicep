// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.CodeAction;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules
{
    public sealed class SecretsInParamsMustBeSecureRule : LinterRuleBase
    {
        public new const string Code = "secure-secrets-in-params";
        public const int MaxNumber = 256;

        private static readonly Regex HasSecretRegex = new("password|pwd|secret|accountkey|acctkey", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Allow certain patterns we know about in ARM
        private static readonly Regex AllowedRegex = new(
            // secret + Permissions (keyVault secret perms is an accessPolicy property)
            "secretpermissions"
            // secret + version (url or simply the version property of a secret)
            + "|secretversion"
            // secret + url/uri
            + "|secretur[il]"
            // secret + name
            + "|secretname",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public SecretsInParamsMustBeSecureRule() : base(
            code: Code,
            description: CoreResources.SecretsInParamsRule_Description,
            LinterRuleCategory.Security)
        { }

        public override string FormatMessage(params object[] values)
        {
            string paramName = (string)values[0];
            return string.Format(CoreResources.SecretsInParamsRule_MessageFormat, paramName);
        }

        override public IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
        {
            foreach (var param in model.Root.ParameterDeclarations)
            {
                if (!param.IsSecure() && !DeclaredTypeReferenceIsSecure(model, param))
                {
                    if (AnalyzeUnsecuredParameter(model, diagnosticLevel, param) is IDiagnostic diag)
                    {
                        yield return diag;
                    }
                }
            }
        }

        private IDiagnostic? AnalyzeUnsecuredParameter(SemanticModel model, DiagnosticLevel diagnosticLevel, ParameterSymbol parameterSymbol)
        {
            string name = parameterSymbol.Name;
            TypeSymbol type = parameterSymbol.Type;
            if (type.IsObject() || type.IsString())
            {
                if (HasSecretRegex.IsMatch(name))
                {
                    if (!AllowedRegex.IsMatch(name))
                    {
                        return CreateDiagnostic(model, parameterSymbol, diagnosticLevel);
                    }
                }
            }

            foreach (var referencedSymbol in model.Binder.GetSymbolsReferencedInDeclarationOf(parameterSymbol))
            {
                if (referencedSymbol is ParameterSymbol referencedParameter && referencedParameter.IsSecure())
                {
                    // The default vlaue has a reference to a parameter marked as secure
                    return CreateDiagnostic(model, parameterSymbol, diagnosticLevel);
                }
            }

            return null;
        }

        private static bool DeclaredTypeReferenceIsSecure(SemanticModel model, ParameterSymbol parameterSymbol)
            => RefersToTypeAlias(parameterSymbol.DeclaringParameter.Type, model) &&
                model.GetDeclaredType(parameterSymbol.DeclaringParameter.Type) is { } declaredType &&
                DeclaredTypeIsSecure(declaredType);

        private static bool DeclaredTypeIsSecure(TypeSymbol type)
        {
            var nonNullable = TypeHelper.TryRemoveNullability(type) ?? type;

            return nonNullable.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure);
        }

        private static bool RefersToTypeAlias(SyntaxBase? typeSyntax, SemanticModel model) => UnwrapNullableSyntax(typeSyntax) switch
        {
            TypeVariableAccessSyntax variableAccess
                => model.Binder.GetSymbolInfo(variableAccess) is TypeAliasSymbol or ImportedTypeSymbol or WildcardImportSymbol,
            TypePropertyAccessSyntax typePropertyAccess => RefersToTypeAlias(typePropertyAccess.BaseExpression, model),
            TypeAdditionalPropertiesAccessSyntax typeAdditionalPropertiesAccess => RefersToTypeAlias(typeAdditionalPropertiesAccess.BaseExpression, model),
            TypeArrayAccessSyntax typeArrayAccess
                => RefersToTypeAlias(typeArrayAccess.BaseExpression, model) || RefersToTypeAlias(typeArrayAccess.IndexExpression, model),
            TypeItemsAccessSyntax typeItemsAccess => RefersToTypeAlias(typeItemsAccess.BaseExpression, model),
            _ => false,
        };

        private static SyntaxBase? UnwrapNullableSyntax(SyntaxBase? maybeNullable) => maybeNullable switch
        {
            NullableTypeSyntax nullable => nullable.Base,
            var otherwise => otherwise,
        };

        private static TypeAliasSymbol? TryGetDirectLocalTypeAlias(SyntaxBase typeSyntax, SemanticModel model) => UnwrapNullableSyntax(typeSyntax) switch
        {
            TypeVariableAccessSyntax variableAccess => model.Binder.GetSymbolInfo(variableAccess) as TypeAliasSymbol,
            _ => null,
        };

        private static bool TypeCanBeMarkedSecure(TypeSymbol type)
        {
            var unwrapped = type is TypeType typeType ? typeType.Unwrapped : type;
            var nonNullable = TypeHelper.TryRemoveNullability(unwrapped) ?? unwrapped;

            return nonNullable.IsObject() || nonNullable.IsString();
        }

        private IDiagnostic CreateDiagnostic(SemanticModel model, ParameterSymbol parameterSymbol, DiagnosticLevel diagnosticLevel)
        {
            var fixableDiagnostic = TryCreateCodeFix(model, parameterSymbol) is { } codeFix
                ? CreateFixableDiagnosticForSpan(diagnosticLevel, parameterSymbol.NameSource.Span, codeFix, parameterSymbol.Name)
                : null;

            return fixableDiagnostic ?? CreateDiagnosticForSpan(diagnosticLevel, parameterSymbol.NameSource.Span, parameterSymbol.Name);
        }

        private static CodeFix? TryCreateCodeFix(SemanticModel model, ParameterSymbol parameterSymbol)
        {
            var decorator = SyntaxFactory.CreateDecorator("secure");
            var newline = model.Configuration.Formatting.Data.NewlineKind.ToEscapeSequence();
            var decoratorText = $"{decorator}{newline}";

            if (TryGetDirectLocalTypeAlias(parameterSymbol.DeclaringParameter.Type, model) is { } typeAliasSymbol &&
                TypeCanBeMarkedSecure(typeAliasSymbol.Type))
            {
                var fixSpan = new TextSpan(typeAliasSymbol.DeclaringSyntax.Span.Position, 0);
                var codeReplacement = new CodeReplacement(fixSpan, decoratorText);

                return new CodeFix("Mark type as secure", isPreferred: true, CodeFixKind.QuickFix, codeReplacement);
            }

            if (!RefersToTypeAlias(parameterSymbol.DeclaringParameter.Type, model))
            {
                var fixSpan = new TextSpan(parameterSymbol.DeclaringSyntax.Span.Position, 0);
                var codeReplacement = new CodeReplacement(fixSpan, decoratorText);

                return new CodeFix("Mark parameter as secure", isPreferred: true, CodeFixKind.QuickFix, codeReplacement);
            }

            return null;
        }
    }
}
