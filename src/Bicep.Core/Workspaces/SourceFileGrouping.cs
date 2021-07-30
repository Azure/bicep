// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using System.Linq;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGrouping
    {
        private readonly ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration;
        private readonly ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> sourceFileParentLookup;

        private readonly ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration;

        public SourceFileGrouping(
            IFileResolver fileResolver,
            BicepFile entryPoint,
            ImmutableHashSet<ISourceFile> sourceFiles,
            ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration,
            ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> sourceFileParentLookup,
            ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration)
        {
            this.FileResolver = fileResolver;
            this.EntryPoint = entryPoint;
            this.SourceFiles = sourceFiles;
            this.sourceFilesByModuleDeclaration = sourceFilesByModuleDeclaration;
            this.sourceFileParentLookup = sourceFileParentLookup;
            this.errorBuildersByModuleDeclaration = errorBuildersByModuleDeclaration;
        }
        public IFileResolver FileResolver { get; }

        public BicepFile EntryPoint { get; }

        public ImmutableHashSet<ISourceFile> SourceFiles { get; }

        public ISourceFile LookUpModuleSourceFile(ModuleDeclarationSyntax moduleDeclaration) =>
            this.sourceFilesByModuleDeclaration[moduleDeclaration];

        public ImmutableHashSet<ISourceFile> GetFilesDependingOn(ISourceFile sourceFile)
        {
            var filesToCheck = new Queue<ISourceFile>(new [] { sourceFile });
            var knownFiles = new HashSet<ISourceFile>();

            while (filesToCheck.TryDequeue(out var current))
            {
                knownFiles.Add(current);

                if (sourceFileParentLookup.TryGetValue(current, out var parents))
                {
                    foreach (var parent in parents.Where(x => !knownFiles.Contains(x)))
                    {
                        knownFiles.Add(parent);
                        filesToCheck.Enqueue(parent);
                    }
                }
            }

            return knownFiles.ToImmutableHashSet();
        }

        public bool TryLookUpModuleErrorDiagnostic(ModuleDeclarationSyntax moduleDeclaration, [NotNullWhen(true)] out ErrorDiagnostic? errorDiagnostic)
        {
            if (this.errorBuildersByModuleDeclaration.TryGetValue(moduleDeclaration, out var errorBuilder))
            {
                errorDiagnostic = errorBuilder(ForPosition(moduleDeclaration.Path));
                return true;
            }

            errorDiagnostic = null;
            return false;
        }
    }
}
