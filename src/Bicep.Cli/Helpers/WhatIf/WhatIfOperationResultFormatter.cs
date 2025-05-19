// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Core.Entities;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Microsoft.WindowsAzure.ResourceStack.Common.Json;
using Newtonsoft.Json.Linq;

#nullable disable
namespace Bicep.Cli.Helpers.WhatIf;

public class WhatIfOperationResultFormatter : WhatIfJsonFormatter
{
    public WhatIfOperationResultFormatter(ColoredStringBuilder builder)
        : base(builder)
    {
    }

    public static string Format(IReadOnlyList<DeploymentWhatIfResourceChangeDefinition> resourceChanges)
    {
        var builder = new ColoredStringBuilder();
        var formatter = new WhatIfOperationResultFormatter(builder);
        formatter.FormatResourceChanges(resourceChanges, true);

        return builder.ToString();
    }

    private static int GetMaxPathLength(IList<DeploymentWhatIfPropertyChangeDefinition> propertyChanges)
    {
        if (propertyChanges == null)
        {
            return 0;
        }

        return propertyChanges
            .Where(ShouldConsiderPathLength)
            .Select(pc => pc.Path.Length)
            .DefaultIfEmpty()
            .Max();
    }

    private static bool ShouldConsiderPathLength(DeploymentWhatIfPropertyChangeDefinition propertyChange)
    {
        switch (propertyChange.PropertyChangeType)
        {
            case DeploymentWhatIfPropertyChangeType.Create:
            case DeploymentWhatIfPropertyChangeType.NoEffect:
                return propertyChange.After.IsLeaf();

            case DeploymentWhatIfPropertyChangeType.Delete:
            case DeploymentWhatIfPropertyChangeType.Modify:
                return propertyChange.Before.IsLeaf();

            default:
                return propertyChange.Children == null || propertyChange.Children.Count == 0;
        }
    }

    private void FormatResourceChanges(IReadOnlyList<DeploymentWhatIfResourceChangeDefinition> resourceChanges, bool definiteChanges)
    {
        if (resourceChanges == null || resourceChanges.Count == 0)
        {
            return;
        }

        int scopeCount = resourceChanges.Select(rc => ResourceIdUtility.SplitResourceId(rc.ResourceId).scope).Distinct().Count();

        // if (definiteChanges)
        // {
        //     this.Builder
        //         .AppendLine()
        //         .Append("The deployment will update the following ")
        //         .AppendLine(scopeCount == 1 ? "scope:" : "scopes:");
        // } else
        // {
        //     this.Builder
        //         .AppendLine()
        //         .AppendLine()
        //         .AppendLine()
        //         .Append("The following change MAY OR MAY NOT be deployed to the following ")
        //         .AppendLine(scopeCount == 1 ? "scope:" : "scopes:");
        // }



        resourceChanges
            .OrderBy(rc => ResourceIdUtility.SplitResourceId(rc.ResourceId).scope.ToUpperInvariant())
            .GroupBy(rc => ResourceIdUtility.SplitResourceId(rc.ResourceId).scope.ToUpperInvariant())
            .ToDictionary(g => g.Key, g => g.ToList())
            .ForEach(kvp => FormatResourceChangesInScope(ResourceIdUtility.SplitResourceId(kvp.Value[0].ResourceId).scope, kvp.Value));
    }

    private void FormatResourceChangesInScope(string scope, IList<DeploymentWhatIfResourceChangeDefinition> resourceChanges)
    {
        // Scope.
        this.Builder
            .AppendLine()
            .AppendLine($"Scope: {scope}");

        // Resource changes.
        var sortedResourceChanges = resourceChanges
            .OrderBy(rc => rc.ChangeType, new ChangeTypeComparer())
            .ThenBy(rc => ResourceIdUtility.SplitResourceId(rc.ResourceId).relativeResourceId)
            .ToImmutableArray();

        sortedResourceChanges
            .GroupBy(rc => rc.ChangeType)
            .ToDictionary(g => g.Key, g => g.ToList())
            .ForEach(kvp =>
            {
                using (this.Builder.NewColorScope(kvp.Key.ToColor()))
                {
                    kvp.Value.ForEach(rc => this.FormatResourceChange(rc, rc == sortedResourceChanges.Last()));
                }
            });
    }

    private void FormatResourceChange(DeploymentWhatIfResourceChangeDefinition resourceChange, bool isLast)
    {
        this.Builder.AppendLine();
        this.FormatResourceChangePath(
            resourceChange.ChangeType,
            ResourceIdUtility.SplitResourceId(resourceChange.ResourceId).relativeResourceId);

        switch (resourceChange.ChangeType)
        {
            case DeploymentWhatIfChangeType.Create when resourceChange.After != null:
                this.FormatJson(resourceChange.After, indentLevel: 2);

                return;

            case DeploymentWhatIfChangeType.Delete when resourceChange.Before != null:
                this.FormatJson(resourceChange.Before, indentLevel: 2);

                return;

            default:
                if (resourceChange.Delta?.Count > 0)
                {
                    using (this.Builder.NewColorScope(Color.Reset))
                    {
                        var propertyChanges = resourceChange.Delta
                            .OrderBy(pc => pc.PropertyChangeType, new PropertyChangeTypeComparer())
                            .ThenBy(pc => pc.Path)
                            .ToList();

                        this.Builder.AppendLine();
                        this.FormatPropertyChanges(propertyChanges);
                    }

                    return;
                }

                if (isLast)
                {
                    this.Builder.AppendLine();
                }

                return;
        }
    }

    private void FormatResourceChangePath(DeploymentWhatIfChangeType changeType, string relativeResourceId)
    {
        this.FormatPath(
            relativeResourceId,
            0,
            1,
            () => this.Builder.Append(changeType.ToSymbol()).Append(Symbol.WhiteSpace));
    }

    private void FormatPropertyChanges(IList<DeploymentWhatIfPropertyChangeDefinition> propertyChanges, int indentLevel = 2)
    {
        int maxPathLength = GetMaxPathLength(propertyChanges);
        propertyChanges.ForEach(pc =>
        {
            this.FormatPropertyChange(pc, maxPathLength, indentLevel);
            this.Builder.AppendLine();
        });
    }

    private void FormatPropertyChange(DeploymentWhatIfPropertyChangeDefinition propertyChange, int maxPathLength, int indentLevel)
    {
        //this.FormatHead(propertyChange, maxPathLength, indentLevel);

        DeploymentWhatIfPropertyChangeType propertyChangeType = propertyChange.PropertyChangeType;
        string path = propertyChange.Path;
        JToken before = propertyChange.Before;
        JToken after = propertyChange.After;
        IList<DeploymentWhatIfPropertyChangeDefinition> children = propertyChange.Children;

        switch (propertyChange.PropertyChangeType)
        {
            case DeploymentWhatIfPropertyChangeType.Create:
                this.FormatPropertyChangePath(propertyChangeType, path, after, children, maxPathLength, indentLevel);
                this.FormatPropertyCreate(after, indentLevel + 1);
                break;

            case DeploymentWhatIfPropertyChangeType.Delete:
                this.FormatPropertyChangePath(propertyChangeType, path, before, children, maxPathLength, indentLevel);
                this.FormatPropertyDelete(before, indentLevel + 1);
                break;

            case DeploymentWhatIfPropertyChangeType.Modify:
                this.FormatPropertyChangePath(propertyChangeType, path, before, children, maxPathLength, indentLevel);
                this.FormatPropertyModify(propertyChange, indentLevel + 1);
                break;

            case DeploymentWhatIfPropertyChangeType.Array:
                this.FormatPropertyChangePath(propertyChangeType, path, null, children, maxPathLength, indentLevel);
                this.FormatPropertyArrayChange(propertyChange, propertyChange.Children, indentLevel + 1);
                break;

            case DeploymentWhatIfPropertyChangeType.NoEffect:
                this.FormatPropertyChangePath(propertyChangeType, path, after, children, maxPathLength, indentLevel);
                this.FormatPropertyNoEffect(after, indentLevel + 1);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FormatPropertyChangePath(
        DeploymentWhatIfPropertyChangeType propertyChangeType,
        string path,
        JToken valueAfterPath,
        IList<DeploymentWhatIfPropertyChangeDefinition> children,
        int maxPathLength,
        int indentLevel)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        int paddingWidth = maxPathLength - path.Length + 1;
        bool hasChildren = children != null && children.Count > 0;

        if (valueAfterPath.IsNonEmptyArray() || (propertyChangeType == DeploymentWhatIfPropertyChangeType.Array && hasChildren))
        {
            paddingWidth = 1;
        }
        if (valueAfterPath.IsNonEmptyObject())
        {
            paddingWidth = 0;
        }
        if (propertyChangeType == DeploymentWhatIfPropertyChangeType.Modify && hasChildren)
        {
            paddingWidth = 0;
        }

        this.FormatPath(
            path,
            paddingWidth,
            indentLevel,
            () => this.FormatPropertyChangeType(propertyChangeType),
            this.FormatColon);
    }

    private void FormatPropertyChangeType(DeploymentWhatIfPropertyChangeType propertyChangeType)
    {
        this.Builder
            .Append(propertyChangeType.ToSymbol(), propertyChangeType.ToColor())
            .Append(Symbol.WhiteSpace);
    }

    private void FormatPropertyCreate(JToken value, int indentLevel)
    {
        using (this.Builder.NewColorScope(Color.Green))
        {
            this.FormatJson(value, indentLevel: indentLevel);
        }
    }

    private void FormatPropertyDelete(JToken value, int indentLevel)
    {
        using (this.Builder.NewColorScope(Color.Orange))
        {
            this.FormatJson(value, indentLevel: indentLevel);
        }
    }

    private void FormatPropertyModify(DeploymentWhatIfPropertyChangeDefinition propertyChange, int indentLevel)
    {
        if (propertyChange.Children != null)
        {
            // Has nested changes.
            this.Builder.AppendLine().AppendLine();
            this.FormatPropertyChanges(propertyChange.Children
                    .OrderBy(pc => pc.PropertyChangeType, new PropertyChangeTypeComparer())
                    .ThenBy(pc => pc.Path)
                    .ToImmutableArray(),
                indentLevel);
        }
        else
        {
            JToken before = propertyChange.Before;
            JToken after = propertyChange.After;

            // The before value.
            this.FormatPropertyDelete(before, indentLevel);

            // Space before =>
            if (before.IsNonEmptyObject())
            {
                this.Builder
                    .AppendLine()
                    .Append(Indent(indentLevel));
            }
            else
            {
                this.Builder.Append(Symbol.WhiteSpace);
            }

            // =>
            this.Builder.Append("=>");

            // Space after =>
            if (!after.IsNonEmptyObject())
            {
                this.Builder.Append(Symbol.WhiteSpace);
            }

            // The after value.
            this.FormatPropertyCreate(after, indentLevel);

            if (!before.IsLeaf() && after.IsLeaf())
            {
                this.Builder.AppendLine();
            }
        }
    }

    private void FormatPropertyArrayChange(DeploymentWhatIfPropertyChangeDefinition parentPropertyChange, IList<DeploymentWhatIfPropertyChangeDefinition> propertyChanges, int indentLevel)
    {
        if (string.IsNullOrEmpty(parentPropertyChange.Path))
        {
            // The parent change doesn't have a path, which means the current
            // array change is a nested change. We need to decrease indent_level
            // and print indentation before printing "[".
            indentLevel--;
            FormatIndent(indentLevel);
        }

        if (propertyChanges.Count == 0)
        {
            this.Builder.AppendLine("[]");
            return;
        }

        // [
        this.Builder
            .Append(Symbol.LeftSquareBracket)
            .AppendLine();

        this.FormatPropertyChanges(propertyChanges
                .OrderBy(pc => int.Parse(pc.Path))
                .ThenBy(pc => pc.PropertyChangeType, new PropertyChangeTypeComparer())
                .ToImmutableArray(),
            indentLevel);

        // ]
        this.Builder
            .Append(Indent(indentLevel))
            .Append(Symbol.RightSquareBracket);
    }

    private void FormatPropertyNoEffect(JToken value, int indentLevel)
    {
        using (this.Builder.NewColorScope(Color.Gray))
        {
            this.FormatJson(value, indentLevel: indentLevel);
        }
    }
}
