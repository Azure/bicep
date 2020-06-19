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

        public DataSet(string name)
        {
            this.Name = name;

            this.lazyBicep = new Lazy<string>(() => this.ReadDataSetFile("Bicep.arm"), LazyThreadSafetyMode.PublicationOnly);
            this.lazyTokens = new Lazy<string>(()=> this.ReadDataSetFile("Tokens.json"), LazyThreadSafetyMode.PublicationOnly);
            this.lazyErrors = new Lazy<string>(() => this.ReadDataSetFile("Errors.json"), LazyThreadSafetyMode.PublicationOnly);
        }

        public string Name { get; }

        public string DisplayName => this.Name;

        public string Bicep => this.lazyBicep.Value;

        public string Tokens => this.lazyTokens.Value;

        public string Errors => this.lazyErrors.Value;

        // validity is set by naming convention
        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

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