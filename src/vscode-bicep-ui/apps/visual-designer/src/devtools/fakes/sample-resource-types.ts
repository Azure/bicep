// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference, TargetScope } from "@/core";

const everyScope: TargetScope[] = ["resourceGroup", "subscription", "managementGroup", "tenant"];

/**
 * Synthesizes `size` extra resource types shaped like the real Azure catalog (one namespace the size of
 * Microsoft.Network, the rest about ten types each) so rendering and search can be profiled at scale.
 */
function createSyntheticCatalog(size: number, scopes: TargetScope[]) {
  if (!Number.isFinite(size) || size <= 0) {
    return [];
  }

  const largeNamespaceSize = Math.min(166, size);
  const groups = [
    {
      group: "Microsoft.SyntheticLarge",
      resourceTypes: Array.from({ length: largeNamespaceSize }, (_, index) => ({
        resourceType: `resource${index}`,
        apiVersion: "2024-01-01",
        scopes,
      })),
    },
  ];
  for (let offset = largeNamespaceSize, groupIndex = 0; offset < size; offset += 10, groupIndex++) {
    groups.push({
      group: `Microsoft.Synthetic${String(groupIndex).padStart(3, "0")}`,
      resourceTypes: Array.from({ length: Math.min(10, size - offset) }, (_, index) => ({
        resourceType: `resource${index}`,
        apiVersion: "2024-01-01",
        scopes,
      })),
    });
  }

  return groups;
}

/**
 * The sample catalog at each type's default version. Like the language server, it offers only types
 * deployable at the document's target scope. `catalogSize` adds synthetic types for profiling.
 */
export function getSampleResourceTypes(catalogRevision: number, targetScope: TargetScope): ResourceTypeReference[] {
  const groups = [
    {
      group: "Microsoft.Storage",
      resourceTypes: [
        {
          resourceType: "storageAccounts",
          apiVersion: catalogRevision ? "2026-01-01" : "2025-01-01",
          scopes: ["resourceGroup"],
        },
      ],
    },
    {
      group: "Microsoft.Network",
      resourceTypes: [{ resourceType: "virtualNetworks", apiVersion: "2024-07-01", scopes: ["resourceGroup"] }],
    },
    {
      group: "Microsoft.Resources",
      resourceTypes: [{ resourceType: "resourceGroups", apiVersion: "2025-04-01", scopes: ["subscription"] }],
    },
    {
      group: "Microsoft.Management",
      resourceTypes: [{ resourceType: "managementGroups", apiVersion: "2023-04-01", scopes: ["tenant"] }],
    },
    {
      group: "Microsoft.Preview",
      resourceTypes: [{ resourceType: "widgets", apiVersion: "2026-01-01-preview", scopes: everyScope }],
    },
    ...createSyntheticCatalog(Number(new URLSearchParams(window.location.search).get("catalogSize") ?? 0), everyScope),
  ];

  return groups.flatMap(({ group, resourceTypes }) =>
    resourceTypes
      .filter(({ scopes }) => scopes.includes(targetScope))
      .map(({ resourceType, apiVersion }) => ({ fullyQualifiedType: `${group}/${resourceType}`, apiVersion })),
  );
}

/** Every API version of a sample type, newest first. */
export function getSampleApiVersions(fullyQualifiedType: string, catalogRevision: number): string[] {
  const versions: Record<string, string[]> = {
    "Microsoft.Storage/storageAccounts": catalogRevision
      ? ["2026-02-01-preview", "2026-01-01"]
      : ["2026-02-01-preview", "2025-01-01", "2024-01-01"],
    "Microsoft.Network/virtualNetworks": ["2025-01-01-preview", "2024-07-01", "2023-11-01"],
    "Microsoft.Resources/resourceGroups": ["2025-04-01", "2022-09-01"],
    "Microsoft.Management/managementGroups": ["2023-04-01", "2021-04-01"],
    "Microsoft.Preview/widgets": ["2026-01-01-preview", "2025-01-01-preview"],
  };

  return versions[fullyQualifiedType] ?? [];
}

/**
 * Resource-catalog responses are deliberately delayed so the dev shell exercises loading states.
 * The `catalogDelay` query parameter overrides that delay (in milliseconds) so end-to-end tests can
 * hold the loading state open long enough to assert on it instead of racing the default timing.
 */
export function getCatalogDelayMs(defaultDelayMs: number): number {
  const raw = new URLSearchParams(window.location.search).get("catalogDelay");

  if (raw === null) {
    return defaultDelayMs;
  }

  const override = Number(raw);

  return Number.isFinite(override) && override >= 0 ? override : defaultDelayMs;
}
