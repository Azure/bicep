// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { VisualResourceTypeCatalogItem } from "./protocol";

export interface ResourceTypeCatalogGroup {
  group: string;
  resourceTypes: {
    resourceType: string;
    apiVersion: string;
  }[];
}

export function buildResourceTypeCatalog(items: readonly VisualResourceTypeCatalogItem[]): ResourceTypeCatalogGroup[] {
  const latestByType = new Map<string, string>();
  for (const item of items) {
    if (!latestByType.has(item.fullyQualifiedType)) {
      latestByType.set(item.fullyQualifiedType, item.apiVersion);
    }
  }

  const grouped = new Map<string, ResourceTypeCatalogGroup["resourceTypes"]>();
  for (const [fullyQualifiedType, apiVersion] of latestByType) {
    const [group, ...typeSegments] = fullyQualifiedType.split("/");
    if (!group || typeSegments.length === 0) {
      continue;
    }

    const resourceTypes = grouped.get(group) ?? [];
    resourceTypes.push({ resourceType: typeSegments.join("/"), apiVersion });
    grouped.set(group, resourceTypes);
  }

  return [...grouped]
    .sort(([left], [right]) => left.localeCompare(right))
    .map(([group, resourceTypes]) => ({
      group,
      resourceTypes: resourceTypes.sort((left, right) => left.resourceType.localeCompare(right.resourceType)),
    }));
}
