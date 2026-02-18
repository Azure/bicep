// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Bicep.Core.Samples
{
    [DebuggerDisplay("{" + nameof(DisplayName) + "}")]
    public class DataSet
    {
        public const string TestFileMain = "main.bicep";
        public const string TestFileMainDiagnostics = "main.diagnostics.bicep";
        public const string TestFileMainIr = "main.ir.bicep";
        public const string TestFileMainTokens = "main.tokens.bicep";
        public const string TestFileMainSymbols = "main.symbols.bicep";
        public const string TestFileMainSyntax = "main.syntax.bicep";
        public const string TestFileMainFormatted = "main.formatted.bicep";
        public const string TestFileMainSourceMap = "main.sourcemap.bicep";
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
        public const string TestArchiveDirectory = "archive";
        public const string TestArchivePrefix = TestArchiveDirectory + "/";
        public const string TestOciArchiveDirectory = "ociArchive";
        public const string TestOciArchivePrefix = TestOciArchiveDirectory + "/";

        public record ExternalModuleInfo(string ModuleSource, ExternalModuleMetadata Metadata);

        public record ExternalModuleMetadata(string Target);

        public static readonly string Prefix = "Files/baselines/";

        private readonly Lazy<string> lazyBicep;

        private readonly Lazy<string> lazyTokens;

        private readonly Lazy<string> lazyDiagnostics;

        private readonly Lazy<string>? lazyIr;

        private readonly Lazy<string>? lazyCompiled;

        private readonly Lazy<string>? lazyCompiledWithSymbolicNames;

        private readonly Lazy<string> lazySyntax;

        private readonly Lazy<string> lazySymbols;

        private readonly Lazy<string> lazyFormatted;

        private readonly Lazy<string>? lazySourceMap;

        private readonly Lazy<ImmutableDictionary<string, string>> lazyCompletions;

        private readonly Lazy<ImmutableDictionary<string, ExternalModuleInfo>> lazyModulesToPublish;

        private readonly Lazy<ImmutableDictionary<string, ExternalModuleInfo>> lazyTemplateSpecs;

        private readonly Lazy<ImmutableDictionary<string, string>>? lazyArchive;

        private readonly Lazy<ImmutableDictionary<string, string>>? lazyOciArchive;

        public DataSet(string name)
        {
            this.Name = name;

            this.lazyBicep = this.CreateRequired(TestFileMain);
            this.lazyTokens = this.CreateRequired(TestFileMainTokens);
            this.lazyDiagnostics = this.CreateRequired(TestFileMainDiagnostics);
            this.lazyIr = this.CreateIffValid(TestFileMainIr);
            this.lazyCompiled = this.CreateIffValid(TestFileMainCompiled);
            this.lazyCompiledWithSymbolicNames = this.CreateIffValid(TestFileMainCompiledWithSymbolicNames);
            this.lazySymbols = this.CreateRequired(TestFileMainSymbols);
            this.lazySyntax = this.CreateRequired(TestFileMainSyntax);
            this.lazyFormatted = this.CreateRequired(TestFileMainFormatted);
            this.lazySourceMap = this.CreateIffValid(TestFileMainSourceMap);
            this.lazyCompletions = new(() => ReadDataSetDictionary(GetStreamName(TestCompletionsPrefix)), LazyThreadSafetyMode.PublicationOnly);
            this.lazyModulesToPublish = new(() => ReadPublishData(GetStreamName(TestPublishPrefix)), LazyThreadSafetyMode.PublicationOnly);
            this.lazyTemplateSpecs = new(() => ReadTemplateSpecsData(GetStreamName(TestTemplateSpecsPrefix)), LazyThreadSafetyMode.PublicationOnly);
            this.lazyArchive = this.CreateDirectoryIfValid(TestArchivePrefix);
            this.lazyOciArchive = this.CreateDirectoryIfValid(TestOciArchivePrefix);
        }

        public string Name { get; }

        public string DisplayName => this.Name;

        public string Bicep => this.lazyBicep.Value;

        public string Tokens => this.lazyTokens.Value;

        public string Diagnostics => this.lazyDiagnostics.Value;

        public string? Ir => this.lazyIr?.Value;

        public string? Compiled => this.lazyCompiled?.Value;

        public string? CompiledWithSymbolicNames => this.lazyCompiledWithSymbolicNames?.Value;

        public string Symbols => this.lazySymbols.Value;

        public string Syntax => this.lazySyntax.Value;

        public string Formatted => this.lazyFormatted.Value;

        public string? SourceMap => this.lazySourceMap?.Value;

        public ImmutableDictionary<string, string> Completions => this.lazyCompletions.Value;

        public ImmutableDictionary<string, ExternalModuleInfo> RegistryModules => this.lazyModulesToPublish.Value;

        public ImmutableDictionary<string, ExternalModuleInfo> TemplateSpecs => this.lazyTemplateSpecs.Value;

        public ImmutableDictionary<string, string>? Archive => this.lazyArchive?.Value;

        public ImmutableDictionary<string, string>? OciArchive => this.lazyOciArchive?.Value;

        public bool HasRegistryModules => !this.RegistryModules.IsEmpty;

        public bool HasTemplateSpecs => !this.TemplateSpecs.IsEmpty;

        public bool HasExternalModules => this.HasRegistryModules || this.HasTemplateSpecs;

        // validity is set by naming convention

        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

        public bool IsStress => this.Name.Contains("Stress", StringComparison.Ordinal);

        private Lazy<string> CreateRequired(string fileName)
        {
            return new Lazy<string>(() => this.ReadDataSetFile(fileName), LazyThreadSafetyMode.PublicationOnly);
        }

        private Lazy<string>? CreateIffValid(string fileName) => this.IsValid ? this.CreateRequired(fileName) : null;

        private Lazy<ImmutableDictionary<string, string>>? CreateDirectoryIfValid(string streamNamePrefix)
            => this.IsValid ? new(() => ReadDataSetDictionary(GetStreamName(streamNamePrefix)), LazyThreadSafetyMode.PublicationOnly) : null;

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((DataSet)data[0]).Name}";

        public string ReadDataSetFile(string fileName) => ReadFile(GetStreamName(fileName));

        private string GetStreamName(string fileName) => $"{GetStreamPrefix()}/{fileName}";

        public string GetStreamPrefix() => $"{Prefix}{this.Name}";

        public static string ReadFile(string streamName)
            => FileHelper.ReadEmbeddedFile(Assembly.GetExecutingAssembly(), streamName);

        public static ImmutableDictionary<string, string> ReadDataSetDictionary(string streamNamePrefix)
            => FileHelper.BuildEmbeddedFileDictionary(Assembly.GetExecutingAssembly(), streamNamePrefix);

        public static string AddDiagsToSourceText<T>(DataSet dataSet, IEnumerable<T> items, Func<T, TextSpan> getSpanFunc, Func<T, string> diagsFunc)
            => OutputHelper.AddDiagsToSourceText<T>(dataSet.Bicep, dataSet.HasCrLfNewlines() ? "\r\n" : "\n", items, getSpanFunc, diagsFunc);

        public static string AddDiagsToSourceText<TPositionable>(DataSet dataSet, IEnumerable<TPositionable> items, Func<TPositionable, string> diagsFunc)
            where TPositionable : IPositionable
            => OutputHelper.AddDiagsToSourceText(dataSet.Bicep, dataSet.HasCrLfNewlines() ? "\r\n" : "\n", items, item => item.Span, diagsFunc);

        public static string GetBaselineUpdatePath(params string[] fileNames)
            => Path.Combine("src", "Bicep.Core.Samples", "Files", "baselines", Path.Combine(fileNames));

        public static string GetBaselineUpdatePath(DataSet dataSet, string fileName)
            => GetBaselineUpdatePath(dataSet.Name, fileName);

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

                if (!rawFiles.TryGetValue(moduleSourceName, out var moduleSource))
                {
                    throw new AssertFailedException($"The module source file '{moduleSourceName}' is missing.");
                }

                if (!rawFiles.TryGetValue(moduleMetadataName, out var moduleMetadataText))
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
