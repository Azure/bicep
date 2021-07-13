// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Syntax;

namespace Bicep.Core.Workspaces
{
    public class SourceFileGrouping
    {
        public BicepFile EntryPoint { get; }

        public ImmutableHashSet<ISourceFile> SourceFiles { get; }

        public ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> ModuleLookup { get; }

        public ImmutableDictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> ModuleFailureLookup { get; }

        public IFileResolver FileResolver { get; }

        public SourceFileGrouping(BicepFile entryPoint, ImmutableHashSet<ISourceFile> sourceFiles, ImmutableDictionary<ModuleDeclarationSyntax, ISourceFile> moduleLookup, ImmutableDictionary<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate> moduleFailureLookup, IFileResolver fileResolver)
        {
            EntryPoint = entryPoint;
            SourceFiles = sourceFiles;
            ModuleLookup = moduleLookup;
            ModuleFailureLookup = moduleFailureLookup;
            FileResolver = fileResolver;
        }
    }
}
