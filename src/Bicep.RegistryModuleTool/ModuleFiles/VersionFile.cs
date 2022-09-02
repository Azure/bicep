// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleValidators;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class VersionFile : ModuleFile
    {
        public const string FileName = "version.json";

        private static readonly JsonElement EmptyFileElement = JsonElementFactory.CreateElement(new Dictionary<string, object>
        {
            ["$schema"] = "https://aka.ms/bicep-registry-module-version-file-schema#",
            ["version"] = "",
            ["pathFilters"] = new[]
            {
                "./main.json",
                "./metadata.json"
            },
        });

        private static readonly JsonElement NoVersionFileElement = EmptyFileElement.Patch(JsonPatchOperations.Remove("/version"));

        public VersionFile(string path, string contents, JsonElement rootElement)
            : base(path)
        {
            this.Contents = contents;
            this.RootElement = rootElement;
        }

        public string Contents { get; }

        public JsonElement RootElement { get; }

        public static VersionFile Generate(IFileSystem fileSystem)
        {

            var path = fileSystem.Path.GetFullPath(FileName);
            var rootElement = EmptyFileElement;

            try
            {
                var existingFile = ReadFromFileSystem(fileSystem);

                // Merge NoVersionFileElement at last in case the author changed $schema or pathFilter.
                rootElement = rootElement
                    .Merge(existingFile.RootElement)
                    .Merge(NoVersionFileElement);
            }
            catch (FileNotFoundException)
            {
                // Nothing to do.
            }

            var content = rootElement.ToFormattedString();

            return new(path, content, rootElement);
        }

        public static VersionFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                var content = fileSystem.File.ReadAllText(FileName);
                var rootElement = JsonElementFactory.CreateElement(content);

                if (rootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new BicepException($"The version file \"{path}\" must contain a JSON object at the root level.");
                }

                return new(path, content, rootElement);
            }
            catch (JsonException jsonException)
            {
                throw new BicepException($"The version file \"{path}\" is not a valid JSON file. {jsonException.Message}");
            }
        }

        public VersionFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(this.Path, this.Contents);

            return this;
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
