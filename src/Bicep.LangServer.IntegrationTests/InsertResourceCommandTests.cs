// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Bicep.Core.UnitTests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Newtonsoft.Json.Linq;
using Bicep.LangServer.IntegrationTests.Helpers;
using FluentAssertions;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Moq;
using Bicep.LanguageServer.Providers;
using OmniSharp.Extensions.LanguageServer.Protocol;
using Bicep.LanguageServer.Handlers;
using Bicep.LanguageServer.Utils;
using Bicep.Core.Text;
using Azure.Deployments.Core.Definitions.Identifiers;
using Bicep.Core.Configuration;
using System.Threading;
using System.Text.Json;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.TypeSystem;
using Bicep.Core;
using Bicep.Core.Extensions;
using System.Linq;
using System;
using Bicep.LangServer.IntegrationTests.Assertions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Bicep.Core.Navigation;
using Bicep.Core.Workspaces;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using System.Collections.Immutable;

namespace Bicep.LangServer.IntegrationTests
{
    [TestClass]
    public class InsertResourceCommandTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        private record Listeners(
            MultipleMessageListener<PublishDiagnosticsParams> Diagnostics,
            MultipleMessageListener<ShowMessageParams> ShowMessage,
            MultipleMessageListener<ApplyWorkspaceEditParams> ApplyWorkspaceEdit,
            MultipleMessageListener<TelemetryEventParams> Telemetry);

        private Listeners CreateListeners()
            => new(new(), new(), new(), new());

        private async Task<LanguageServerHelper> StartLanguageServer(
            Listeners listeners,
            IAzResourceProvider azResourceProvider,
            IAzResourceTypeLoader azResourceTypeLoader)
        {
            return await LanguageServerHelper.StartServer(
                this.TestContext,
                options => options
                    .OnPublishDiagnostics(listeners.Diagnostics.AddMessage)
                    .OnShowMessage(listeners.ShowMessage.AddMessage)
                    .OnApplyWorkspaceEdit(async p =>
                    {
                        listeners.ApplyWorkspaceEdit.AddMessage(p);

                        await Task.Yield();
                        return new();
                    })
                    .OnTelemetryEvent(listeners.Telemetry.AddMessage),
                services => services.WithAzResourceTypeLoader(azResourceTypeLoader).WithAzResourceProvider(azResourceProvider));
        }

        private static async Task<string> ApplyWorkspaceEdit(Listeners listeners, Uri fileUri, string fileContents)
        {
            var edit = await listeners.ApplyWorkspaceEdit.WaitNext();

            var changes = edit.Edit.Changes![fileUri];
            changes.Should().HaveCount(1);
            var test = changes.First();

            var lineStarts = TextCoordinateConverter.GetLineStarts(fileContents);
            var startOffset = PositionHelper.GetOffset(lineStarts, test.Range.Start);
            var endOffset = PositionHelper.GetOffset(lineStarts, test.Range.End);

            return fileContents.Substring(0, startOffset) + test.NewText + fileContents.Substring(endOffset);
        }

        private async Task<string> InvokeInsertResource(ILanguageClient client, Listeners listeners, Uri fileUri, string fileWithCursor, string resourceId)
        {
            var (fileContents, cursor) = ParserHelper.GetFileWithSingleCursor(fileWithCursor, '|');
            var file = SourceFileFactory.CreateBicepFile(fileUri, fileContents);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(file.FileUri, fileContents, 0));
            await listeners.Diagnostics.WaitNext();

            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = DocumentUri.From(file.FileUri),
                Position = PositionHelper.GetPosition(file.LineStarts, cursor),
                ResourceId = resourceId,
            }, default);

            return fileContents;
        }

        [TestMethod]
        public async Task Insert_resource_command_should_insert_basic_resource()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly),
                new TypeProperty("floatProp", LanguageConstants.Int, TypePropertyFlags.None),
                new TypeProperty("bigIntProp", LanguageConstants.Int, TypePropertyFlags.None));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "My.Rp", new[] { "myTypes" }, new[] { "myName" });
            var mockResource = new JObject
            {
                ["id"] = resourceId.FullyQualifiedId,
                ["name"] = resourceId.NameHierarchy.Last(),
                ["type"] = resourceId.FormatFullyQualifiedType(),
                ["properties"] = new JObject
                {
                    ["readOnlyProp"] = "abc",
                    ["readWriteProp"] = "def",
                    ["writeOnlyProp"] = "ghi",
                    ["int64Prop"] = 9223372036854775807,
                    ["floatProp"] = 0.5,
                    ["bigIntProp"] = JToken.Parse("3456789871234786124871623847612837461287436"),
                },
            };

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), "2020-01-01", It.IsAny<CancellationToken>()))
                .Returns(async () => await JsonSerializer.DeserializeAsync<JsonElement>(mockResource.ToJsonStream()));

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", resourceId.FullyQualifiedId);

            var replacedFile = await ApplyWorkspaceEdit(listeners, fileUri, fileContents);
            replacedFile.Should().Be(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'test'
}
@description('Generated from /subscriptions/23775d31-d753-4290-805b-e5bde53eba6e/resourceGroups/myRg/providers/My.Rp/myTypes/myName')
resource myName 'My.Rp/myTypes@2020-01-01' = {
  name: 'myName'
  properties: {
    readWriteProp: 'def'
    writeOnlyProp: 'ghi'
    int64Prop: 9223372036854775807
    floatProp: json('0.5')
    bigIntProp: json('3456789871234786124871623847612837461287436')
  }
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/success", new JObject
                {
                    ["resourceType"] = "My.Rp/myTypes",
                    ["apiVersion"] = "2020-01-01",
                });
        }

        [TestMethod]
        public async Task Insert_resource_command_works_for_empty_file()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch);
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "My.Rp", new[] { "myTypes" }, new[] { "myName" });
            var mockResource = new JObject
            {
                ["id"] = resourceId.FullyQualifiedId,
                ["name"] = resourceId.NameHierarchy.Last(),
                ["type"] = resourceId.FormatFullyQualifiedType(),
                ["properties"] = new JObject { },
            };

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), "2020-01-01", It.IsAny<CancellationToken>()))
                .Returns(async () => await JsonSerializer.DeserializeAsync<JsonElement>(mockResource.ToJsonStream()));

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"|", resourceId.FullyQualifiedId);

            var replacedFile = await ApplyWorkspaceEdit(listeners, fileUri, fileContents);
            replacedFile.Should().Be(@"
@description('Generated from /subscriptions/23775d31-d753-4290-805b-e5bde53eba6e/resourceGroups/myRg/providers/My.Rp/myTypes/myName')
resource myName 'My.Rp/myTypes@2020-01-01' = {
  name: 'myName'
  properties: {
  }
}");
        }

        [TestMethod]
        public async Task Insert_resource_command_should_insert_resource_group()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("Microsoft.Resources/resourceGroups", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = new IAzResourceProvider.AzResourceIdentifier(
                "/subscriptions/23775d31-d753-4290-805b-e5bde53eba6e/resourceGroups/myRg",
                "Microsoft.Resources/resourceGroups",
                "myRg",
                "myRg",
                "23775d31-d753-4290-805b-e5bde53eba6e");
            var mockResource = new JObject
            {
                ["id"] = resourceId.FullyQualifiedId,
                ["name"] = resourceId.UnqualifiedName,
                ["type"] = resourceId.FullyQualifiedType,
                ["properties"] = new JObject
                {
                    ["readOnlyProp"] = "abc",
                    ["readWriteProp"] = "def",
                    ["writeOnlyProp"] = "ghi",
                },
            };

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), "2020-01-01", It.IsAny<CancellationToken>()))
                .Returns(async () => await JsonSerializer.DeserializeAsync<JsonElement>(mockResource.ToJsonStream()));

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", resourceId.FullyQualifiedId);

            var replacedFile = await ApplyWorkspaceEdit(listeners, fileUri, fileContents);
            replacedFile.Should().Be(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'test'
}
@description('Generated from /subscriptions/23775d31-d753-4290-805b-e5bde53eba6e/resourceGroups/myRg')
resource myRg 'Microsoft.Resources/resourceGroups@2020-01-01' = {
  name: 'myRg'
  properties: {
    readWriteProp: 'def'
    writeOnlyProp: 'ghi'
  }
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/success", new JObject
                {
                    ["resourceType"] = "Microsoft.Resources/resourceGroups",
                    ["apiVersion"] = "2020-01-01",
                });
        }

        [TestMethod]
        public async Task Insert_resource_command_should_insert_child_resource()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes/childType", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "My.Rp", new[] { "myTypes", "childType" }, new[] { "myName", "childName" });
            var mockResource = new JObject
            {
                ["id"] = resourceId.FullyQualifiedId,
                ["name"] = resourceId.NameHierarchy.Last(),
                ["type"] = resourceId.FormatFullyQualifiedType(),
                ["properties"] = new JObject
                {
                    ["readOnlyProp"] = "abc",
                    ["readWriteProp"] = "def",
                    ["writeOnlyProp"] = "ghi",
                },
            };

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), "2020-01-01", It.IsAny<CancellationToken>()))
                .Returns(async () => await JsonSerializer.DeserializeAsync<JsonElement>(mockResource.ToJsonStream()));

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", resourceId.FullyQualifiedId);

            var replacedFile = await ApplyWorkspaceEdit(listeners, fileUri, fileContents);
            replacedFile.Should().Be(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'test'
}
@description('Generated from /subscriptions/23775d31-d753-4290-805b-e5bde53eba6e/resourceGroups/myRg/providers/My.Rp/myTypes/myName/childType/childName')
resource childName 'My.Rp/myTypes/childType@2020-01-01' = {
  name: 'myName/childName'
  properties: {
    readWriteProp: 'def'
    writeOnlyProp: 'ghi'
  }
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/success", new JObject
                {
                    ["resourceType"] = "My.Rp/myTypes/childType",
                    ["apiVersion"] = "2020-01-01",
                });
        }

        [TestMethod]
        public async Task Insert_resource_command_displays_error_for_incorrectly_formatted_resourceId()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", "this isn't a resource id!");

            var message = await listeners.ShowMessage.WaitNext();
            message.Should().HaveMessageAndType(
                "Failed to parse supplied resourceId \"this isn't a resource id!\".",
                MessageType.Error);

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/failure", new JObject
                {
                    ["failureType"] = "ParseResourceIdFailed"
                });
        }

        [TestMethod]
        public async Task Insert_resource_command_displays_error_for_resource_with_no_types()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeLoader = TestTypeHelper.CreateEmptyAzResourceTypeLoader();

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "MadeUp.Rp", new[] { "madeUpTypes" }, new[] { "myName" });

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", resourceId.FullyQualifiedId);

            var message = await listeners.ShowMessage.WaitNext();
            message.Should().HaveMessageAndType(
                "Failed to find a Bicep type definition for resource of type \"MadeUp.Rp/madeUpTypes\".",
                MessageType.Error);

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/failure", new JObject
                {
                    ["failureType"] = "MissingType(MadeUp.Rp/madeUpTypes)",
                });
        }

        [TestMethod]
        public async Task Insert_resource_command_should_try_to_fetch_without_apiVersion_if_apiVersion_lookup_fails()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "My.Rp", new[] { "myTypes" }, new[] { "myName" });

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), "2020-01-01", It.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException("Something went wrong!"));

            var mockResource = new JObject
            {
                ["id"] = resourceId.FullyQualifiedId,
                ["name"] = resourceId.NameHierarchy.Last(),
                ["type"] = resourceId.FormatFullyQualifiedType(),
                ["properties"] = new JObject
                {
                    ["readOnlyProp"] = "abc",
                    ["readWriteProp"] = "def",
                    ["writeOnlyProp"] = "ghi",
                },
            };

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), null, It.IsAny<CancellationToken>()))
                .Returns(async () => await JsonSerializer.DeserializeAsync<JsonElement>(mockResource.ToJsonStream()));

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", resourceId.FullyQualifiedId);

            var replacedFile = await ApplyWorkspaceEdit(listeners, fileUri, fileContents);
            replacedFile.Should().Be(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'test'
}
@description('Generated from /subscriptions/23775d31-d753-4290-805b-e5bde53eba6e/resourceGroups/myRg/providers/My.Rp/myTypes/myName')
resource myName 'My.Rp/myTypes@2020-01-01' = {
  name: 'myName'
  properties: {
    readWriteProp: 'def'
    writeOnlyProp: 'ghi'
  }
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/success", new JObject
                {
                    ["resourceType"] = "My.Rp/myTypes",
                    ["apiVersion"] = "2020-01-01",
                });
        }

        [TestMethod]
        public async Task Insert_resource_command_should_return_exception_info_if_both_GETs_fail()
        {
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "My.Rp", new[] { "myTypes" }, new[] { "myName" });

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), "2020-01-01", It.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException("Something went wrong!"));

            mockAzResourceProvider.Setup(x => x.GetGenericResource(It.IsAny<RootConfiguration>(), It.Is<IAzResourceProvider.AzResourceIdentifier>(x => x.FullyQualifiedId == resourceId.FullyQualifiedId), null, It.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException("And something went wrong again!"));

            var fileUri = new Uri("file:///template.bicep");
            var fileContents = await InvokeInsertResource(client, listeners, fileUri, @"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
", resourceId.FullyQualifiedId);

            var message = await listeners.ShowMessage.WaitNext();
            message.Should().HaveMessageAndType(
                "Caught exception fetching resource: And something went wrong again!.",
                MessageType.Error);

            var telemetry = await listeners.Telemetry.WaitForAll();
            telemetry.Should().ContainEvent("InsertResource/failure", new JObject
                {
                    ["failureType"] = "FetchResourceFailure",
                });
        }
    }
}
