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
    /// On the wire, patches are serialized by OmniSharp's Newtonsoft-based serializer. Writing relies on
    /// default serialization (the runtime record type emits all of its properties, and <see cref="Op"/>
    /// emits the <c>op</c> discriminator). Reading the union back is handled by
    /// <see cref="GraphPatchJsonConverter"/>, applied here as a class-level attribute so the type is
    /// self-describing for any Newtonsoft serializer (including OmniSharp's typed client). The attribute
    /// is inherited by the derived records, so the converter uses a re-entrancy guard to fall back to
    /// default deserialization once it has selected the concrete patch type, avoiding infinite recursion.
    /// </para>
    /// </summary>
    [JsonConverter(typeof(GraphPatchJsonConverter))]
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

        public sealed record SetGraphBounds(GraphBounds Bounds) : GraphPatch
        {
            public override string Op => "setGraphBounds";
        }

        public sealed record SetErrorCount(int ErrorCount) : GraphPatch
        {
            public override string Op => "setErrorCount";
        }
    }

    /// <summary>
    /// Deserializes the <see cref="GraphPatch"/> discriminated union by reading the <c>op</c> field.
    /// Writing relies on default serialization (the <see cref="GraphPatch.Op"/> property emits <c>op</c>),
    /// so this converter is read-only. It is applied to <see cref="GraphPatch"/> via a class-level
    /// attribute, which the derived records inherit. To avoid re-entering itself when deserializing a
    /// concrete patch type, it uses a thread-local guard so the inner <see cref="JToken.ToObject(Type, JsonSerializer)"/>
    /// call falls back to default deserialization.
    /// </summary>
    public sealed class GraphPatchJsonConverter : JsonConverter
    {
        [ThreadStatic]
        private static bool skipConverter;

        public override bool CanWrite => false;

        public override bool CanRead => !skipConverter;

        public override bool CanConvert(Type objectType) => typeof(GraphPatch).IsAssignableFrom(objectType);

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var op = jObject.Value<string>("op");

            var targetType = op switch
            {
                "clearGraph" => typeof(GraphPatch.ClearGraph),
                "addNode" => typeof(GraphPatch.AddNode),
                "removeNode" => typeof(GraphPatch.RemoveNode),
                "updateNode" => typeof(GraphPatch.UpdateNode),
                "addEdge" => typeof(GraphPatch.AddEdge),
                "removeEdge" => typeof(GraphPatch.RemoveEdge),
                "setNodeLayout" => typeof(GraphPatch.SetNodeLayout),
                "setGraphBounds" => typeof(GraphPatch.SetGraphBounds),
                "setErrorCount" => typeof(GraphPatch.SetErrorCount),
                _ => throw new JsonSerializationException($"Unknown graph patch op '{op}'."),
            };

            return Deserialize(jObject, targetType, serializer);
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) =>
            throw new NotSupportedException($"{nameof(GraphPatchJsonConverter)} is read-only; default serialization emits the discriminator.");

        private static GraphPatch Deserialize(JObject jObject, Type targetType, JsonSerializer serializer)
        {
            // The attribute is inherited by the concrete patch types, so suppress the converter for the
            // nested call to avoid recursing back into ReadJson.
            skipConverter = true;
            try
            {
                return (GraphPatch?)jObject.ToObject(targetType, serializer)
                    ?? throw new JsonSerializationException($"Failed to deserialize graph patch of type '{targetType.Name}'.");
            }
            finally
            {
                skipConverter = false;
            }
        }
    }
}
