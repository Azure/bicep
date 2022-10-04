// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Rewriters
{
    public static class RewriterHelper
    {
        public static (BicepFile bicepFile, bool hasChanges) Rewrite(Compilation prevCompilation, BicepFile bicepFile, Func<SemanticModel, SyntaxRewriteVisitor> rewriteVisitorBuilder)
        {
            // Sometimes bicepFile does not exist in the previous compilation, in which case we fall back on the compilation's entry point model
            var prevModel = prevCompilation.SourceFileGrouping.FileResultByUri.ContainsKey(bicepFile.FileUri) ? prevCompilation.GetSemanticModel(bicepFile) : prevCompilation.GetEntrypointSemanticModel();
            var semanticModel = new SemanticModel(prevCompilation, bicepFile, prevModel.FileResolver, prevModel.LinterAnalyzer, prevModel.Configuration, prevModel.Features, prevModel.ApiVersionProvider);
            var newProgramSyntax = rewriteVisitorBuilder(semanticModel).Rewrite(bicepFile.ProgramSyntax);

            if (object.ReferenceEquals(bicepFile.ProgramSyntax, newProgramSyntax))
            {
                return (bicepFile, false);
            }

            bicepFile = SourceFileFactory.CreateBicepFile(bicepFile.FileUri, newProgramSyntax.ToTextPreserveFormatting());
            return (bicepFile, true);
        }

        public static BicepFile RewriteMultiple(Compilation prevCompilation, BicepFile bicepFile, int rewritePasses, params Func<SemanticModel, SyntaxRewriteVisitor>[] rewriteVisitorBuilders)
        {
            // Changing the syntax changes the semantic model, so it's possible for rewriters to have dependencies on each other.
            // For example, fixing the casing of a type may fix type validation, causing another rewriter to apply.
            // To handle this, run the rewriters in a loop until we see no more changes.
            for (var i = 0; i < rewritePasses; i++)
            {
                var hasChanges = false;
                foreach (var rewriteVisitorBuilder in rewriteVisitorBuilders)
                {
                    var result = Rewrite(prevCompilation, bicepFile, rewriteVisitorBuilder);
                    hasChanges |= result.hasChanges;
                    bicepFile = result.bicepFile;
                }

                if (!hasChanges)
                {
                    break;
                }
            }

            return bicepFile;
        }
    }
}
