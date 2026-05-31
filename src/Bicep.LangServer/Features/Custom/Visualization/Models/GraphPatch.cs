// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Features.Custom.Visualization.Models
{
    /// <summary>
    /// A typed, ordered patch describing one change between the graph the client submitted and the
    /// server's current graph. A response carries a complete delta as a list of these patches, so an
    /// empty list means nothing changed. The <see cref="Op"/> property is the wire discriminator.
    /// <para>
    /// Deserialization requires <see cref="GraphPatchJsonConverter"/> to be registered on the serializer.
    /// The converter is intentionally not applied via a class-level attribute: such an attribute is
    /// inherited by the derived records and would cause the converter to re-enter itself when it
    /// deserializes a concrete patch type, resulting in infinite recursion.
    /// </para>
    /// </summary>
    public abstract record GraphPatch
    {
        public abstract string Op { get; }

        /// <summary>Replace the entire graph with an empty one (for example when no graph can be produced).</summary>
        public sealed record ClearGraph() : GraphPatch
        {
            public override string Op => "clearGraph";
        }

        public sealed record AddNode(GraphNode Node) : GraphPatch
        {
            public override string Op => "addNode";
        }

        public sealed record RemoveNode(string NodeId) : GraphPatch
        {
            public override string Op => "removeNode";
        }

        public sealed record UpdateNode(string NodeId, GraphNodeChanges Changes) : GraphPatch
        {
            public override string Op => "updateNode";
        }

        public sealed record AddEdge(GraphEdge Edge) : GraphPatch
        {
            public override string Op => "addEdge";
        }

        public sealed record RemoveEdge(string EdgeId) : GraphPatch
        {
            public override string Op => "removeEdge";
        }

        public sealed record SetNodeLayout(string NodeId, NodeLayout Layout) : GraphPatch
        {
            public override string Op => "setNodeLayout";
        }

        public sealed record SetErrorCount(int ErrorCount) : GraphPatch
        {
            public override string Op => "setErrorCount";
        }
    }

    /// <summary>
    /// Deserializes the <see cref="GraphPatch"/> discriminated union by reading the <c>op</c> field.
    /// Writing relies on default serialization (the <see cref="GraphPatch.Op"/> property emits <c>op</c>),
    /// so this converter is read-only.
    /// </summary>
    public sealed class GraphPatchJsonConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => objectType == typeof(GraphPatch);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var op = jObject.Value<string>("op");

            return op switch
            {
                "clearGraph" => new GraphPatch.ClearGraph(),
                "addNode" => Deserialize<GraphPatch.AddNode>(jObject, serializer),
                "removeNode" => Deserialize<GraphPatch.RemoveNode>(jObject, serializer),
                "updateNode" => Deserialize<GraphPatch.UpdateNode>(jObject, serializer),
                "addEdge" => Deserialize<GraphPatch.AddEdge>(jObject, serializer),
                "removeEdge" => Deserialize<GraphPatch.RemoveEdge>(jObject, serializer),
                "setNodeLayout" => Deserialize<GraphPatch.SetNodeLayout>(jObject, serializer),
                "setErrorCount" => Deserialize<GraphPatch.SetErrorCount>(jObject, serializer),
                _ => throw new JsonSerializationException($"Unknown graph patch op '{op}'."),
            };
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
            throw new NotSupportedException($"{nameof(GraphPatchJsonConverter)} is read-only; default serialization emits the discriminator.");

        private static GraphPatch Deserialize<T>(JObject jObject, JsonSerializer serializer)
            where T : GraphPatch =>
            jObject.ToObject<T>(serializer) ?? throw new JsonSerializationException($"Failed to deserialize graph patch of type '{typeof(T).Name}'.");
    }
}
