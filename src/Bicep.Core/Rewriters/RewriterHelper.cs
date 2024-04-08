// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Rewriters;

public static class RewriterHelper
{
    private static (BicepSourceFile bicepFile, bool hasChanges) Rewrite(BicepCompiler compiler, Workspace workspace, BicepSourceFile bicepFile, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
    {
        var compilation = compiler.CreateCompilationWithoutRestore(bicepFile.FileUri, workspace);
        var newProgramSyntax = rewriteVisitorBuilder(compilation.GetEntrypointSemanticModel()).Rewrite(bicepFile.ProgramSyntax);

        if (object.ReferenceEquals(bicepFile.ProgramSyntax, newProgramSyntax))
        {
            return (bicepFile, false);
        }

        bicepFile = SourceFileFactory.CreateBicepFile(bicepFile.FileUri, newProgramSyntax.ToString());
        return (bicepFile, true);
    }

    public static BicepSourceFile RewriteMultiple(BicepCompiler compiler, Compilation compilation, BicepSourceFile bicepFile, int rewritePasses, params Func<SemanticModel, SyntaxRewriteVisitor>[] rewriteVisitorBuilders)
    {
        var workspace = new Workspace();
        workspace.UpsertSourceFiles(compilation.SourceFileGrouping.SourceFiles);
        var fileUri = bicepFile.FileUri;

        // Changing the syntax changes the semantic model, so it's possible for rewriters to have dependencies on each other.
        // For example, fixing the casing of a type may fix type validation, causing another rewriter to apply.
        // To handle this, run the rewriters in a loop until we see no more changes.
        for (var i = 0; i < rewritePasses; i++)
        {
            var hasChanges = false;
            foreach (var rewriteVisitorBuilder in rewriteVisitorBuilders)
            {
                var result = Rewrite(compiler, workspace, bicepFile, rewriteVisitorBuilder);

                if (result.hasChanges)
                {
                    hasChanges = true;
                    bicepFile = result.bicepFile;
                    workspace.UpsertSourceFile(result.bicepFile);
                }
            }

            if (!hasChanges)
            {
                break;
            }
        }

        return bicepFile;
    }
}
