// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { VisualResourceTypeCatalogItem } from "./protocol";

export interface ResourceTypeNamespace {
  name: string;
  resourceTypeCount: number;
}

export interface ResourceTypeNamespaceCatalog {
  catalogId: string;
  namespaces: ResourceTypeNamespace[];
}

export interface ResourceTypeCatalogGroup {
  group: string;
  resourceTypes: {
    resourceType: string;
    apiVersion: string;
  }[];
}

export interface ResourceTypeCatalog {
  catalogId: string;
  groups: ResourceTypeCatalogGroup[];
}

export function buildResourceTypeCatalog(items: readonly VisualResourceTypeCatalogItem[]): ResourceTypeCatalogGroup[] {
  const grouped = new Map<string, ResourceTypeCatalogGroup["resourceTypes"]>();
  for (const { fullyQualifiedType, apiVersion } of items) {
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
