// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Tracing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bicep.Core.UnitTests.Tracing
{
    [TestClass]
    public class AzureEventSourceListenerFactoryTests
    {
        private static readonly ReadOnlyCollection<string> SampleNamesWithHeaders = new List<string>
        {
            "one",
            "headers",
            "two"
        }.AsReadOnly();

        private static readonly ReadOnlyCollection<object> SampleValuesWithHeaders = new List<object>
        {
            1,
            "one: two\r\nthree: four",
            "two"
        }.AsReadOnly();

        private static readonly ReadOnlyCollection<string> SampleNamesWithoutHeaders = new List<string>
        {
            "one",
            "notHeaders",
            "two"
        }.AsReadOnly();

        private static readonly ReadOnlyCollection<object> SampleValuesWithoutHeaders = new List<object>
        {
            1,
            "something",
            "two"
        }.AsReadOnly();

        private const string FormatString = "{0}-->{1}<--{2}";


        private static readonly string AzureCoreEventSource = "Azure-Core";

        [TestMethod]
        public void FullVerbosityShouldReturnNull()
        {
            AzureEventSourceListenerFactory.GetFormattedMessageWithoutHeaders(
                Features.TraceVerbosity.Full,
                AzureCoreEventSource,
                FormatString,
                SampleNamesWithHeaders,
                SampleValuesWithHeaders).Should().BeNull();
        }

        [TestMethod]
        public void NonAzureCoreShouldReturnNull()
        {
            AzureEventSourceListenerFactory.GetFormattedMessageWithoutHeaders(
                Features.TraceVerbosity.Basic,
                "not-azure-core",
                FormatString,
                SampleNamesWithHeaders,
                SampleValuesWithHeaders).Should().BeNull();
        }

        [TestMethod]
        public void NonHeaderEventShouldReturnNull()
        {
            AzureEventSourceListenerFactory.GetFormattedMessageWithoutHeaders(
                Features.TraceVerbosity.Basic,
                AzureCoreEventSource,
                FormatString,
                SampleNamesWithoutHeaders,
                SampleValuesWithoutHeaders).Should().BeNull();
        }

        [TestMethod]
        public void HeadersShouldBeRemoved()
        {
            AzureEventSourceListenerFactory.GetFormattedMessageWithoutHeaders(
                Features.TraceVerbosity.Basic,
                AzureCoreEventSource,
                FormatString,
                SampleNamesWithHeaders,
                SampleValuesWithHeaders).Should().Be("1--><--two");
        }
    }
}
