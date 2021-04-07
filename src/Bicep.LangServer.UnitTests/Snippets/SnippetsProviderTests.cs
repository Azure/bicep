// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.UnitTests.Assertions;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.LangServer.UnitTests.Snippets
{
    [TestClass]
    public class SnippetsProviderTests
    {
        [TestMethod]
        public void GetDescriptionAndText_WithEmptyInput_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            (string description, string text) = snippetsProvider.GetDescriptionAndText(string.Empty, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithNullInput_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            (string description, string text) = snippetsProvider.GetDescriptionAndText(null, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithOnlyWhitespaceInput_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            (string description, string text) = snippetsProvider.GetDescriptionAndText("   ", @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void GetDescriptionAndText_WithValidInput_ReturnsDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            string template = @"// DNS Zone
resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            string expectedText = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            Assert.AreEqual("DNS Zone", description);
            Assert.AreEqual(expectedText, text);
        }

        [TestMethod]
        public void GetDescriptionAndText_WithMissingCommentInInput_ReturnsEmptyDescriptionAndValidText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            string template = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            string expectedText = @"resource dnsZone 'Microsoft.Network/dnsZones@2018-05-01' = {
  name: '${1:dnsZone}'
  location: 'global'
  tags: {
    displayName: '${1:dnsZone}'
  }
}";

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.AreEqual(expectedText, text);
        }

        [TestMethod]
        public void GetDescriptionAndText_WithCommentAndMissingDeclarations_ReturnsEmptyDescriptionAndText()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            string template = @"// DNS Zone";

            (string description, string text) = snippetsProvider.GetDescriptionAndText(template, @"C:\foo.bicep");

            Assert.IsTrue(description.Equals(string.Empty));
            Assert.IsTrue(text.Equals(string.Empty));
        }

        [TestMethod]
        public void CompletionPriorityOfResourceSnippets_ShouldBeHigh()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();

            IEnumerable<Snippet> snippets = snippetsProvider.GetTopLevelNamedDeclarationSnippets()
                .Where(x => x.Prefix.StartsWith("resource"));

            foreach (Snippet snippet in snippets)
            {
                Assert.AreEqual(CompletionPriority.High, snippet.CompletionPriority);
            }
        }


        [TestMethod]
        public void CompletionPriorityOfNonResourceSnippets_ShouldBeMedium()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            IEnumerable<Snippet> snippets = snippetsProvider.GetTopLevelNamedDeclarationSnippets()
                .Where(x => !x.Prefix.StartsWith("resource"));

            foreach (Snippet snippet in snippets)
            {
                Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            }
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippet_WithValidTypeAndNoDependencies_ShouldReturnSnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            Snippet? snippet = snippetsProvider.GetResourceBodyCompletionSnippet("'Microsoft.Network/dnsZones@2018-05-01'");

            Assert.IsNotNull(snippet);
            Assert.AreEqual("res-dns-zone", snippet.Prefix);
            Assert.AreEqual("DNS Zone", snippet.Detail);
            Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            snippet.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: '${1:dnsZone}'
  location: 'global'
}");
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippet_WithInvalidType_ShouldReturnNull()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            Snippet? snippet = snippetsProvider.GetResourceBodyCompletionSnippet("'invalid_type'");

            Assert.IsNull(snippet);
        }

        [TestMethod]
        public void GetResourceBodyCompletionSnippet_WithValidTypeAndDependencies_ShouldReturnSnippet()
        {
            SnippetsProvider snippetsProvider = new SnippetsProvider();
            Snippet? snippet = snippetsProvider.GetResourceBodyCompletionSnippet("'Microsoft.Automation/automationAccounts/modules@2015-10-31'");

            Assert.IsNotNull(snippet);
            Assert.AreEqual("res-automation-module", snippet.Prefix);
            Assert.AreEqual("Automation Module", snippet.Detail);
            Assert.AreEqual(CompletionPriority.Medium, snippet.CompletionPriority);
            snippet.Text.Should().BeEquivalentToIgnoringNewlines(@"{
  name: '${automationAccount.name}/${2:automationVariable}'
  properties: {
    contentLink: {
      uri: '${3:https://content-url.nupkg}'
    }
  }
}
resource automationAccount 'Microsoft.Automation/automationAccounts@2015-10-31' = {
  name: '${1:automationAccount}'
}
");
        }
    }
}
