// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Json;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Bicep.RegistryModuleTool.Extensions;
using Bicep.RegistryModuleTool.ModuleValidators;
using Bicep.RegistryModuleTool.Proxies;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public record MainArmTemplateParameter(string Name, string Type, bool Required, string? Description);

    public record MainArmTemplateOutput(string Name, string Type, string? Description);

    public sealed class MainArmTemplateFile : ModuleFile
    {
        public const string FileName = "main.json";

        private ArmTemplateSemanticModel armTemplate;

        private readonly Lazy<JsonElement> lazyRootElement;

        private readonly Lazy<IEnumerable<MainArmTemplateParameter>> lazyParameters;

        private readonly Lazy<IEnumerable<MainArmTemplateOutput>> lazyOutputs;

        private readonly Lazy<string> lazyTemplateHash;

        public MainArmTemplateFile(string path, string content)
            : base(path)
        {
            this.Content = content;

            this.lazyRootElement = new(() => JsonElementFactory.CreateElement(content));

            this.armTemplate = new ArmTemplateSemanticModel(SourceFileFactory.CreateArmTemplateFile(new Uri("inmemory://" + this.Path), this.Content));
            this.lazyParameters = new(() => !lazyRootElement.Value.TryGetProperty("parameters", out var parametersElement)
                ? Enumerable.Empty<MainArmTemplateParameter>()
                : parametersElement.EnumerateObject().Select(ToParameter));
            this.lazyOutputs = new(() => !lazyRootElement.Value.TryGetProperty("outputs", out var outputsElement)
                ? Enumerable.Empty<MainArmTemplateOutput>()
                : outputsElement.EnumerateObject().Select(ToOutput));
            this.lazyTemplateHash = new(() => lazyRootElement.Value.GetPropertyByPath("metadata._generator.templateHash").ToNonNullString());
        }

        private static string GetPrimitiveTypeName(ITypeReference typeRef) => typeRef.Type switch {
            StringType or StringLiteralType
                => typeRef.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) ? "securestring" : "string",
            UnionType unionOfStrings when unionOfStrings.Members.All(m => m.Type is StringLiteralType || m.Type is StringType)
                => "string",
            IntegerType or IntegerLiteralType => "int",
            UnionType unionOfInts when unionOfInts.Members.All(m => m.Type is IntegerLiteralType || m.Type is IntegerType)
                => "int",
            BooleanType or BooleanLiteralType => "bool",
            UnionType unionOfBools when unionOfBools.Members.All(m => m.Type is BooleanLiteralType || m.Type is BooleanType)
                => "bool",
            ObjectType => typeRef.Type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsSecure) ? "secureObject" : "object",
            ArrayType => "array",
            TypeSymbol otherwise => throw new InvalidOperationException($"Unable to determine primitive type of {otherwise.Name}"),
        };

        public string Content { get; }

        public JsonElement RootElement => this.lazyRootElement.Value;

        public IEnumerable<MainArmTemplateParameter> Parameters => this.lazyParameters.Value;

        public IEnumerable<MainArmTemplateOutput> Outputs => this.lazyOutputs.Value;

        public string TemplateHash => this.lazyTemplateHash.Value;

        public static MainArmTemplateFile Generate(IFileSystem fileSystem, BicepCliProxy bicepCliProxy, MainBicepFile mainBicepFile)
        {
            var tempFilePath = fileSystem.Path.GetTempFileName();

            try
            {
                bicepCliProxy.Build(mainBicepFile.Path, tempFilePath);
            }
            catch (Exception)
            {
                fileSystem.File.Delete(tempFilePath);

                throw;
            }

            using var tempFileStream = fileSystem.FileStream.CreateDeleteOnCloseStream(tempFilePath);
            using var streamReader = new StreamReader(tempFileStream);

            var path = fileSystem.Path.GetFullPath(FileName);
            var content = streamReader.ReadToEnd();

            return new(path, content);
        }

        public static MainArmTemplateFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);
            var content = fileSystem.File.ReadAllText(FileName);

            return new(path, content);
        }

        public MainArmTemplateFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(this.Path, this.Content);

            return this;
        }

        private MainArmTemplateParameter ToParameter(JsonProperty parameterProperty)
        {
            var parameters = this.armTemplate.Parameters;

            string name = parameters[parameterProperty.Name].Name;
            string type = GetPrimitiveTypeName(parameters[parameterProperty.Name].TypeReference);
            bool required = parameters[parameterProperty.Name].IsRequired;
            string? description = parameters[parameterProperty.Name].Description;

            return new(name, type, required, description);
        }

        private MainArmTemplateOutput ToOutput(JsonProperty outputProperty)
        {
            var outputs = this.armTemplate.Outputs.ToImmutableDictionaryExcludingNullValues(x => x.Name, StringComparer.Ordinal);

            string name = outputs[outputProperty.Name].Name;
            string type = GetPrimitiveTypeName(outputs[outputProperty.Name].TypeReference);
            string? description = outputs[outputProperty.Name].Description;

            return new(name, type, description);
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);
    }
}
