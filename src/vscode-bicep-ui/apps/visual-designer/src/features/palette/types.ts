// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeCatalogEntry } from "./atoms";

export interface ResourceTypeNamespace {
  name: string;
  resourceTypeCount: number;
}

export interface ResourceTypeCatalogGroup {
  group: string;
  resourceTypes: ResourceTypeCatalogEntry[];
}

export interface ResourceTypeCatalog {
  catalogId: string;
  groups: ResourceTypeCatalogGroup[];
}

/** A resource type the user can create. Mirrors the host's resource-creation contract. */
export interface ResourceTypeReference {
  fullyQualifiedType: string;
  apiVersion: string;
}
