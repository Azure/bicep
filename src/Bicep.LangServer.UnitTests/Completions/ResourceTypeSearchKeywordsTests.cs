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
using Bicep.LanguageServer.Snippets;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Bicep.LangServer.UnitTests.Completions
{
    [TestClass]
    public class ResourceTypeSearchKeywordsTests
    {
        [DataRow("Microsoft.Web/sites", "'Microsoft.Web/sites appservice webapp function'")]
        [DataRow("microsoft.web/sites", "'microsoft.web/sites appservice webapp function'")]
        [DataRow("microsoft.app/abc", "'microsoft.app/abc containerapp'")]
        [DataRow("Microsoft.ContainerService/managedClusters", "'Microsoft.ContainerService/managedClusters aks kubernetes k8s cluster'")]
        [DataRow("toplevel1", "'toplevel1 top level keyword'")]
        [DataRow("toplevel1/secondlevel1", "'toplevel1/secondlevel1 top level keyword'")]
        [DataRow("toplevel1/secondlevel2", "'toplevel1/secondlevel2 top level keyword'")]
        [DataRow("toplevel1/secondlevel1/thirdlevel", "'toplevel1/secondlevel1/thirdlevel top level keyword'")]
        [DataRow("toplevel2", null)]
        [DataRow("toplevel2/secondlevel1", "'toplevel2/secondlevel1 second level keyword'")]
        [DataRow("toplevel2/secondlevel2", null)]
        [DataRow("toplevel2/secondlevel2/thirdlevel", null)]
        [DataRow("toplevel2/secondlevel1/thirdlevel", "'toplevel2/secondlevel1/thirdlevel second level keyword'")]
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

            var result = sut.TryGetResourceTypeFilterText(new ResourceTypeReference(resourceType, "2020-06-01"), surroundWithSingleQuotes: true);

            result.Should().Be(expectedFilter);
        }

        [DataRow(
            "res-app-plan",
            "Application Service Plan (Server Farm)",
            """
                no resources
                """,
            null)]
        [DataRow(
            "res-app-plan",
            "Application Service Plan (Server Farm)",
            """
                resource /*${1:appServicePlan}*/appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
                  name: /*${2:'name'}*/'name'
                  location: /*${3:location}*/'location'
                  sku: {
                    name: 'F1'
                    capacity: 1
                  }
                }
                """,
            "res-app-plan Application Service Plan (Server Farm) Microsoft.Web/serverfarms asp appserviceplan hostingplan")]
        [DataRow(
            "res-app-plan",
            "Application Service Plan (Server Farm)",
            """
                resource /*${1:appServicePlan}*/appServicePlan 'Microsoft.Web/serverfarms@2020-12-01' = {
                  name: /*${2:'name'}*/'name'
                  location: /*${3:location}*/'location'
                  sku: {
                    name: 'F1'
                    capacity: 1
                  }
                }
                resource /*${1:appServicePlan}*/appServicePlan 'Microsoft.Web/serverfarms@3333-12-01' = {
                  name: /*${2:'name'}*/'name'
                  location: /*${3:location}*/'location'
                  sku: {
                    name: 'F1'
                    capacity: 1
                  }
                }
                resource /*${1:appServicePlan}*/appServicePlan 'Microsoft.Web/unknown@2020-12-01-beta' = {
                  name: /*${2:'name'}*/'name'
                  location: /*${3:location}*/'location'
                  sku: {
                    name: reference
                    capacity: 1
                  }
                }
                """,
            "res-app-plan Application Service Plan (Server Farm) Microsoft.Web/serverfarms asp appserviceplan hostingplan Microsoft.Web/unknown")]
        [DataRow(
            "res-automation-job-schedule",
            "Automation Job Schedule",
            """
            resource automationAccount 'Microsoft.Automation/automationAccounts@2019-06-01' = {
              name: /*${1:'name'}*/'name'
            }

            resource /*${2:automationJobSchedule}*/automationJobSchedule 'Microsoft.Automation/automationAccounts/jobSchedules@2019-06-01' = {
              parent: automationAccount
              name: /*${3:'name'}*/'name'
              properties: {
                schedule: {
                  name: /*${4:'name'}*/'name'
                }
                runbook: {
                  name: /*${5:'name'}*/'name'
                }
              }
            }
            """,
            "res-automation-job-schedule Automation Job Schedule Microsoft.Automation/automationAccounts Microsoft.Automation/automationAccounts/jobSchedules")]
        [DataTestMethod]
        public void TryGetSnippetFilterText(string prefix, string detail, string text, string? expectedFilter)
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

            var snippet = new Snippet(text, prefix: prefix, detail: detail);
            var result = sut.TryGetSnippetFilterText(snippet);

            result.Should().Be(expectedFilter);
        }
    }
}
