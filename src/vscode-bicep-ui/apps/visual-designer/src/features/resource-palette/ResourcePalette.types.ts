// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeCatalogEntry } from "./atoms";
import type { PointerEvent } from "react";

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

export interface ResourceTypeReference {
  fullyQualifiedType: string;
  apiVersion: string;
}

export interface ResourcePaletteProps {
  catalogId?: string;
  namespaces?: ResourceTypeNamespace[];
  namespaceError?: unknown;
  loadNamespace: (providerNamespace: string) => Promise<ResourceTypeCatalog>;
  search: (query: string) => Promise<ResourceTypeCatalog>;
  onRetryNamespaces: () => void;
  onResourceTypeActivate?: (resourceType: ResourceTypeReference) => void;
  onResourceTypePointerDown?: (resourceType: ResourceTypeReference, event: PointerEvent<HTMLButtonElement>) => void;
}
