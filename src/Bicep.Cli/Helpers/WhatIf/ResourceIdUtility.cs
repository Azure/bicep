// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.RegularExpressions;
using Azure.Deployments.Core.Definitions.Identifiers;

namespace Bicep.Cli.Helpers.WhatIf;

/// <summary>
/// Class for building and parsing resource Ids.
/// </summary>
public static class ResourceIdUtility
{
    /// <summary>
    /// Split a fully qualified resource identifier into two parts (resource scope, relative resource identifier).
    /// </summary>
    /// <param name="fullyQualifiedResourceId">The fully qualified resource identifier to split.</param>
    /// <returns>The resource scope and the relative resource identifier.</returns>
    public static (string scope, string relativeResourceId) SplitResourceId(string fullyQualifiedResourceId)
    {
        ResourceId.TryParse(fullyQualifiedResourceId, out var resourceId);

        switch (resourceId)
        {
            case TenantLevelResourceId _:
                return ("/", GetUnqualified(resourceId, 2));
            case SubscriptionLevelResourceId subscriptionLevelId:
                return ($"/subscriptions/{subscriptionLevelId.SubscriptionId}", GetUnqualified(resourceId, 4));
            case ResourceGroupLevelResourceId rgLevelId:
                return ($"/subscriptions/{rgLevelId.SubscriptionId}/resourceGroups/{rgLevelId.ResourceGroup}", GetUnqualified(resourceId, 6));
            default:
                return ("<unknown>", fullyQualifiedResourceId);
        }
    }

    private static string GetUnqualified(ResourceId resourceId, int skipCount)
    {
        var sections = resourceId.ToString().Split('/');
        return string.Join('/', sections.Skip(skipCount));
    }
}
