// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Configuration;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Providers
{
    public interface IAzResourceProvider
    {
        public record AzResourceIdentifier(
            string FullyQualifiedId,
            string FullyQualifiedType,
            string FullyQualifiedName,
            string UnqualifiedName,
            string subscriptionId);

        Task<JsonElement> GetGenericResource(RootConfiguration configuration, AzResourceIdentifier resourceId, string? apiVersion, CancellationToken cancellationToken);
    }
}
