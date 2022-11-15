// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Snippets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Bicep.LangServer.UnitTests.Providers
{
    [TestClass]
    public class MCRCompletionProviderTests
    {
        //[TestMethod]
        //public void VerifyGetModuleNames()
        //{
        //    MCRCompletionProvider mcrCompletionProvider = new MCRCompletionProvider();
        //    List<string> moduleNames = mcrCompletionProvider.GetModuleNames();

        //    moduleNames.Should().NotBeNull();
        //}

        [TestMethod]
        public void VerifyGetTags()
        {
            MCRCompletionProvider mcrCompletionProvider = new MCRCompletionProvider();
            List<string> tags = mcrCompletionProvider.GetTags("app/dapr-containerapp");

            tags.Should().NotBeNull();
            tags.Should().Contain("1.0.1");
            tags.Should().Contain("1.0.2");
        }
    }
}
