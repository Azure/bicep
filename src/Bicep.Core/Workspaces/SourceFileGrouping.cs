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
        private readonly ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration;

        private readonly ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration;

        public SourceFileGrouping(IFileResolver fileResolver, BicepFile entryPoint, ImmutableHashSet<ISourceFile> sourceFiles, ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> sourceFilesByModuleDeclaration, ImmutableDictionary<ModuleDeclarationSyntax, ErrorBuilderDelegate> errorBuildersByModuleDeclaration)
        {
            this.FileResolver = fileResolver;
            this.EntryPoint = entryPoint;
            this.SourceFiles = sourceFiles;
            this.sourceFilesByModuleDeclaration = sourceFilesByModuleDeclaration;
            this.errorBuildersByModuleDeclaration = errorBuildersByModuleDeclaration;
        }
        public IFileResolver FileResolver { get; }

        public BicepFile EntryPoint { get; }

        public ImmutableHashSet<ISourceFile> SourceFiles { get; }

        public ISourceFile LookUpModuleSourceFile(ModuleDeclarationSyntax moduleDeclaration) =>
            this.sourceFilesByModuleDeclaration[moduleDeclaration];

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
