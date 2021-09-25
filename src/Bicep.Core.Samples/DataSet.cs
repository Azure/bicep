// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Bicep.Core.Samples
{
    [DebuggerDisplay("{" + nameof(DisplayName) + "}")]
    public class DataSet
    {
        public const string TestFileMain = "main.bicep";
        public const string TestFileMainDiagnostics = "main.diagnostics.bicep";
        public const string TestFileMainTokens = "main.tokens.bicep";
        public const string TestFileMainSymbols = "main.symbols.bicep";
        public const string TestFileMainSyntax = "main.syntax.bicep";
        public const string TestFileMainFormatted = "main.formatted.bicep";
        public const string TestFileMainCompiled = "main.json";
        public const string TestFileMainCompiledWithSymbolicNames = "main.symbolicnames.json";
        public const string TestCompletionsDirectory = "Completions";
        public const string TestCompletionsPrefix = TestCompletionsDirectory + "/";
        public const string TestFunctionsDirectory = "Functions";
        public const string TestFunctionsPrefix = TestFunctionsDirectory + "/";
        public const string TestPublishDirectory = "Publish";
        public const string TestPublishPrefix = TestPublishDirectory + "/";
        public const string TestTemplateSpecsDirectory = "TemplateSpecs";
        public const string TestTemplateSpecsPrefix = TestTemplateSpecsDirectory + "/";

        public record ExternalModuleInfo(string ModuleSource, ExternalModuleMetadata Metadata);

        public record ExternalModuleMetadata(string Target);

        public static readonly string Prefix = typeof(DataSet).Namespace == null ? string.Empty : typeof(DataSet).Namespace + '/';

        private readonly Lazy<string> lazyBicep;

        private readonly Lazy<string> lazyTokens;

        private readonly Lazy<string> lazyDiagnostics;

        private readonly Lazy<string>? lazyCompiled;

        private readonly Lazy<string>? lazyCompiledWithSymbolicNames;

        private readonly Lazy<string> lazySyntax;

        private readonly Lazy<string> lazySymbols;

        private readonly Lazy<string> lazyFormatted;

        private readonly Lazy<ImmutableDictionary<string, string>> lazyCompletions;

        private readonly Lazy<ImmutableDictionary<string, ExternalModuleInfo>> lazyModulesToPublish;

        private readonly Lazy<ImmutableDictionary<string, ExternalModuleInfo>> lazyTemplateSpecs;

        public DataSet(string name)
        {
            this.Name = name;

            this.lazyBicep = this.CreateRequired(TestFileMain);
            this.lazyTokens = this.CreateRequired(TestFileMainTokens);
            this.lazyDiagnostics = this.CreateRequired(TestFileMainDiagnostics);
            this.lazyCompiled = this.CreateIffValid(TestFileMainCompiled);
            this.lazyCompiledWithSymbolicNames = this.CreateIffValid(TestFileMainCompiledWithSymbolicNames);
            this.lazySymbols = this.CreateRequired(TestFileMainSymbols);
            this.lazySyntax = this.CreateRequired(TestFileMainSyntax);
            this.lazyFormatted = this.CreateRequired(TestFileMainFormatted);
            this.lazyCompletions = new(() => ReadDataSetDictionary(GetStreamName(TestCompletionsPrefix)), LazyThreadSafetyMode.PublicationOnly);
            this.lazyModulesToPublish = new(() => ReadPublishData(GetStreamName(TestPublishPrefix)), LazyThreadSafetyMode.PublicationOnly);
            this.lazyTemplateSpecs = new(() => ReadTemplateSpecsData(GetStreamName(TestTemplateSpecsPrefix)), LazyThreadSafetyMode.PublicationOnly);
        }

        public string Name { get; }

        public string DisplayName => this.Name;

        public string Bicep => this.lazyBicep.Value;

        public string Tokens => this.lazyTokens.Value;

        public string Diagnostics => this.lazyDiagnostics.Value;

        public string? Compiled => this.lazyCompiled?.Value;

        public string? CompiledWithSymbolicNames => this.lazyCompiledWithSymbolicNames?.Value;

        public string Symbols => this.lazySymbols.Value;

        public string Syntax => this.lazySyntax.Value;

        public string Formatted => this.lazyFormatted.Value;

        public ImmutableDictionary<string, string> Completions => this.lazyCompletions.Value;

        public ImmutableDictionary<string, ExternalModuleInfo> RegistryModules => this.lazyModulesToPublish.Value;

        public ImmutableDictionary<string, ExternalModuleInfo> TemplateSpecs => this.lazyTemplateSpecs.Value;

        public bool HasRegistryModules => this.RegistryModules.Any();

        public bool HasTemplateSpecs => this.TemplateSpecs.Any();

        public bool HasExternalModules => this.HasRegistryModules || this.HasTemplateSpecs;

        // validity is set by naming convention

        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

        public bool IsStress => this.Name.Contains("Stress", StringComparison.Ordinal);

        private Lazy<string> CreateRequired(string fileName)
        {
            return new Lazy<string>(() => this.ReadDataSetFile(fileName), LazyThreadSafetyMode.PublicationOnly);
        }

        private Lazy<string>? CreateIffValid(string fileName) => this.IsValid ? this.CreateRequired(fileName) : null;

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((DataSet) data[0]).Name}";

        private string ReadDataSetFile(string fileName) => ReadFile(GetStreamName(fileName));

        private string GetStreamName(string fileName) => $"{GetStreamPrefix()}/{fileName}";

        public string GetStreamPrefix() => $"{Prefix}{this.Name}";

        public static string ReadFile(string streamName)
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
            stream.Should().NotBeNull($"because stream '{streamName}' should exist");

            using var reader = new StreamReader(stream ?? Stream.Null);

            return reader.ReadToEnd();
        }

        public static ImmutableDictionary<string, string> ReadDataSetDictionary(string streamNamePrefix)
        {
            var matches = Assembly.GetExecutingAssembly()
                .GetManifestResourceNames()
                .Where(streamName => streamName.StartsWith(streamNamePrefix, StringComparison.Ordinal))
                .Select(streamName => (streamName, key: streamName.Substring(streamNamePrefix.Length)));

            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var (streamName, key) in matches)
            {
                builder.Add(key, ReadFile(streamName));
            }

            return builder.ToImmutable();
        }

        public static string AddDiagsToSourceText<T>(DataSet dataSet, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc)
            => OutputHelper.AddDiagsToSourceText<T>(dataSet.Bicep, dataSet.HasCrLfNewlines() ? "\r\n" : "\n", items, getSpanFunc, diagsFunc);

        public static string AddDiagsToSourceText<TPositionable>(DataSet dataSet, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc)
            where TPositionable : IPositionable
            => OutputHelper.AddDiagsToSourceText(dataSet.Bicep, dataSet.HasCrLfNewlines() ? "\r\n" : "\n", items, item => item.Span, diagsFunc);

        public static string GetBaselineUpdatePath(DataSet dataSet, string fileName)
            => Path.Combine("src", "Bicep.Core.Samples", "Files", dataSet.Name, fileName);

        private static ImmutableDictionary<string, ExternalModuleInfo> ReadPublishData(string streamNamePrefix) =>
            ReadExternalModuleData(streamNamePrefix, LanguageConstants.LanguageFileExtension);

        private static ImmutableDictionary<string, ExternalModuleInfo> ReadTemplateSpecsData(string streamNamePrefix) =>
            ReadExternalModuleData(streamNamePrefix, LanguageConstants.JsonFileExtension);

        private static ImmutableDictionary<string, ExternalModuleInfo> ReadExternalModuleData(string streamNamePrefix, string moduleExtension)
        {
            static string GetModuleName(string fileName)
            {
                var parts = fileName.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    throw new InvalidOperationException($"File '{fileName}' doesn't match the expected naming convention.");
                }

                return parts[0];
            }

            var rawFiles = ReadDataSetDictionary(streamNamePrefix);
            var uniqueModulesToPublish = rawFiles.Keys.Select(GetModuleName).Distinct();

            var builder = ImmutableDictionary.CreateBuilder<string, ExternalModuleInfo>();
            foreach (var moduleName in uniqueModulesToPublish)
            {
                var moduleSourceName = $"{moduleName}{moduleExtension}";
                var moduleMetadataName = $"{moduleName}.metadata.json";

                if(!rawFiles.TryGetValue(moduleSourceName, out var moduleSource))
                {
                    throw new AssertFailedException($"The module source file '{moduleSourceName}' is missing.");
                }

                if(!rawFiles.TryGetValue(moduleMetadataName, out var moduleMetadataText))
                {
                    throw new AssertFailedException($"The module metadata file '{moduleMetadataName}' is missing.");
                }

                var metadata = JsonConvert.DeserializeObject<ExternalModuleMetadata>(moduleMetadataText) ?? throw new AssertFailedException($"Module metadata file '{moduleMetadataName}' deserialized into a null object.");

                builder.Add(moduleName, new ExternalModuleInfo(moduleSource, metadata));
            }

            return builder.ToImmutable();
        }
    }
}
