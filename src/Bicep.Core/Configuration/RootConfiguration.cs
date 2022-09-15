// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using Bicep.Core.Diagnostics;

namespace Bicep.Core.Configuration
{
    public class RootConfiguration
    {
        public RootConfiguration(CloudConfiguration cloud, ModuleAliasesConfiguration moduleAliases, AnalyzersConfiguration analyzers, string? configurationPath, IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate>? diagnosticBuilders)
        {
            this.Cloud = cloud;
            this.ModuleAliases = moduleAliases;
            this.Analyzers = analyzers;
            this.ConfigurationPath = configurationPath;
            this.DiagnosticBuilders = diagnosticBuilders?.ToImmutableArray() ?? ImmutableArray<DiagnosticBuilder.DiagnosticBuilderDelegate>.Empty;
        }

        public static RootConfiguration Bind(JsonElement element, string? configurationPath = null, IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate>? diagnosticBuilders = default)
        {
            var cloud = CloudConfiguration.Bind(element.GetProperty("cloud"), configurationPath);
            var moduleAliases = ModuleAliasesConfiguration.Bind(element.GetProperty("moduleAliases"), configurationPath);
            var analyzers = new AnalyzersConfiguration(element.GetProperty("analyzers"));

            return new(cloud, moduleAliases, analyzers, configurationPath, diagnosticBuilders);
        }

        public CloudConfiguration Cloud { get; }

        public ModuleAliasesConfiguration ModuleAliases { get; }

        public AnalyzersConfiguration Analyzers { get; }

        public string? ConfigurationPath { get; }

        public IEnumerable<DiagnosticBuilder.DiagnosticBuilderDelegate> DiagnosticBuilders { get; }

        public bool IsBuiltIn => ConfigurationPath is null;

        public string ToUtf8Json()
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using (var writer = new Utf8JsonWriter(bufferWriter, new() { Indented = true }))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("cloud");
                this.Cloud.WriteTo(writer);

                writer.WritePropertyName("moduleAliases");
                this.ModuleAliases.WriteTo(writer);

                writer.WritePropertyName("analyzers");
                this.Analyzers.WriteTo(writer);

                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }
    }
}
