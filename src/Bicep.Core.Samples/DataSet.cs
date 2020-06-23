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
        private static readonly string Prefix = typeof(DataSet).Namespace == null ? string.Empty : typeof(DataSet).Namespace + '.';

        private readonly Lazy<string> lazyBicep;

        private readonly Lazy<string> lazyTokens;

        private readonly Lazy<string> lazyErrors;

        private readonly Lazy<string>? lazyCompiled;

        public DataSet(string name)
        {
            this.Name = name;

            this.lazyBicep = this.CreateRequired("Bicep.arm");
            this.lazyTokens = this.CreateRequired("Tokens.json");
            this.lazyErrors = this.CreateRequired("Errors.json");
            this.lazyCompiled = this.CreateIffValid("Compiled.json");
        }

        public string Name { get; }

        public string DisplayName => this.Name;

        public string Bicep => this.lazyBicep.Value;

        public string Tokens => this.lazyTokens.Value;

        public string Errors => this.lazyErrors.Value;

        public string? Compiled => this.lazyCompiled?.Value;

        // validity is set by naming convention

        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

        private Lazy<string> CreateRequired(string fileName)
        {
            return new Lazy<string>(() => this.ReadDataSetFile(fileName), LazyThreadSafetyMode.PublicationOnly);
        }

        private Lazy<string>? CreateIffValid(string fileName) => this.IsValid ? this.CreateRequired(fileName) : null;

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((DataSet) data[0]).Name}";

        private string ReadDataSetFile(string fileName) => ReadFile($"{Prefix}{this.Name}.{fileName}");

        private static string ReadFile(string streamName)
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
            stream.Should().NotBeNull($"because stream '{streamName}' should exist");

            using var reader = new StreamReader(stream ?? Stream.Null);

            return reader.ReadToEnd();
        }
    }
}