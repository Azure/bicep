// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.FileSystem;
using Bicep.Core.Navigation;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.RegistryModuleTool.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.CommandLine;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public readonly record struct ModuleMetadata(string Name, string? Value)
    {
        public static readonly ModuleMetadata Undefined = new("", null);

        public bool IsUndefined => this == Undefined;
    }

    public sealed class MainBicepFile : ModuleFile
    {
        public const string FileName = "main.bicep";

        public const string ModuleNameMetadataName = "name";

        public const string ModuleDescriptionMetadataName = "description";

        public const string ModuleOwnerMetadataName = "owner";

        private static readonly string DefaultModuleNameMetadata = $"metadata {ModuleNameMetadataName} = 'TODO: <module name>'";

        private static readonly string DefaultModuleDescriptionMetadata = $"metadata {ModuleDescriptionMetadataName} = 'TODO: <module description>'";

        private static readonly string DefaultModuleOwnerMetadata = $"metadata {ModuleOwnerMetadataName} = 'TODO: <GitHub username of module owner>'";

        private static readonly string DefaultContent = $"""
            {DefaultModuleNameMetadata}
            {DefaultModuleDescriptionMetadata}
            {DefaultModuleOwnerMetadata}

            """.ReplaceLineEndings();

        private MainBicepFile(string path, SemanticModel semanticModel)
            : base(path)
        {
            this.SemanticModel = semanticModel;
        }

        public SemanticModel SemanticModel { get; }

        // Do not use SemanticModel.Parameters.Values() since the order may change.
        public IEnumerable<ParameterMetadata> Parameters => this.SemanticModel.Root.ParameterDeclarations
            .Select(x => this.SemanticModel.Parameters[x.Name]);

        public IEnumerable<OutputMetadata> Outputs => this.SemanticModel.Outputs;

        public ModuleMetadata TryGetMetadata(string metadataName)
        {
            foreach (var metadataSymbol in this.SemanticModel.Root.MetadataDeclarations)
            {
                if (metadataSymbol.Name.Equals(metadataName, StringComparison.Ordinal) &&
                    metadataSymbol.DeclaringSyntax is MetadataDeclarationSyntax metadataDeclarationSyntax &&
                    metadataDeclarationSyntax.Value is StringSyntax stringSyntax)
                {
                    return new(metadataName, stringSyntax.TryGetLiteralValue());
                }
            }

            return ModuleMetadata.Undefined;
        }

        public static async Task<MainBicepFile> GenerateAsync(IFileSystem fileSystem, BicepCompiler compiler, IConsole console)
        {
            string path = fileSystem.Path.GetFullPath(FileName);
            string content;

            if (fileSystem.File.Exists(path))
            {
                content = await fileSystem.File.ReadAllTextAsync(path);
            }
            else
            {
                content = DefaultContent;
                await fileSystem.File.WriteAllTextAsync(path, content);
            }

            var compilation = await compiler.CompileAsync(path, console);
            var semanticModel = compilation.GetEntrypointSemanticModel();
            var metadataNames = semanticModel.Root.MetadataDeclarations.Select(x => x.Name).ToHashSet();
            var metadataLinesToInsert = new StringBuilder();

            if (!metadataNames.Contains(ModuleNameMetadataName))
            {
                metadataLinesToInsert.AppendLine(DefaultModuleNameMetadata);
            }

            if (!metadataNames.Contains(ModuleDescriptionMetadataName))
            {
                metadataLinesToInsert.AppendLine(DefaultModuleDescriptionMetadata);
            }

            if (!metadataNames.Contains(ModuleOwnerMetadataName))
            {
                metadataLinesToInsert.AppendLine(DefaultModuleOwnerMetadata);;
            }

            if (metadataLinesToInsert.Length > 0)
            {
                var firstMetadataDeclarationSyntax = semanticModel.Root.MetadataDeclarations.FirstOrDefault()?.DeclaringSyntax;

                if (firstMetadataDeclarationSyntax is not null)
                {
                    content = content.Insert(firstMetadataDeclarationSyntax.Span.Position, metadataLinesToInsert.ToString());
                }
                else
                {
                    metadataLinesToInsert.AppendLine();
                    content = $"{metadataLinesToInsert}{content}";
                }

                await fileSystem.File.WriteAllTextAsync(path, content);
                compilation = await compiler.CompileAsync(path, console);
                semanticModel = compilation.GetEntrypointSemanticModel();
            }

            return new(path, semanticModel);
        }

        public static async Task<MainBicepFile> OpenAsync(IFileSystem fileSystem, BicepCompiler compiler, IConsole console)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            // Ensure file exists.
            using (fileSystem.FileStream.New(path, FileMode.Open)) { }

            var compilation = await compiler.CompileAsync(path, console);
            var semanticModel = compilation.GetEntrypointSemanticModel();

            return new(path, semanticModel);
        }

        protected override Task<IEnumerable<string>> ValidatedByAsync(IModuleFileValidator validator) => validator.ValidateAsync(this);
    }
}
