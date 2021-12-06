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
    internal record MainArmTemplateParameterInstance(string Name, object Value);

    internal class MainArmTemplateParametersFile : ModuleFile
    {
        private const string FileName = "main.parameters.json";

        private readonly Lazy<IEnumerable<MainArmTemplateParameterInstance>> lazyParameterValues;

        public MainArmTemplateParametersFile(string path, JsonElement rootElement)
            : base(path)
        {
            this.RootElement = rootElement;

            this.lazyParameterValues = new(() => !rootElement.TryGetProperty("parameters", out var parametersElement)
                ? Enumerable.Empty<MainArmTemplateParameterInstance>()
                : parametersElement.EnumerateObject().Select(ToParameterInstance));
        }

        public JsonElement RootElement { get; }

        public IEnumerable<MainArmTemplateParameterInstance> ParameterInstances => lazyParameterValues.Value;

        public static MainArmTemplateParametersFile Generate(IFileSystem fileSystem, MainArmTemplateFile mainArmTemplateFile)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using var writer = new Utf8JsonWriter(bufferWriter, new JsonWriterOptions { Indented = true });

            writer.WriteStartObject();

            writer.WriteString("$schema", "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#");
            writer.WriteString("contentVersion", "1.0.0.0");

            writer.WritePropertyName("parameters");
            writer.WriteStartObject();

            var parameterInstances = fileSystem.File.Exists(FileName)
                ? GenerateParameterInstancesWithExistingFile(ReadFromFileSystem(fileSystem), mainArmTemplateFile)
                : mainArmTemplateFile.Parameters
                    .Where(x => x.Required)
                    .Select(GenerateDummyParameterInstance);

            foreach (var parameterInstance in parameterInstances)
            {
                writer.WritePropertyName(parameterInstance.Name);
                writer.WriteStartObject();
                JsonElementFactory.CreateElement(parameterInstance.Value).WriteTo(writer);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();

            writer.WriteEndObject();

            var path = fileSystem.Path.GetFullPath(FileName);
            var rootElement = JsonElementFactory.CreateElement(bufferWriter.WrittenMemory);

            return new(path, rootElement);
        }

        public static MainArmTemplateParametersFile ReadFromFileSystem(IFileSystem fileSystem)
        {
            var path = fileSystem.Path.GetFullPath(FileName);

            try
            {
                using var stream = fileSystem.FileStream.Create(FileName, FileMode.Open, FileAccess.Read);
                var jsonElement = JsonElementFactory.CreateElement(stream);

                return new(path, jsonElement);
            }
            catch (JsonException jsonException)
            {
                throw new BicepException($"The ARM template parameters file \"{path}\" is not a valid JSON file. {jsonException.Message}");
            }
        }

        protected override void ValidatedBy(IModuleFileValidator validator)
        {
            validator.Validate(this);
        }

        public static IEnumerable<MainArmTemplateParameterInstance> GenerateParameterInstancesWithExistingFile(
            MainArmTemplateParametersFile existingFile,
            MainArmTemplateFile mainArmTemplateFile)
        {
            var mergedParameterInstances = new List<MainArmTemplateParameterInstance>();
            var parametersByName = mainArmTemplateFile.Parameters.ToDictionary(x => x.Name, x => x);
            var parameterInstancesByName = existingFile.ParameterInstances.ToDictionary(x => x.Name, x => x);

            foreach (var parameter in mainArmTemplateFile.Parameters)
            {
                if (parameterInstancesByName.TryGetValue(parameter.Name, out var existingInstance))
                {
                    mergedParameterInstances.Add(existingInstance);
                }
                else if (parameter.Required)
                {
                    mergedParameterInstances.Add(GenerateDummyParameterInstance(parameter));
                }
            }

            return mergedParameterInstances;
        }

        private static MainArmTemplateParameterInstance GenerateDummyParameterInstance(MainArmTemplateParameter parameter)
        {
            var name = parameter.Name;
            object value = parameter.Type.ToLowerInvariant() switch
            {
                "bool" => false,
                "int" => 0,
                "string" or "securestring" => JsonElementFactory.CreateElement(@"[ """" ]")[0],
                "object" or "secureobject" => JsonElementFactory.CreateElement("{}"),
                "array" => JsonElementFactory.CreateElement("[]"),

                _ => throw new InvalidOperationException($"Unknown parameter type \"{parameter.Type}\""),
            };

            return new(name, value);
        }

        private MainArmTemplateParameterInstance ToParameterInstance(JsonProperty parameterInstanceProperty)
        {
            var name = parameterInstanceProperty.Name;
            var value = parameterInstanceProperty.Value.GetProperty("value");

            return new(name, value);
        }
    }
}
