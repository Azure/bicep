// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Text.Json;
using Bicep.Core.Configuration;

namespace Bicep.LanguageServer.Features.Custom.InsertResource
{
    public interface IAzResourceProvider
    {
        public record AzResourceIdentifier(
            string FullyQualifiedId,
            string FullyQualifiedType,
            string FullyQualifiedName,
            string UnqualifiedName,
            string subscriptionId);

        Task<JsonElement> GetGenericResource(IBicepConfiguration configuration, AzResourceIdentifier resourceId, string? apiVersion, CancellationToken cancellationToken);
    }
}
