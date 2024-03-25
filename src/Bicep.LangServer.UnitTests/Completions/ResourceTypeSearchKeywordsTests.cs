// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Resources;
using Bicep.LanguageServer.Completions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class ResourceTypeSearchKeywordsTests
    {
        [DataRow("Microsoft.Web/sites", "'appservice,webapp,function,Microsoft.Web/sites'")]
        [DataRow("microsoft.web/sites", "'appservice,webapp,function,microsoft.web/sites'")]
        [DataRow("microsoft.app/abc", "'containerapp,microsoft.app/abc'")]
        [DataRow("Microsoft.ContainerService/managedClusters", "'aks,kubernetes,k8s,cluster,Microsoft.ContainerService/managedClusters'")]
        [DataRow("toplevel1", "'top level keyword,toplevel1'")]
        [DataRow("toplevel1/secondlevel1", "'top level keyword,toplevel1/secondlevel1'")]
        [DataRow("toplevel1/secondlevel2", "'top level keyword,toplevel1/secondlevel2'")]
        [DataRow("toplevel1/secondlevel1/thirdlevel", "'top level keyword,toplevel1/secondlevel1/thirdlevel'")]
        [DataRow("toplevel2", null)]
        [DataRow("toplevel2/secondlevel1", "'second level keyword,toplevel2/secondlevel1'")]
        [DataRow("toplevel2/secondlevel2", null)]
        [DataRow("toplevel2/secondlevel2/thirdlevel", null)]
        [DataRow("toplevel2/secondlevel1/thirdlevel", "'second level keyword,toplevel2/secondlevel1/thirdlevel'")]

        [DataTestMethod]
        public void TryGetResourceTypeFilterText(string resourceType, string? expectedFilter)
        {
            var sut = new ResourceTypeSearchKeywords(new Dictionary<string, string[]>
            {
                ["Microsoft.Web/sites"] = ["appservice", "webapp", "function"],
                ["Microsoft.Web/serverFarms"] = ["asp", "appserviceplan", "hostingplan"],
                ["MICROSOFT.APP"] = ["containerapp"],
                ["Microsoft.ContainerService"] = ["aks", "kubernetes", "k8s", "cluster"],
                ["Microsoft.Authorization/roleAssignments"] = ["rbac"],
                ["toplevel1"] = ["top level keyword"],
                ["toplevel2/secondlevel1"] = ["second level keyword"],
            });

            var result = sut.TryGetResourceTypeFilterText(new ResourceTypeReference(resourceType, "2020-06-01"));

            result.Should().Be(expectedFilter);
        }
    }
}
