// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit
{
    /// <summary>
    /// A content-addressed store of nested module templates that is shared across all levels of a
    /// compilation. Identical template bodies are stored once and referenced by their sha256 digest
    /// via a module's <c>templateLink.contentId</c> property, instead of being inlined repeatedly.
    /// </summary>
    public sealed class ModuleContentStore
    {
        // Ordered by digest so the emitted "content" section is deterministic.
        private readonly SortedDictionary<string, JToken> contentByContentId = new(StringComparer.Ordinal);

        /// <summary>
        /// Adds a template body to the store if it is not already present and returns its contentId
        /// (e.g. "sha256:abc123...").
        /// </summary>
        public string Add(JToken content)
        {
            // Serialize without indentation and hash the UTF-8 bytes so that byte-identical templates
            // always produce the same digest regardless of platform line endings.
            var serialized = content.ToString(Formatting.None);
            var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(serialized))).ToLowerInvariant();
            var contentId = $"sha256:{digest}";

            contentByContentId.TryAdd(contentId, content);

            return contentId;
        }

        public bool IsEmpty => contentByContentId.Count == 0;

        public IEnumerable<KeyValuePair<string, JToken>> Entries => contentByContentId;
    }
}
