using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Tests.UnitSamples
{
    public class UnitSamplesDataSource: Attribute, ITestDataSource
    {
        private static readonly string Prefix = typeof(UnitSamplesDataSource).Namespace! + '.';

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            // this needs to be explicit so it's easy to comment out tests
            // as mstest v2 does not allow you to directly choose with data row
            // will be run or debugged
            return new List<object[]>
            {
                CreateDataRow("Inputs.Parameters.arm")
            };
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
