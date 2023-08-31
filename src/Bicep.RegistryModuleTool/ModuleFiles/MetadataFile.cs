// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleValidators;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    // metadata.json is now obsolete, but we will read it if there and convert it to Bicep metadata on generate
    public sealed class MetadataFile : ModuleFile
    {
        public const string FileName = "metadata.json";

        public MetadataFile(string path, JsonElement rootElement)
            : base(path)
        {
            this.RootElement = rootElement;
        }

        public JsonElement RootElement { get; }

        public string? Name => this.RootElement.TryGetProperty("name", out var element) ? element.GetString() : null;

        public string? Summary => this.RootElement.TryGetProperty("summary", out var element) ? element.GetString() :
            //  Some older metadata.json files have "description" instead of "metadata"
            this.RootElement.TryGetProperty("description", out var elementDescription) ? elementDescription.GetString() : null;

        public string? Owner => this.RootElement.TryGetProperty("owner", out var element) ? element.GetString() : null;

        public static MetadataFile? TryReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            if (!fileSystem.Path.Exists(path))
            {
                return null;
            }

            try
            {
                using var stream = fileSystem.FileStream.New(path, FileMode.Open, FileAccess.Read);
                var jsonElement = JsonElementFactory.CreateElementFromStream(stream);

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

        public void DeleteFile(IFileSystem fileSystem)
        {
            if (fileSystem.File.Exists(this.Path))
            {
                fileSystem.File.Delete(this.Path);
            }
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
