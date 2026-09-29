// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

/** One resource type in the catalog, at its default API version. */
export interface ResourceTypeCatalogEntry {
  resourceType: string;
  apiVersion: string;
}

/** The catalog's types in one resource provider namespace, such as `Microsoft.Storage`. */
export interface ResourceTypeGroup {
  group: string;
  resourceTypes: ResourceTypeCatalogEntry[];
}
