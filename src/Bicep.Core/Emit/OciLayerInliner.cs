// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit;

/// <summary>
/// Flattens a layered OCI artifact into a single template by recursively replacing
/// nested <c>templateLink.digest</c> references with the content of the referenced layer.
/// </summary>
internal static class OciLayerInliner
{
    public static JToken Inline(JToken template, ArmTemplateFile sourceFile)
    {
        if (!ContainsDigestLink(template))
        {
            return template;
        }

        var artifactDirectory = sourceFile.FileHandle.GetParent();
        var clone = template.DeepClone();

        InlineRecursively(clone, artifactDirectory, new HashSet<string>());

        return clone;
    }

    private static void InlineRecursively(JToken token, IDirectoryHandle artifactDirectory, IReadOnlySet<string> ancestorDigests)
    {
        switch (token)
        {
            case JObject obj:
                if (TryGetDigestLink(obj) is { } digest)
                {
                    if (ancestorDigests.Contains(digest))
                    {
                        throw new InvalidOperationException($"Detected a cycle while inlining OCI artifact layer \"{digest}\".");
                    }

                    var layer = ReadLayer(artifactDirectory, digest);

                    InlineRecursively(layer, artifactDirectory, new HashSet<string>(ancestorDigests) { digest });

                    obj.Property("templateLink")!.Replace(new JProperty("template", layer));

                    return;
                }

                foreach (var property in obj.Properties().ToArray())
                {
                    InlineRecursively(property.Value, artifactDirectory, ancestorDigests);
                }

                return;
            case JArray array:
                foreach (var item in array.ToArray())
                {
                    InlineRecursively(item, artifactDirectory, ancestorDigests);
                }

                return;
        }
    }

    private static JToken ReadLayer(IDirectoryHandle artifactDirectory, string digest)
    {
        if (OciLayerCache.TryGetLayerFile(artifactDirectory, digest) is not { } layerFile)
        {
            throw new InvalidOperationException($"Failed to find layer \"{digest}\" of the restored OCI artifact in \"{artifactDirectory.Uri}\". Try running a force restore.");
        }

        return JToken.Parse(layerFile.ReadAllText());
    }

    private static bool ContainsDigestLink(JToken token) => token switch
    {
        JObject obj => TryGetDigestLink(obj) is not null || obj.Properties().Any(p => ContainsDigestLink(p.Value)),
        JArray array => array.Any(ContainsDigestLink),
        _ => false,
    };

    private static string? TryGetDigestLink(JObject obj) =>
        obj["templateLink"] is JObject link && link.Count == 1 && link["digest"]?.Type == JTokenType.String
            ? link["digest"]!.Value<string>()
            : null;
}
