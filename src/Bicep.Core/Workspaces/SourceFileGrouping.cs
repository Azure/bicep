// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGrouping
    {
        public SourceFileGrouping(
            IFileResolver fileResolver,
            BicepFile entryPoint,
            ImmutableHashSet<ISourceFile> sourceFiles,
            ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration,
            ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> sourceFileParentLookup,
            ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration,
            ImmutableHashSet<ModuleDeclarationSyntax> modulesToRestore)
        {
            this.FileResolver = fileResolver;
            this.EntryPoint = entryPoint;
            this.SourceFiles = sourceFiles;
            this.SourceFilesByModuleDeclaration = sourceFilesByModuleDeclaration;
            this.SourceFileParentLookup = sourceFileParentLookup;
            this.ErrorBuildersByModuleDeclaration = errorBuildersByModuleDeclaration;
            this.ModulesToRestore = modulesToRestore;
        }

        public ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> SourceFilesByModuleDeclaration { get; }

        public ImmutableDictionary<ISourceFile, ImmutableHashSet<ISourceFile>> SourceFileParentLookup { get; }

        public ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> ErrorBuildersByModuleDeclaration { get; }

        public ImmutableHashSet<ModuleDeclarationSyntax> ModulesToRestore { get; }

        public IFileResolver FileResolver { get; }

        public BicepFile EntryPoint { get; }

        public ImmutableHashSet<ISourceFile> SourceFiles { get; }

        public ISourceFile LookUpModuleSourceFile(ModuleDeclarationSyntax moduleDeclaration) =>
            this.SourceFilesByModuleDeclaration[moduleDeclaration];

        public ISourceFile? TryLookupModuleSourceFile(ModuleDeclarationSyntax moduleDeclaration) =>
            this.SourceFilesByModuleDeclaration.TryGetValue(moduleDeclaration, out var sourceFile) ? sourceFile : null;

        public ImmutableHashSet<ISourceFile> GetFilesDependingOn(ISourceFile sourceFile)
        {
            var filesToCheck = new Queue<ISourceFile>(new [] { sourceFile });
            var knownFiles = new HashSet<ISourceFile>();

            while (filesToCheck.TryDequeue(out var current))
            {
                knownFiles.Add(current);

                if (SourceFileParentLookup.TryGetValue(current, out var parents))
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
            if (this.ErrorBuildersByModuleDeclaration.TryGetValue(moduleDeclaration, out var errorBuilder))
            {
                errorDiagnostic = errorBuilder(ForPosition(moduleDeclaration.Path));
                return true;
            }

            errorDiagnostic = null;
            return false;
        }
    }
}
