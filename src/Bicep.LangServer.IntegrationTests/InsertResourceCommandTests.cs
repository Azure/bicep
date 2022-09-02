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
            return await LanguageServerHelper.StartServerWithClientConnectionAsync(
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
                new LanguageServer.Server.CreationOptions(
                    onRegisterServices: services => services
                        .AddSingleton<IAzResourceProvider>(azResourceProvider)
                        .AddSingleton<IAzResourceTypeLoader>(azResourceTypeLoader)));
        }

        [TestMethod]
        public async Task Insert_resource_command_should_insert_basic_resource()
        {
            var documentUri = DocumentUri.From("/template.bicep");
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

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = resourceId.FullyQualifiedId,
            }, default);

            var edit = await listeners.ApplyWorkspaceEdit.WaitNext();

            var changes = edit.Edit.Changes![documentUri];
            changes.Should().HaveCount(1);
            var test = changes.First();

            var startOffset = PositionHelper.GetOffset(lineStarts, test.Range.Start);
            var endOffset = PositionHelper.GetOffset(lineStarts, test.Range.End);

            var replacedFile = file.Substring(0, startOffset) + test.NewText + file.Substring(endOffset);

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
        public async Task Insert_resource_command_should_insert_resource_group()
        {
            var documentUri = DocumentUri.From("/template.bicep");
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

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = resourceId.FullyQualifiedId,
            }, default);

            var edit = await listeners.ApplyWorkspaceEdit.WaitNext();

            var changes = edit.Edit.Changes![documentUri];
            changes.Should().HaveCount(1);
            var test = changes.First();

            var startOffset = PositionHelper.GetOffset(lineStarts, test.Range.Start);
            var endOffset = PositionHelper.GetOffset(lineStarts, test.Range.End);

            var replacedFile = file.Substring(0, startOffset) + test.NewText + file.Substring(endOffset);

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
            var documentUri = DocumentUri.From("/template.bicep");
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

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = resourceId.FullyQualifiedId,
            }, default);

            var edit = await listeners.ApplyWorkspaceEdit.WaitNext();

            var changes = edit.Edit.Changes![documentUri];
            changes.Should().HaveCount(1);
            var test = changes.First();

            var startOffset = PositionHelper.GetOffset(lineStarts, test.Range.Start);
            var endOffset = PositionHelper.GetOffset(lineStarts, test.Range.End);

            var replacedFile = file.Substring(0, startOffset) + test.NewText + file.Substring(endOffset);

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
            var documentUri = DocumentUri.From("/template.bicep");
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeDefinition = TestTypeHelper.CreateCustomResourceType("My.Rp/myTypes", "2020-01-01", TypeSymbolValidationFlags.WarnOnTypeMismatch,
                new TypeProperty("readOnlyProp", LanguageConstants.String, TypePropertyFlags.ReadOnly),
                new TypeProperty("readWriteProp", LanguageConstants.String, TypePropertyFlags.None),
                new TypeProperty("writeOnlyProp", LanguageConstants.String, TypePropertyFlags.WriteOnly));
            var typeLoader = TestTypeHelper.CreateAzResourceTypeLoaderWithTypes(typeDefinition.AsEnumerable());

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = "this isn't a resource id!",
            }, default);

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
            var documentUri = DocumentUri.From("/template.bicep");
            var listeners = CreateListeners();
            var mockAzResourceProvider = new Mock<IAzResourceProvider>(MockBehavior.Strict);

            var typeLoader = TestTypeHelper.CreateEmptyAzResourceTypeLoader();

            using var helper = await StartLanguageServer(listeners, mockAzResourceProvider.Object, typeLoader);
            var client = helper.Client;

            var resourceId = ResourceGroupLevelResourceId.Create("23775d31-d753-4290-805b-e5bde53eba6e", "myRg", "MadeUp.Rp", new[] { "madeUpTypes" }, new[] { "myName" });

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = resourceId.FullyQualifiedId,
            }, default);

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
            var documentUri = DocumentUri.From("/template.bicep");
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

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = resourceId.FullyQualifiedId,
            }, default);

            var edit = await listeners.ApplyWorkspaceEdit.WaitNext();

            var changes = edit.Edit.Changes![documentUri];
            changes.Should().HaveCount(1);
            var test = changes.First();

            var startOffset = PositionHelper.GetOffset(lineStarts, test.Range.Start);
            var endOffset = PositionHelper.GetOffset(lineStarts, test.Range.End);

            var replacedFile = file.Substring(0, startOffset) + test.NewText + file.Substring(endOffset);

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
            var documentUri = DocumentUri.From("/template.bicep");
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

            var (file, cursors) = ParserHelper.GetFileWithCursors(@"
param myParam string = 'test'
resource myRes 'myRp/provider@2019-01-01' = {
  name: 'te|st'
}
module myMod './module.bicep' = {
  name: 'test'
}
output myOutput string = 'myOutput'
");
            var lineStarts = TextCoordinateConverter.GetLineStarts(file);

            client.TextDocument.DidOpenTextDocument(TextDocumentParamHelper.CreateDidOpenDocumentParams(documentUri, file, 0));
            await listeners.Diagnostics.WaitNext();

            var cursor = cursors.Single();
            var result = await client.SendRequest(new InsertResourceParams
            {
                TextDocument = documentUri,
                Position = PositionHelper.GetPosition(lineStarts, cursor),
                ResourceId = resourceId.FullyQualifiedId,
            }, default);

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
