// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Core.Helpers;
using Azure.Deployments.DiffEngine;
using Azure.Deployments.Engine.Definitions;
using Azure.Deployments.Engine.Workers;
using Azure.Deployments.Engine.Workers.Utils;
using Azure.Deployments.Expression.Intermediate.Extensions;
using Azure.Deployments.Templates.Engines;
using Bicep.Cli.Commands;
using JsonDiffPatchDotNet;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Cli.Helpers.Snapshot;

public class SnapshotDiffer
{
    public static IReadOnlyList<DeploymentWhatIfResourceChangeDefinition> CalculateChanges(Snapshot source, Snapshot target)
    {
        var changes = new List<DeploymentWhatIfResourceChangeDefinition>();
        var differ = new LinearDiffer<DeploymentWhatIfResource>(
            itemComparer: DeploymentWhatIfResource.CompareResources,
            addCallback: addedResource =>
            {
                changes.Add(item: new DeploymentWhatIfResourceChangeDefinition
                {
                    ResourceId = addedResource.Id,
                    ChangeType = DeploymentWhatIfChangeType.Create,
                    Before = null,
                    After = addedResource.Body,
                });
            },
            removeCallback: removedResource =>
            {
                changes.Add(new DeploymentWhatIfResourceChangeDefinition
                {
                    ResourceId = removedResource.Id,
                    ChangeType = DeploymentWhatIfChangeType.Delete,
                    Before = removedResource.Body,
                    After = null,
                });
            },
            updateCallback: (resourceBefore, resourceAfter) =>
            {
                var jdp = new JsonDiffPatch(new Options { TextDiff = TextDiffMode.Simple });
                var delta = DeltaConverter.Convert(delta: jdp.Diff(left: resourceBefore.Body, right: resourceAfter.Body));

                if (!JToken.DeepEquals(resourceBefore.Body, resourceAfter.Body))
                {
                    changes.Add(new DeploymentWhatIfResourceChangeDefinition
                    {
                        ResourceId = resourceAfter.Id,
                        ChangeType = DeploymentWhatIfChangeType.Modify,
                        Delta = delta
                    });
                }
            });

        var before = source.PredictedResources.Select(ConvertToWhatIfResource).ToList();
        before.Sort(DeploymentWhatIfResource.CompareResources);
        var after = target.PredictedResources.Select(ConvertToWhatIfResource).ToList();
        after.Sort(DeploymentWhatIfResource.CompareResources);

        differ.ComputeDifferences(before, after);

        return changes;
    }

    private static DeploymentWhatIfResource ConvertToWhatIfResource(JsonElement resource)
    {
        var id = resource.GetProperty("id").ToString();
        var apiVersion = resource.GetProperty("apiVersion").ToString();

        return new()
        {
            Id = id,
            ApiVersion = apiVersion,
            Body = DeploymentWhatIfJob.CorrectResourceBodyIdentifiers(
                resourceBody: resource.ToString().FromJson<JObject>(),
                fullyQualifiedResourceId: id,
                apiVersion: apiVersion),
        };
    }
}
