// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.LanguageServer.Features.Custom.Visualization.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace Bicep.LangServer.UnitTests.Features.Visualization;

[TestClass]
public class GraphPatchSerializationTests
{
    // Patches travel over the LSP wire serialized by OmniSharp's serializer, so the test exercises that exact
    // stack (camelCase naming, LSP model converters, and so on) rather than a hand-rolled configuration. No
    // patch converter is registered here: GraphPatch carries a class-level converter attribute, so reading the
    // union back works on a plain OmniSharp serializer, exactly as it does on the typed client.
    private static readonly JsonSerializer Serializer = CreateWireSerializer();

    private static JsonSerializer CreateWireSerializer() => JsonSerializer.Create(new LspSerializer().Settings);

    private static JToken Serialize(GraphPatch patch) => JToken.FromObject(patch, Serializer);

    private static GraphPatch Deserialize(JToken token) =>
        token.ToObject<GraphPatch>(Serializer) ?? throw new InvalidOperationException("Deserialization returned null.");

    [TestMethod]
    public void ClearGraph_WhenSerialized_RoundTripsWithCamelCaseOp()
    {
        var patch = new GraphPatch.ClearGraph();

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("clearGraph");
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void AddNode_WhenSerialized_RoundTripsWithCamelCaseFields()
    {
        var node = new GraphNode(
            Id: "res:foo",
            Kind: GraphNodeKind.Resource,
            ParentId: null,
            Type: "Microsoft.Storage/storageAccounts",
            SymbolName: "foo",
            IsCollection: false,
            HasChildren: true,
            HasError: false);
        var patch = new GraphPatch.AddNode(node);

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("addNode");
        json["node"]!.Value<string>("symbolName").Should().Be("foo");
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void RemoveNode_WhenSerialized_RoundTrips()
    {
        var patch = new GraphPatch.RemoveNode("res:foo");

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("removeNode");
        json.Value<string>("nodeId").Should().Be("res:foo");
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void UpdateNode_WithPartialChanges_RoundTrips()
    {
        var patch = new GraphPatch.UpdateNode("res:foo", new GraphNodeChanges(HasError: true));

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("updateNode");
        var changes = json["changes"].Should().BeOfType<JObject>().Subject;
        changes.ContainsKey("type").Should().BeFalse();
        changes.ContainsKey("isCollection").Should().BeFalse();
        changes.ContainsKey("hasChildren").Should().BeFalse();
        changes.Value<bool>("hasError").Should().BeTrue();
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void AddEdge_WhenSerialized_RoundTrips()
    {
        var patch = new GraphPatch.AddEdge(new GraphEdge("e1", "res:a", "res:b"));

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("addEdge");
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void RemoveEdge_WhenSerialized_RoundTrips()
    {
        var patch = new GraphPatch.RemoveEdge("e1");

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("removeEdge");
        json.Value<string>("edgeId").Should().Be("e1");
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void SetNodeLayout_WhenSerialized_RoundTrips()
    {
        var patch = new GraphPatch.SetNodeLayout("res:foo", new NodeLayout(12.5, -3.25));

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("setNodeLayout");
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void SetGraphBounds_WhenSerialized_RoundTrips()
    {
        var patch = new GraphPatch.SetGraphBounds(new GraphBounds(640.5, 480.25));

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("setGraphBounds");
        json["bounds"]!.Value<double>("width").Should().Be(640.5);
        json["bounds"]!.Value<double>("height").Should().Be(480.25);
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void SetErrorCount_WhenSerialized_RoundTripsWithCamelCaseOp()
    {
        var patch = new GraphPatch.SetErrorCount(3);

        var json = Serialize(patch);

        json.Value<string>("op").Should().Be("setErrorCount");
        json.Value<int>("errorCount").Should().Be(3);
        Deserialize(json).Should().Be(patch);
    }

    [TestMethod]
    public void Deserialize_WithUnknownOp_Throws()
    {
        var json = new JObject { ["op"] = "bogus" };

        var act = () => Deserialize(json);

        act.Should().Throw<JsonSerializationException>().WithMessage("*bogus*");
    }
}
