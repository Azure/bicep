// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;

namespace Bicep.Core.SourceGraph;

/// <summary>
/// Extracts the artifact reference text from a <c>module</c>, <c>import</c>, <c>using</c>, <c>extends</c>, <c>test</c>
/// or <c>extension</c> declaration, including the <c>extensions</c> configuration lookup for <c>extension &lt;name&gt;</c>.
/// </summary>
public static class ArtifactReferenceText
{
    public static ResultWithDiagnosticBuilder<string> TryGet(BicepSourceFile referencingFile, IArtifactReferenceSyntax artifactReferenceSyntax)
    {
        if (artifactReferenceSyntax is ExtensionDeclarationSyntax extensionSyntax)
        {
            return TryGetExtensionReferenceText(referencingFile, extensionSyntax);
        }

        return SyntaxHelper.TryGetForeignTemplatePath(artifactReferenceSyntax, GetPathNotSpecifiedErrorBuilder(artifactReferenceSyntax));
    }

    private static DiagnosticBuilder.DiagnosticBuilderDelegate GetPathNotSpecifiedErrorBuilder(IArtifactReferenceSyntax artifactReferenceSyntax) => artifactReferenceSyntax switch
    {
        UsingDeclarationSyntax => x => x.UsingPathHasNotBeenSpecified(),
        ExtendsDeclarationSyntax => x => x.ExtendsPathHasNotBeenSpecified(),
        CompileTimeImportDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
        ModuleDeclarationSyntax => x => x.ModulePathHasNotBeenSpecified(),
        StackDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
        TestDeclarationSyntax => x => x.PathHasNotBeenSpecified(),
        _ => throw new NotImplementedException($"Unexpected artifact reference syntax type '{artifactReferenceSyntax.GetType().Name}'."),
    };

    private static ResultWithDiagnosticBuilder<string> TryGetExtensionReferenceText(BicepSourceFile referencingFile, ExtensionDeclarationSyntax extensionSyntax)
    {
        switch (extensionSyntax.SpecificationString)
        {
            case StringSyntax inlinedSpec:
                if (inlinedSpec.TryGetLiteralValue() is not string pathValue)
                {
                    return new(x => x.ExtensionSpecificationInterpolationUnsupported());
                }

                return new(pathValue);
            case IdentifierSyntax configSpec:
                var config = referencingFile.LoadConfiguration();
                if (!config.Extensions.TryGetExtensionSource(configSpec.IdentifierName).IsSuccess(out var extensionEntry, out var error))
                {
                    return new(error);
                }

                var referenceText = extensionEntry.Value;
                if (referenceText.Contains(':'))
                {
                    // The extension path contains a scheme.
                    return new(referenceText);
                }

                if (config.ConfigFileUri is null)
                {
                    throw new InvalidOperationException("The configuration file URI must be set when trying to resolve an extension reference.");
                }

                // A bare path is relative to the configuration file; rewrite it relative to the referencing file.
                var extensionUri = config.ConfigFileUri.Resolve(referenceText);

                return new(extensionUri.GetPathRelativeTo(referencingFile.FileHandle.Uri));
            default:
                return new(x => x.ExpectedExtensionSpecification());
        }
    }
}
