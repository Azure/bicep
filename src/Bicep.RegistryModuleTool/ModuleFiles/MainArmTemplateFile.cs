// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Bicep.Core.Emit;
using Bicep.RegistryModuleTool.ModuleFileValidators;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public record MainArmTemplateParameter(string Name, string Type, bool Required, string? Description);

    public record MainArmTemplateOutput(string Name, string Type, string? Description);

    public sealed class MainArmTemplateFile : ModuleFile
    {
        public const string FileName = "main.json";

        public MainArmTemplateFile(string path, string content)
            : base(path)
        {
            this.Content = content;
        }

        public string Content { get; }

        public static async Task<MainArmTemplateFile> GenerateAsync(IFileSystem fileSystem, MainBicepFile mainBicepFile)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var writer = new StringWriter();
            var emitter = new TemplateEmitter(mainBicepFile.SemanticModel);

            emitter.Emit(writer);

            var content = writer.ToString();

            await fileSystem.File.WriteAllTextAsync(path, content);

            return new(path, content);
        }

        public static async Task<MainArmTemplateFile> OpenAsync(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = await fileSystem.File.ReadAllTextAsync(path);

            return new(path, content);
        }

        protected override Task<IEnumerable<string>> ValidatedByAsync(IModuleFileValidator validator) => validator.ValidateAsync(this);
    }
}
