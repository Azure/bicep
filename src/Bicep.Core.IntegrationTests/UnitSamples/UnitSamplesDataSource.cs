using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.UnitSamples
{
    public class UnitSamplesDataSource: Attribute, ITestDataSource
    {
        private static readonly string Prefix = typeof(UnitSamplesDataSource).Namespace! + '.';

        private readonly bool includeValid;
        private readonly bool includeInvalid;

        public UnitSamplesDataSource(bool includeValid = true, bool includeInvalid = true)
        {
            this.includeValid = includeValid;
            this.includeInvalid = includeInvalid;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            // this needs to be explicit so it's easy to comment out tests
            // as mstest v2 does not allow you to directly choose which data row
            // will be run or debugged
            var rows = new List<object[]>();

            if (includeValid)
            {
                rows.Add(CreateDataRow("Valid.Parameters.arm"));
            }

            if (includeInvalid)
            {
                rows.Add(CreateDataRow("Invalid.InvalidParameters.arm"));
            }

            return rows;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            data.Length.Should().Be(2, "because dynamic data should consist of display name and ARM file contents");
            data[0].Should().BeOfType<string>("because the first data row item should be a string");
            
            return (string)data[0];
        }

        private static object[] CreateDataRow(string suffix) => new object[] {suffix, ReadFile(Prefix + suffix)};

        private static string ReadFile(string streamName)
        {
            using var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(streamName) ?? Stream.Null);
            string contents = reader.ReadToEnd();
            contents.Should().NotBeEmpty($"because stream '{streamName}' should not be empty");

            return contents;
        }
    }
}
