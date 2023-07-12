// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Navigation;
using Bicep.Core.Syntax;
using Bicep.RegistryModuleTool.ModuleValidators;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class MainBicepFile : ModuleFile
    {
        public const string FileName = "main.bicep";

        public const string ModuleNameMetadataName = "name";
        public const string ModuleDescriptionMetadataName = "description";
        public const string ModuleOwnerMetadataName = "owner";

        public MainBicepFile(string path, string contents)
            : base(path)
        {
            this.Contents = contents;
        }

        public string Contents { get; }

        public static MainBicepFile EnsureInFileSystem(IFileSystem fileSystem)
        {
            string path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                using (fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read)) { }
            }
            catch (FileNotFoundException)
            {
                fileSystem.File.WriteAllText(path, @"metadata name = ''
metadata description = ''
metadata owner = ''

".ReplaceLineEndings());
            }

            return ReadFromFileSystem(fileSystem);
        }

        public static MainBicepFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            string path = fileSystem.Path.GetFullPath(FileName);

            using (fileSystem.FileStream.New(path, FileMode.Open)) { }

            return new(path, fileSystem.File.ReadAllText(path));
        }

        public static MainBicepFile Generate(IFileSystem fileSystem, MetadataFile? metadataFile, MainArmTemplateFile mainArmTemplateFile)
        {
            MainBicepFile existingBicepFile = EnsureInFileSystem(fileSystem);

            if (metadataFile is null)
            {
                return new(fileSystem.Path.GetFullPath(FileName), existingBicepFile.Contents);
            }

            // Move contents from metadata.json into main.bicep as metadata
            var linesToPrepend = new StringBuilder();

            if (mainArmTemplateFile.NameMetadata is null)
            {
                string moduleName = metadataFile?.Name ?? "TODO: Enter the module name";
                linesToPrepend.AppendLine($"metadata {MainBicepFile.ModuleNameMetadataName} = {MainBicepFile.FormatBicepString(moduleName)}");
            }
            if (mainArmTemplateFile.DescriptionMetadata is null)
            {
                string description = metadataFile?.Summary ?? "TODO: Enter a short description for the module";
                linesToPrepend.AppendLine($"metadata {MainBicepFile.ModuleDescriptionMetadataName} = {MainBicepFile.FormatBicepString(description)}");
            }
            if (mainArmTemplateFile.OwnerMetadata is null)
            {
                string owner = metadataFile?.Owner ?? "TODO: Enter the owner's github username";
                linesToPrepend.AppendLine($"metadata {MainBicepFile.ModuleOwnerMetadataName} = {MainBicepFile.FormatBicepString(owner)}");
            }

            // Delete metadata.json
            if (metadataFile is not null)
            {
                metadataFile.DeleteFile(fileSystem);
            }

            string newBicepContents;
            if (linesToPrepend.Length > 0)
            {
                newBicepContents = linesToPrepend.ToString()
                    + ((existingBicepFile.Contents.StartsWith("metadata ")) ? "" : "\n")
                    + existingBicepFile.Contents;
            }
            else
            {
                newBicepContents = existingBicepFile.Contents;
            }

            return new(fileSystem.Path.GetFullPath(FileName), newBicepContents);
        }

        public MainBicepFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(this.Path, this.Contents);

            return this;
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);

        private static string FormatBicepString(string value) {
            return SyntaxFactory.CreateStringLiteral(value).ToText();
        }
    }
}
