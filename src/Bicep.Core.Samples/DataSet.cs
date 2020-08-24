using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using FluentAssertions;

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
        public const string TestFileMainCompiled = "main.json";

        private static readonly string Prefix = typeof(DataSet).Namespace == null ? string.Empty : typeof(DataSet).Namespace + '.';

        private readonly Lazy<string> lazyBicep;

        private readonly Lazy<string> lazyTokens;

        private readonly Lazy<string> lazyDiagnostics;

        private readonly Lazy<string>? lazyCompiled;

        private readonly Lazy<string> lazySyntax;

        private readonly Lazy<string> lazySymbols;

        public DataSet(string name)
        {
            this.Name = name;

            this.lazyBicep = this.CreateRequired(TestFileMain);
            this.lazyTokens = this.CreateRequired(TestFileMainTokens);
            this.lazyDiagnostics = this.CreateRequired(TestFileMainDiagnostics);
            this.lazyCompiled = this.CreateIffValid(TestFileMainCompiled);
            this.lazySymbols = this.CreateRequired(TestFileMainSymbols);
            this.lazySyntax = this.CreateRequired(TestFileMainSyntax);
        }

        public string Name { get; }

        public string DisplayName => this.Name;

        public string Bicep => this.lazyBicep.Value;

        public string Tokens => this.lazyTokens.Value;

        public string Diagnostics => this.lazyDiagnostics.Value;

        public string? Compiled => this.lazyCompiled?.Value;

        public string Symbols => this.lazySymbols.Value;

        public string Syntax => this.lazySyntax.Value;

        // validity is set by naming convention

        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

        private Lazy<string> CreateRequired(string fileName)
        {
            return new Lazy<string>(() => this.ReadDataSetFile(fileName), LazyThreadSafetyMode.PublicationOnly);
        }

        private Lazy<string>? CreateIffValid(string fileName) => this.IsValid ? this.CreateRequired(fileName) : null;

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((DataSet) data[0]).Name}";

        private string ReadDataSetFile(string fileName) => ReadFile($"{Prefix}{this.Name}.{fileName}");

        public static string ReadFile(string streamName)
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
            stream.Should().NotBeNull($"because stream '{streamName}' should exist");

            using var reader = new StreamReader(stream ?? Stream.Null);

            return reader.ReadToEnd();
        }
    }
}