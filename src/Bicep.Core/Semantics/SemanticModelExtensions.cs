// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics;

public static class SemanticModelExtensions
{
    public static Result<ISemanticModel, ErrorDiagnostic> TryLookupModel(this SemanticModel model, IArtifactReferenceSyntax referenceSyntax, DiagnosticBuilder.ErrorBuilderDelegate onInvalidSourceFileType)
        => TryGetModelForArtifactReference(
            model.SourceFileGrouping,
            referenceSyntax,
            onInvalidSourceFileType,
            model.ModelLookup);

    public static Result<ISemanticModel, ErrorDiagnostic> TryLookupModel(this SemanticModel model, IArtifactReferenceSyntax referenceSyntax)
        => TryGetModelForArtifactReference(
            model.SourceFileGrouping,
            referenceSyntax,
            model.ModelLookup);

    private static Result<ISemanticModel, ErrorDiagnostic> TryGetModelForArtifactReference(
        IArtifactFileLookup sourceFileLookup,
        IArtifactReferenceSyntax reference,
        ISemanticModelLookup semanticModelLookup)
    {
        return TryGetSourceFile(sourceFileLookup, reference).Transform(semanticModelLookup.GetSemanticModel);
    }

    private static Result<ISemanticModel, ErrorDiagnostic> TryGetModelForArtifactReference(
        IArtifactFileLookup sourceFileLookup,
        IArtifactReferenceSyntax reference,
        DiagnosticBuilder.ErrorBuilderDelegate onInvalidSourceFileType,
        ISemanticModelLookup semanticModelLookup)
    {
        if (!TryGetSourceFile(sourceFileLookup, reference).IsSuccess(out var sourceFile, out var error))
        {
            return new(error);
        }

        // when we inevitably add a third language ID,
        // the inclusion list style below will prevent the new language ID from being
        // automatically allowed to be referenced via module declarations
        var isValidReference = (reference, sourceFile) switch
        {
            (ExtendsDeclarationSyntax, BicepParamFile) => true,
            (_, BicepFile or ArmTemplateFile or TemplateSpecFile) => true,
            _ => false,
        };
        if (!isValidReference)
        {
            return new(onInvalidSourceFileType(DiagnosticBuilder.ForPosition(reference.SourceSyntax)));
        }

        return new(semanticModelLookup.GetSemanticModel(sourceFile));
    }

    private static Result<ISourceFile, ErrorDiagnostic> TryGetSourceFile(IArtifactFileLookup sourceFileLookup, IArtifactReferenceSyntax reference)
    {
        if (!sourceFileLookup.TryGetSourceFile(reference).IsSuccess(out var sourceFile, out var errorBuilder))
        {
            return new(errorBuilder(DiagnosticBuilder.ForPosition(reference.SourceSyntax)));
        }

        return new(sourceFile);
    }
}