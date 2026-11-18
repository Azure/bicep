// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Resolution;
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
                string pathPrefix;
                string path;
                switch (ArtifactAddress.Parse(referenceText))
                {
                    case ArtifactAddress.Path:
                        pathPrefix = "";
                        path = referenceText;
                        break;
                    case ArtifactAddress.SchemeQualified { Scheme: ArtifactSchemes.File, Body: var body } when referencingFile.LoadFeatures().ArtifactAliasesAndRedirectsEnabled:
                        if (!RelativePath.TryCreate(body).IsSuccess(out _, out var pathError))
                        {
                            return new(pathError);
                        }

                        pathPrefix = $"{ArtifactSchemes.File}:";
                        path = body;
                        break;
                    default:
                        // A reference to a remote artifact.
                        return new(referenceText);
                }

                if (config.ConfigFileUri is null)
                {
                    throw new InvalidOperationException("The configuration file URI must be set when trying to resolve an extension reference.");
                }

                // A path, bare or with "file:", is relative to the configuration file; rewrite it relative to the referencing file.
                var extensionUri = config.ConfigFileUri.Resolve(path);

                return new(pathPrefix + extensionUri.GetPathRelativeTo(referencingFile.FileHandle.Uri));
            default:
                return new(x => x.ExpectedExtensionSpecification());
        }
    }
}
