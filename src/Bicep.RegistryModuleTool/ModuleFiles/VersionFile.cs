// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using System.Text.Json;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleFileValidators;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class VersionFile : ModuleFile
    {
        public const string FileName = "version.json";

        private static readonly JsonElement DefaultRootElement = JsonElementFactory.CreateElement(new Dictionary<string, object>
        {
            ["$schema"] = "https://aka.ms/bicep-registry-module-version-file-schema#",
            ["version"] = ""
        });

        private static readonly JsonElement DefaultRootElementWithoutVersion = DefaultRootElement.Patch(JsonPatchOperations.Remove("/version"));

        public VersionFile(string path, string contents, JsonElement rootElement)
            : base(path)
        {
            this.Contents = contents;
            this.RootElement = rootElement;
        }

        public string Contents { get; }

        public JsonElement RootElement { get; }

        public static async Task<VersionFile> GenerateAsync(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var rootElement = DefaultRootElement;

            try
            {
                var existingFile = await OpenAsync(fileSystem);

                // Merge DefaultRootElementWithoutVersion in case the author changed $schema.
                rootElement = rootElement
                    .Merge(existingFile.RootElement)
                    .Merge(DefaultRootElementWithoutVersion);
            }
            catch (FileNotFoundException)
            {
                // Nothing to do.
            }

            var contents = rootElement.ToIndentedString();

            await fileSystem.File.WriteAllTextAsync(path, contents);

            return new(path, contents, rootElement);
        }

        public static async Task<VersionFile> OpenAsync(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                var contents = await fileSystem.File.ReadAllTextAsync(FileName);
                var rootElement = JsonElementFactory.CreateElement(contents);

                if (rootElement.ValueKind != JsonValueKind.Object)
                {
                    throw new BicepException($"The version file \"{path}\" must contain a JSON object at the root level.");
                }

                return new(path, contents, rootElement);
            }
            catch (JsonException jsonException)
            {
                throw new BicepException($"The version file \"{path}\" is not a valid JSON file. {jsonException.Message}");
            }
        }

        protected override Task<IEnumerable<string>> ValidatedByAsync(IModuleFileValidator validator) => validator.ValidateAsync(this);
    }
}
