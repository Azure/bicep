// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Baselines;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Bicep.LangServer.UnitTests.Snippets;

[TestClass]
public class SnippetCacheTests
{
    [NotNull]
    public TestContext? TestContext { get; set; }

    private SnippetCacheBuilder CreateSnippetCacheBuilder()
        => ServiceBuilder.Create(s => s.AddSingleton<SnippetCacheBuilder>()).Construct<SnippetCacheBuilder>();

    [TestMethod]
    [TestCategory(BaselineHelper.BaselineTestCategory)]
    public async Task Verify_snippet_cache()
    {
        var baselineFolder = BaselineFolder.BuildOutputFolder(TestContext, new EmbeddedFile(typeof(SnippetCache).Assembly, "Files/SnippetCache.json"));
        var baselineFile = baselineFolder.EntryFile;

        var snippetCache = await CreateSnippetCacheBuilder().Build();

        baselineFile.WriteToOutputFolder(SnippetCache.Serialize(snippetCache));
        baselineFile.ShouldHaveExpectedJsonValue();

        // If the baseline has been updated, then verify that the FromManifest method gives us the same result.
        var fromManifest = SnippetCache.FromManifest();
        SnippetCache.Serialize(snippetCache).Should().Be(SnippetCache.Serialize(fromManifest));
    }

    [TestMethod]
    public async Task GetDescriptionAndSnippetText_WithEmptyInput_ReturnsEmptyDescriptionAndText()
    {
        (string description, string text) = await CreateSnippetCacheBuilder().GetDescriptionAndSnippetText(string.Empty, @"C:\foo.bicep");

        description.Should().Be(string.Empty);
        text.Should().Be(string.Empty);
    }

    [TestMethod]
    public async Task GetDescriptionAndSnippetText_WithOnlyWhitespaceInput_ReturnsEmptyDescriptionAndText()
    {
        (string description, string text) = await CreateSnippetCacheBuilder().GetDescriptionAndSnippetText("   ", @"C:\foo.bicep");

        description.Should().Be(string.Empty);
        text.Should().Be(string.Empty);
    }

    [TestMethod]
    public async Task GetDescriptionAndSnippetText_WithValidInput_ReturnsDescriptionAndText()
    {
        string template = @"// DNS Zone
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

        (string description, string text) = await CreateSnippetCacheBuilder().GetDescriptionAndSnippetText(template, @"C:\foo.bicep");

        string expectedText = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

        description.Should().Be("DNS Zone");
        expectedText.Should().Be(text);
    }

    [TestMethod]
    public async Task GetDescriptionAndSnippetText_WithMissingCommentInInput_ReturnsEmptyDescriptionAndValidText()
    {
        string template = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

        (string description, string text) = await CreateSnippetCacheBuilder().GetDescriptionAndSnippetText(template, @"C:\foo.bicep");

        string expectedText = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

        description.Should().Be(string.Empty);
        expectedText.Should().Be(text);
    }

    [TestMethod]
    public async Task GetDescriptionAndSnippetText_WithCommentAndMissingDeclarations_ReturnsEmptyDescriptionAndText()
    {
        string template = @"// DNS Zone";

        (string description, string text) = await CreateSnippetCacheBuilder().GetDescriptionAndSnippetText(template, @"C:\foo.bicep");

        description.Should().Be(string.Empty);
        text.Should().Be(string.Empty);
    }

    [DataTestMethod]
    [DataRow("", "")]
    [DataRow("   ", "   ")]
    public void RemoveSnippetPlaceholderComments_WithInvalidInput_ReturnsInputTextAsIs(string input, string expected)
    {
        string actual = SnippetCacheBuilder.RemoveSnippetPlaceholderComments(input);

        actual.Should().Be(expected);
    }

    [TestMethod]
    public void RemoveSnippetPlaceholderComments_WithoutMatchingSnippetPlaceholderCommentPatternInInput_ReturnsInputTextAsIs()
    {
        string input = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: 'name'
  location: resourceGroup().location
}";

        string actual = SnippetCacheBuilder.RemoveSnippetPlaceholderComments(input);

        actual.Should().BeEquivalentToIgnoringNewlines(input);
    }

    [TestMethod]
    public void RemoveSnippetPlaceholderComments_WithMatchingSnippetPlaceholderCommentPatternInInput_RemovesSnippetPlaceholderComments()
    {
        string input = @"// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: /*${1:'name'}*/'name'
  location: resourceGroup().location
}

resource /*${2:dnsRecord}*/dnsRecord 'Microsoft.Network/dnsZones//*${3|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}*/A@2018-05-01' = {
  parent: dnsZone
  name: /*${4:'name'}*/'name'
  properties: {
    TTL: 3600
    mode: /*'${5|Detection,Prevention|}'*/'Detection'
    /*'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${6:'appServicePlan'}'*/'resource': 'Resource'
    '/*${7|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}*/ARecords': []
    precision: /*${8:-1}*/-1
    appSettings: [
    {
      name: 'AzureWebJobsDashboard'
      value: /*'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName1};AccountKey=${listKeys(${5:'storageAccountID1'}, '2019-06-01').key1}'*/'value'
    }
    id: /*$0*/
  }
}";

        string actual = SnippetCacheBuilder.RemoveSnippetPlaceholderComments(input);

        actual.Should().BeEquivalentToIgnoringNewlines(@"// DNS Record
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: ${1:'name'}
  location: resourceGroup().location
}

resource ${2:dnsRecord} 'Microsoft.Network/dnsZones/${3|A,AAAA,CNAME,MX,NS,PTR,SOA,SRV,TXT|}@2018-05-01' = {
  parent: dnsZone
  name: ${4:'name'}
  properties: {
    TTL: 3600
    mode: '${5|Detection,Prevention|}'
    'hidden-related:${resourceGroup().id}/providers/Microsoft.Web/serverfarms/${6:'appServicePlan'}': 'Resource'
    '${7|ARecords,AAAARecords,MXRecords,NSRecords,PTRRecords,SRVRecords,TXTRecords,CNAMERecord,SOARecord|}': []
    precision: ${8:-1}
    appSettings: [
    {
      name: 'AzureWebJobsDashboard'
      value: 'DefaultEndpointsProtocol=https;AccountName=${4:storageAccountName1};AccountKey=${listKeys(${5:'storageAccountID1'}, '2019-06-01').key1}'
    }
    id: $0
  }
}");
    }
}
