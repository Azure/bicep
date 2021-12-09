// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class MetadataFile : ModuleFile
    {
        public const string FileName = "metadata.json";

        public MetadataFile(string path, JsonElement rootElement)
            : base(path)
        {
            this.RootElement = rootElement;
        }

        public JsonElement RootElement { get; }

        public string? ItemDisplayName => this.RootElement.TryGetProperty("itemDisplayName", out var element) ? element.GetString() : null;

        public string? Description => this.RootElement.TryGetProperty("description", out var element) ? element.GetString() : null;

        public static void CreateInFileSystem(IFileSystem fileSystem)
        {
            using var stream = fileSystem.FileStream.Create(FileName, FileMode.CreateNew);
            using var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

            writer.WriteStartObject();

            writer.WriteString("$schema", "https://aka.ms/azure-quickstart-templates-metadata-schema#");
            writer.WriteString("type", "");
            writer.WriteString("itemDisplayName", "");
            writer.WriteString("description", "");
            writer.WriteString("summary", "");
            writer.WriteString("githubUsername", "");
            writer.WriteString("dateUpdated", DateTime.Now.ToString("yyyy-MM-dd"));

            writer.WriteEndObject();
        }

        public static MetadataFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                using var stream = fileSystem.FileStream.Create(path, FileMode.Open, FileAccess.Read);
                var jsonElement = JsonElementFactory.CreateElement(stream);

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
