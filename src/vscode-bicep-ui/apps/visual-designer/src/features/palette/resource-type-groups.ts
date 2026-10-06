// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/core";
import type { ResourceTypeGroup } from "./types";

const featuredNamespaces = [
  "Microsoft.Compute",
  "Microsoft.Network",
  "Microsoft.Storage",
  "Microsoft.Web",
  "Microsoft.App",
  "Microsoft.ContainerService",
  "Microsoft.ContainerRegistry",
  "Microsoft.KeyVault",
  "Microsoft.ManagedIdentity",
  "Microsoft.Authorization",
  "Microsoft.Resources",
  "Microsoft.Sql",
  "Microsoft.DBforPostgreSQL",
  "Microsoft.DBforMySQL",
  "Microsoft.DocumentDB",
  "Microsoft.Cache",
  "Microsoft.CognitiveServices",
  "Microsoft.Insights",
  "Microsoft.OperationalInsights",
  "Microsoft.ServiceBus",
  "Microsoft.EventHub",
];

const featuredRanks = new Map(featuredNamespaces.map((name, index) => [name.toLowerCase(), index]));

/** Featured providers first, then other Microsoft providers, then third-party providers, each alphabetically. */
export function compareNamespaces(left: string, right: string): number {
  const leftName = left.toLowerCase();
  const rightName = right.toLowerCase();
  const leftRank = featuredRanks.get(leftName) ?? featuredNamespaces.length;
  const rightRank = featuredRanks.get(rightName) ?? featuredNamespaces.length;

  return (
    leftRank - rightRank ||
    Number(rightName.startsWith("microsoft.")) - Number(leftName.startsWith("microsoft.")) ||
    leftName.localeCompare(rightName)
  );
}

/** Group resource types by provider namespace, in browsing order. */
export function groupResourceTypes(resourceTypes: readonly ResourceTypeReference[]): ResourceTypeGroup[] {
  const groups = new Map<string, ResourceTypeGroup>();

  for (const { fullyQualifiedType, apiVersion } of resourceTypes) {
    const separator = fullyQualifiedType.indexOf("/");
    if (separator <= 0) {
      continue;
    }

    const name = fullyQualifiedType.slice(0, separator);
    const group = groups.get(name) ?? { group: name, resourceTypes: [] };
    group.resourceTypes.push({ resourceType: fullyQualifiedType.slice(separator + 1), apiVersion });
    groups.set(name, group);
  }

  for (const group of groups.values()) {
    group.resourceTypes.sort((left, right) => left.resourceType.localeCompare(right.resourceType));
  }

  return [...groups.values()].sort((left, right) => compareNamespaces(left.group, right.group));
}

/** The types whose fully qualified name contains `query`, ignoring case. Groups left empty are dropped. */
export function filterResourceTypeGroups(groups: readonly ResourceTypeGroup[], query: string): ResourceTypeGroup[] {
  const normalizedQuery = query.toLocaleLowerCase();

  return groups
    .map((group) => ({
      ...group,
      resourceTypes: group.resourceTypes.filter(({ resourceType }) =>
        `${group.group}/${resourceType}`.toLocaleLowerCase().includes(normalizedQuery),
      ),
    }))
    .filter((group) => group.resourceTypes.length > 0);
}
