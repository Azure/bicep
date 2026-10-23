// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Analyzers.Linter.Rules;

public sealed class SecureStorageAccountRule : LinterRuleBase
{
    public new const string Code = "secure-storage-account";

    private const string StorageAccountResourceType = "Microsoft.Storage/storageAccounts";
    private const string AllowBlobPublicAccessProperty = "allowBlobPublicAccess";
    private const string SupportsHttpsTrafficOnlyProperty = "supportsHttpsTrafficOnly";
    private const string MinimumTlsVersionProperty = "minimumTlsVersion";

    public SecureStorageAccountRule()
        : base(
            code: Code,
            description: CoreResources.SecureStorageAccountRuleDescription,
            LinterRuleCategory.Security,
            // Existing storage-account declarations may intentionally rely on platform defaults; opt in before enforcing these recommendations.
            overrideCategoryDefaultDiagnosticLevel: DiagnosticLevel.Off)
    {
    }

    public override string FormatMessage(params object[] values)
        => string.Format(CoreResources.SecureStorageAccountRuleMessageFormat, values);

    public override IEnumerable<IDiagnostic> AnalyzeInternal(SemanticModel model, DiagnosticLevel diagnosticLevel)
    {
        List<IDiagnostic> diagnostics = [];

        foreach (var resource in model.DeclaredResources.Where(resource =>
                     resource.IsAzResource &&
                     !resource.IsExistingResource &&
                     LanguageConstants.ResourceTypeComparer.Equals(resource.TypeReference.FormatType(), StorageAccountResourceType)))
        {
            var resourceSyntax = resource.Symbol.DeclaringResource;
            var properties = resourceSyntax.TryGetBody()?.TryGetPropertyByName("properties")?.Value as ObjectSyntax;

            ValidateProperty(model, diagnosticLevel, diagnostics, resourceSyntax, properties, AllowBlobPublicAccessProperty, "false", IsTrue);
            ValidateProperty(model, diagnosticLevel, diagnostics, resourceSyntax, properties, SupportsHttpsTrafficOnlyProperty, "true", IsFalse);
            ValidateProperty(model, diagnosticLevel, diagnostics, resourceSyntax, properties, MinimumTlsVersionProperty, "'TLS1_2'", IsBelowTls12);
        }

        return diagnostics;
    }

    private void ValidateProperty(
        SemanticModel model,
        DiagnosticLevel diagnosticLevel,
        List<IDiagnostic> diagnostics,
        ResourceDeclarationSyntax resourceSyntax,
        ObjectSyntax? properties,
        string propertyName,
        string recommendedValue,
        Func<TypeSymbol, bool> isKnownUnsafe)
    {
        var property = properties?.TryGetPropertyByName(propertyName);
        if (property is null || isKnownUnsafe(model.GetTypeInfo(property.Value)))
        {
            diagnostics.Add(CreateDiagnosticForSpan(
                diagnosticLevel,
                property?.Value.Span ?? resourceSyntax.Type.Span,
                propertyName,
                recommendedValue));
        }
    }

    private static bool IsTrue(TypeSymbol type) => type is BooleanLiteralType { Value: true };

    private static bool IsFalse(TypeSymbol type) => type is BooleanLiteralType { Value: false };

    private static bool IsBelowTls12(TypeSymbol type) =>
        type is StringLiteralType { RawStringValue: var value } &&
        (value.Equals("TLS1_0", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("TLS1_1", StringComparison.OrdinalIgnoreCase));
}
