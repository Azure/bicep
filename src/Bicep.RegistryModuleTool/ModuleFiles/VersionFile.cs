// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System.Buffers;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public sealed class VersionFile : ModuleFile
    {
        public const string FileName = "version.json";

        private static readonly Regex VersionRegex = new(@"0|[1-9]\d*\.0|[1-9]\d*", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public VersionFile(string path, string content)
            : base(path)
        {
            this.Content = content;
        }

        public string Content { get; }

        public static VersionFile Generate(IFileSystem fileSystem)
        {
            var currentDirectoryName = fileSystem.Directory.GetCurrentDirectoryName();

            if (!VersionRegex.IsMatch(currentDirectoryName))
            {
                throw new BicepException(@$"The directory name ""{currentDirectoryName}"" must be in the format of a version number ""MAJOR.MINOR"".");
            }

            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();

                writer.WriteString("$schema", "https://raw.githubusercontent.com/dotnet/Nerdbank.GitVersioning/master/src/NerdBank.GitVersioning/version.schema.json");
                writer.WriteString("version", currentDirectoryName);

                writer.WritePropertyName("pathFilters");
                writer.WriteStartArray();
                writer.WriteStringValue("./main.json");
                writer.WriteEndArray();

                writer.WriteEndObject();
            }

            var content = Encoding.UTF8.GetString(bufferWriter.WrittenSpan);

            return new(fileSystem.Path.GetFullPath(FileName), content);
        }

        public static VersionFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = fileSystem.File.ReadAllText(FileName);

            return new(path, content);
        }

        public VersionFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(this.Path, this.Content);

            return this;
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
