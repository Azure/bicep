// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Azure;
using System.Formats.Tar;
using System.IO.Compression;
using System.IO;

namespace Bicep.Core.IntegrationTests.TestDataGenerators;

public static class DynamicAzTypesTestDataGenerator
{
    public record ArtifactRegistryAddress(string RegistryAddress, string RepositoryPath, string ProviderVersion)
    {
        public string ToSpecificationString(char delim) => $"br:{RegistryAddress}/{RepositoryPath}{delim}{ProviderVersion}";

        public (string, string) ClientDescriptor() => (RegistryAddress, RepositoryPath);
    }
    public static IEnumerable<object[]> ArtifactRegistryAddressNegativeTestScenarios()
    {
        // constants
        const string placeholderProviderVersion = "0.0.0-placeholder";

        // unresolvable host registry. For example if DNS is down or unresponsive
        const string unreachableRegistryAddress = "unknown.registry.azurecr.io";
        const string NoSuchHostMessage = $" (No such host is known. ({unreachableRegistryAddress}:443))";
        var AggregateExceptionMessage = $"Retry failed after 4 tries. Retry settings can be adjusted in ClientOptions.Retry or by configuring a custom retry policy in ClientOptions.RetryPolicy.{string.Concat(Enumerable.Repeat(NoSuchHostMessage, 4))}";
        var unreacheable = new ArtifactRegistryAddress(unreachableRegistryAddress, "bicep/providers/az", placeholderProviderVersion);
        yield return new object[] {
                unreacheable,
                new AggregateException(AggregateExceptionMessage),
                new (string, DiagnosticLevel, string)[]{
                    ("BCP192", DiagnosticLevel.Error, @$"Unable to restore the artifact with reference ""{unreacheable.ToSpecificationString(':')}"": {AggregateExceptionMessage}"),
                    ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
                },
            };

        // manifest not found is thrown when the repository address is not registered and/or the version doesn't exist in the registry
        const string NotFoundMessage = "The artifact does not exist in the registry.";
        var withoutRepo = new ArtifactRegistryAddress(LanguageConstants.BicepPublicMcrRegistry, "unknown/path/az", placeholderProviderVersion);
        yield return new object[] {
                withoutRepo,
                new RequestFailedException(404, NotFoundMessage),
                new (string, DiagnosticLevel, string)[]{
                    ("BCP192", DiagnosticLevel.Error, $@"Unable to restore the artifact with reference ""{withoutRepo.ToSpecificationString(':')}"": {NotFoundMessage}"),
                    ("BCP084", DiagnosticLevel.Error, "The symbolic name \"az\" is reserved. Please use a different symbolic name. Reserved namespaces are \"az\", \"sys\".")
                },
            };
    }

    public static IEnumerable<object[]> ArtifactRegistryCorruptedPackageNegativeTestScenarios()
    {
        // Scenario: An empty Stream
        //yield return new object[]
        //{
        //    new MemoryStream(),
        //    "Unable to read beyond the end of the stream."
        //};

        // Scenario: Artifact layer payload is not a .tgz file


        using var fileStream = new FileStream("test.txt", FileMode.Create);
        fileStream.Write(Encoding.ASCII.GetBytes("NotAGZip"));
        fileStream.Seek(0, SeekOrigin.Begin);
        yield return new object[]
        {
                fileStream,
                "BareStreamWithTextOnIt"
        };

        // Scenario: Artifact layer payload is a Gzip but not a Tar
        var stream = new MemoryStream(Encoding.ASCII.GetBytes("NotAGZip"));
        var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);
        stream.Seek(0, SeekOrigin.Begin);
        yield return new object[]
        {
                gzStream,
                "NotATar"
        };

        // Scenario: Artifact layer payload is missing an "index.json"
        stream = new MemoryStream();
        AddFileEntries(stream, ("unknown.json", "{}"));
        stream.Seek(0, SeekOrigin.Begin);
        yield return new object[]
        {
                stream,
                "NoIndex"
        };

        // Scenario: "index.json" is not valid JSON
        stream = new MemoryStream();
        AddFileEntries(stream, ("index.json", @"{""INVALID_JSON"": 777"));
        stream.Seek(0, SeekOrigin.Begin);
        yield return new object[]
        {
                stream,
                "InvalidJson"
        };
        // Scenario: "index.json" with wrong data
        // Scenario: "index.json" missing required data
        // Scenario: "index.json" references a missing "types.json" file

        // Scenario: "types.json" is not valid JSON
        // Scenario: "types.json" with wrong data
        // Scenario: "types.json" missing required data

        //FileHelper.GetUniqueTestOutputPath(TestContext);

    }

    private static void AddFileEntries(Stream stream, params (string filePath, string contents)[] files)
    {
        foreach (var entry in files)
        {
            using var gzStream = new GZipStream(stream, CompressionMode.Compress, leaveOpen: true);
            using var tarWriter = new TarWriter(gzStream, leaveOpen: true);
            var tarEntry = new PaxTarEntry(TarEntryType.RegularFile, "unkown.json")
            {
                DataStream = new MemoryStream(Encoding.UTF8.GetBytes("{}"))
            };
            tarWriter.WriteEntry(tarEntry);
        }
    }

}
