// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Diagnostics;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.Navigation;

public static class IArtifactReferenceSyntaxExtensions
{
    public static StringSyntax? TryGetPath(this IArtifactReferenceSyntax syntax)
        => syntax.Path as StringSyntax;

    public static ResultWithDiagnostic<ISemanticModel> TryGetReferencedModel(
        this IArtifactReferenceSyntax reference,
        IArtifactFileLookup sourceFileLookup,
        ISemanticModelLookup semanticModelLookup)
    {
        return TryGetSourceFile(sourceFileLookup, reference).Transform(semanticModelLookup.GetSemanticModel);
    }

    public static ResultWithDiagnostic<ISemanticModel> TryGetReferencedModel(
        this IArtifactReferenceSyntax reference,
        IArtifactFileLookup sourceFileLookup,
        ISemanticModelLookup semanticModelLookup,
        DiagnosticBuilder.DiagnosticBuilderDelegate onInvalidSourceFileType)
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
            (ExtendsDeclarationSyntax, _) => false,
            (_, BicepFile or ArmTemplateFile or TemplateSpecFile) => true,
            _ => false,
        };
        if (!isValidReference)
        {
            return new(onInvalidSourceFileType(DiagnosticBuilder.ForPosition(reference.SourceSyntax)));
        }

        return new(semanticModelLookup.GetSemanticModel(sourceFile));
    }

    private static ResultWithDiagnostic<ISourceFile> TryGetSourceFile(IArtifactFileLookup sourceFileLookup, IArtifactReferenceSyntax reference)
    {
        if (!sourceFileLookup.TryGetSourceFile(reference).IsSuccess(out var sourceFile, out var errorBuilder))
        {
            return new(errorBuilder(DiagnosticBuilder.ForPosition(reference.SourceSyntax)));
        }

        return new(sourceFile);
    }
}
