// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bicep.Core.Parser;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public const string TestCompletionsPrefix = TestCompletionsDirectory + "/";
        public const string TestCompletionsDirectory = "Completions";

        public static readonly string Prefix = typeof(DataSet).Namespace == null ? string.Empty : typeof(DataSet).Namespace + '/';

        private readonly Lazy<string> lazyBicep;

        private readonly Lazy<string> lazyTokens;

        private readonly Lazy<string> lazyDiagnostics;

        private readonly Lazy<string>? lazyCompiled;

        private readonly Lazy<string> lazySyntax;

        private readonly Lazy<string> lazySymbols;

        private readonly Lazy<string> lazyFormatted;

        private readonly Lazy<ImmutableDictionary<string, string>> lazyCompletions;

        public DataSet(string name)
        {
            this.Name = name;

            this.lazyBicep = this.CreateRequired(TestFileMain);
            this.lazyTokens = this.CreateRequired(TestFileMainTokens);
            this.lazyDiagnostics = this.CreateRequired(TestFileMainDiagnostics);
            this.lazyCompiled = this.CreateIffValid(TestFileMainCompiled);
            this.lazySymbols = this.CreateRequired(TestFileMainSymbols);
            this.lazySyntax = this.CreateRequired(TestFileMainSyntax);
            this.lazyFormatted = this.CreateRequired(TestFileMainFormatted);
            this.lazyCompletions = new Lazy<ImmutableDictionary<string, string>>(() => ReadDataSetDictionary(GetStreamName(TestCompletionsPrefix)), LazyThreadSafetyMode.PublicationOnly);
        }

        public string Name { get; }

        public string DisplayName => this.Name;

        public string Bicep => this.lazyBicep.Value;

        public string Tokens => this.lazyTokens.Value;

        public string Diagnostics => this.lazyDiagnostics.Value;

        public string? Compiled => this.lazyCompiled?.Value;

        public string Symbols => this.lazySymbols.Value;

        public string Syntax => this.lazySyntax.Value;

        public string Formatted => this.lazyFormatted.Value;

        public ImmutableDictionary<string, string> Completions => this.lazyCompletions.Value;

        // validity is set by naming convention

        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

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
    }
}
