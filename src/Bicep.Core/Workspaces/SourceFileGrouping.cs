// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
            ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration,
            ImmutableHashSet<ModuleDeclarationSyntax> modulesToRestore)
        {
            this.FileResolver = fileResolver;
            this.EntryPoint = entryPoint;
            this.SourceFiles = sourceFiles;
            this.SourceFilesByModuleDeclaration = sourceFilesByModuleDeclaration;
            this.ErrorBuildersByModuleDeclaration = errorBuildersByModuleDeclaration;
            this.ModulesToRestore = modulesToRestore;
        }

        public ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> SourceFilesByModuleDeclaration { get; }

        public ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> ErrorBuildersByModuleDeclaration { get; }

        public ImmutableHashSet<ModuleDeclarationSyntax> ModulesToRestore { get; }

        public IFileResolver FileResolver { get; }

        public BicepFile EntryPoint { get; }

        public ImmutableHashSet<ISourceFile> SourceFiles { get; }

        public ISourceFile LookUpModuleSourceFile(ModuleDeclarationSyntax moduleDeclaration) =>
            this.SourceFilesByModuleDeclaration[moduleDeclaration];

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
