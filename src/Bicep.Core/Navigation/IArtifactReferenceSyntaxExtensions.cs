// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Navigation;

public static class IArtifactReferenceSyntaxExtensions
{
    public static StringSyntax? TryGetPath(this IArtifactReferenceSyntax syntax)
        => syntax.Path as StringSyntax;

    public static Result<ISemanticModel, ErrorDiagnostic> TryGetReferencedModel(
        this IArtifactReferenceSyntax reference,
        IArtifactFileLookup sourceFileLookup,
        ISemanticModelLookup semanticModelLookup)
    {
        return TryGetSourceFile(sourceFileLookup, reference).Transform(semanticModelLookup.GetSemanticModel);
    }

    public static Result<ISemanticModel, ErrorDiagnostic> TryGetReferencedModel(
        this IArtifactReferenceSyntax reference,
        IArtifactFileLookup sourceFileLookup,
        ISemanticModelLookup semanticModelLookup,
        DiagnosticBuilder.ErrorBuilderDelegate onInvalidSourceFileType)
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
