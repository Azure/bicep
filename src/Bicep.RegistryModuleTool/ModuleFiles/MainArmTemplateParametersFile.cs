// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Exceptions;
using Bicep.Core.Json;
using Bicep.RegistryModuleTool.ModuleFileValidators;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Bicep.RegistryModuleTool.ModuleFiles
{
    public record MainArmTemplateParameterInstance(string Name, object? Value);

    public sealed class MainArmTemplateParametersFile : ModuleFile
    {
        public const string FileName = "azuredeploy.parameters.json";

        private readonly Lazy<IEnumerable<MainArmTemplateParameterInstance>> lazyParameterValues;

        public MainArmTemplateParametersFile(string path, string content, JsonElement rootElement)
            : base(path)
        {
            this.Content = content;
            this.RootElement = rootElement;

            this.lazyParameterValues = new(() => !rootElement.TryGetProperty("parameters", out var parametersElement)
                ? Enumerable.Empty<MainArmTemplateParameterInstance>()
                : parametersElement.EnumerateObject().Select(ToParameterInstance));
        }

        public string Content { get; }

        public JsonElement RootElement { get; }

        public IEnumerable<MainArmTemplateParameterInstance> ParameterInstances => lazyParameterValues.Value;

        public static MainArmTemplateParametersFile Generate(IFileSystem fileSystem, MainArmTemplateFile mainArmTemplateFile)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { Indented = true }))
            {
                writer.WriteStartObject();

                writer.WriteString("$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");
                writer.WriteString("contentVersion", "1.0.0.0");

                writer.WritePropertyName("parameters");
                writer.WriteStartObject();

                var parameterInstances = GenerateParameterInstances(fileSystem, mainArmTemplateFile);

                foreach (var (parameterName, parameterValue) in parameterInstances)
                {
                    writer.WritePropertyName(parameterName);
                    writer.WriteStartObject();

                    if (parameterValue is not null)
                    {
                        writer.WritePropertyName("value");
                        JsonElementFactory.CreateElement(parameterValue).WriteTo(writer);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();

                writer.WriteEndObject();
            }

            var path = fileSystem.Path.GetFullPath(FileName);
            var content = Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
            var rootElement = JsonElementFactory.CreateElement(bufferWriter.WrittenMemory);

            return new(path, content, rootElement);
        }

        public static MainArmTemplateParametersFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                var content = fileSystem.File.ReadAllText(path);
                var jsonElement = JsonElementFactory.CreateElement(content);

                return new(path, content, jsonElement);
            }
            catch (JsonException jsonException)
            {
                throw new BicepException($"The ARM template parameters file \"{path}\" is not a valid JSON file. {jsonException.Message}");
            }
        }

        public MainArmTemplateParametersFile WriteToFileSystem(IFileSystem fileSystem)
        {
            fileSystem.File.WriteAllText(FileName, this.Content);

            return this;
        }

        protected override void ValidatedBy(IModuleFileValidator validator) => validator.Validate(this);

        private static IEnumerable<MainArmTemplateParameterInstance> GenerateParameterInstances(IFileSystem fileSystem, MainArmTemplateFile mainArmTemplateFile)
        {
            var parameterInstances = new List<MainArmTemplateParameterInstance>();
            var parametersByName = mainArmTemplateFile.Parameters.ToDictionary(x => x.Name, x => x);
            var existingParameterInstancesByName = new Dictionary<string, MainArmTemplateParameterInstance>();

            try
            {
                var existingParametersFile = ReadFromFileSystem(fileSystem);
                existingParameterInstancesByName = existingParametersFile.ParameterInstances.ToDictionary(x => x.Name, x => x);

                foreach (var parameterInstance in existingParametersFile.ParameterInstances)
                {
                    if (parametersByName.ContainsKey(parameterInstance.Name))
                    {
                        parameterInstances.Add(parameterInstance);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // Nothing to do.
            }
            
            foreach (var parameter in mainArmTemplateFile.Parameters)
            {
                if (parameter.Required && !existingParameterInstancesByName.ContainsKey(parameter.Name))
                {
                    parameterInstances.Add(CreateBlankParameterInstance(parameter));
                }
            }

            return parameterInstances;
        }

        private static MainArmTemplateParameterInstance CreateBlankParameterInstance(MainArmTemplateParameter parameter) =>
            new(parameter.Name, null);

        private MainArmTemplateParameterInstance ToParameterInstance(JsonProperty parameterInstanceProperty)
        {
            var name = parameterInstanceProperty.Name;

            if (parameterInstanceProperty.Value.TryGetProperty("value", out var value))
            {
                return new(name, value);
            }

            return new(name, null);
        }
    }
}
