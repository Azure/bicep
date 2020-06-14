using System;
using System.IO;
using System.Reflection;
using FluentAssertions;

namespace Bicep.Core.Samples
{
    public class DataSet
    {
        private static readonly string Prefix = typeof(DataSet).Namespace == null ? string.Empty : typeof(DataSet).Namespace + '.';

        public DataSet(string name)
        {
            this.Name = name;
        }

        public string Name { get; }

        public string Bicep => ReadDataSetFile("Bicep.arm");

        // validity is set by naming convention
        public bool IsValid => this.Name.Contains("Invalid", StringComparison.Ordinal) == false;

        public static string GetDisplayName(MethodInfo info, object[] data) => $"{info.Name}_{((DataSet) data[0]).Name}";

        private string ReadDataSetFile(string fileName) => ReadFile($"{Prefix}{Name}.{fileName}");

        private static string ReadFile(string streamName)
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName);
            stream.Should().NotBeNull($"because stream '{streamName}' should exist");

            using var reader = new StreamReader(stream ?? Stream.Null);
            string contents = reader.ReadToEnd();
            
            return contents;
        }
    }
}