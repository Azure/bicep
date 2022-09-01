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
    public sealed class MetadataFile : ModuleFile
    {
        public const string FileName = "metadata.json";

        private static readonly JsonElement EmptyFileElement = JsonElementFactory.CreateElement(new Dictionary<string, string>
        {
            ["$schema"] = "https://aka.ms/bicep-registry-module-metadata-file-schema-v2#",
            ["name"] = "",
            ["summary"] = "",
            ["owner"] = "",
        });

        public MetadataFile(string path, JsonElement rootElement)
            : base(path)
        {
            this.RootElement = rootElement;
        }

        public JsonElement RootElement { get; }

        public string? Name => this.RootElement.TryGetProperty("name", out var element) ? element.GetString() : null;

        public string? Summary => this.RootElement.TryGetProperty("summary", out var element) ? element.GetString() : null;

        public static MetadataFile EnsureInFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var rootElement = EmptyFileElement;

            try
            {
                var existingFile = ReadFromFileSystem(fileSystem);
                rootElement = rootElement.Merge(existingFile.RootElement);
            }
            catch (FileNotFoundException)
            {
                // Nothing to do.
            }

            using var writeStream = fileSystem.FileStream.Create(path, FileMode.Create, FileAccess.Write);
            using var writer = new Utf8JsonWriter(writeStream, new JsonWriterOptions { Indented = true });

            rootElement.WriteTo(writer);

            return new(path, rootElement);
        }

        public static MetadataFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                using var stream = fileSystem.FileStream.Create(path, FileMode.Open, FileAccess.Read);
                var jsonElement = JsonElementFactory.CreateElement(stream);

                if (jsonElement.ValueKind != JsonValueKind.Object)
                {
                    throw new BicepException($"The metadata file \"{path}\" must contain a JSON object at the root level.");
                }

                return new(path, jsonElement);
            }
            catch (JsonException jsonException)
            {
                throw new BicepException($"The metadata file \"{path}\" is not a valid JSON file. {jsonException.Message}");
            }
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
