// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent } from "react";
import type { ResourceTypeReference } from "@/features/canvas";
import type { ResourceTypeCatalog, ResourceTypeNamespace } from "../types";

import { useAtomValue } from "jotai";
import { useEffect } from "react";
import {
  getNamespaceResourceTypesKey,
  namespaceResourceTypesAtomFamily,
  resourceTypeCatalogLoadingCountAtom,
} from "../atoms";
import { useResourceTypeSearch } from "../hooks/use-resource-type-search";
import { PaletteControls } from "./PaletteControls";
import { LazyResourceTypeGroups, PaletteMessage, PaletteRetry, SearchResourceTypeGroups } from "./ResourceTypeGroups";

export interface PaletteContentProps {
  catalogId?: string;
  namespaces?: ResourceTypeNamespace[];
  namespaceError?: unknown;
  loadNamespace: (providerNamespace: string) => Promise<ResourceTypeCatalog>;
  search: (query: string) => Promise<ResourceTypeCatalog>;
  onRetryNamespaces: () => void;
  onResourceTypeActivate?: (resourceType: ResourceTypeReference) => void;
  onResourceTypePointerDown?: (resourceType: ResourceTypeReference, event: PointerEvent<HTMLButtonElement>) => void;
}

export function PaletteContent({
  catalogId,
  namespaces,
  namespaceError,
  loadNamespace,
  search,
  onRetryNamespaces,
  onResourceTypeActivate,
  onResourceTypePointerDown,
}: PaletteContentProps) {
  const {
    activeState: searchState,
    expandedGroups: searchExpandedGroups,
    isSearching,
    normalizedQuery,
    query,
    setExpandedGroups: setSearchExpandedGroups,
    setQuery,
  } = useResourceTypeSearch(search);
  const namespaceLoadingCount = useAtomValue(resourceTypeCatalogLoadingCountAtom);

  useEffect(
    () => () => {
      if (catalogId && namespaces) {
        for (const namespace of namespaces) {
          namespaceResourceTypesAtomFamily.remove(getNamespaceResourceTypesKey(catalogId, namespace.name));
        }
      }
    },
    [catalogId, namespaces],
  );

  const searchGroups = searchState.status === "loaded" ? searchState.groups : [];
  const showProgress =
    (!namespaces && !namespaceError) || (isSearching && searchState.status === "loading") || namespaceLoadingCount > 0;

  return (
    <>
      <PaletteControls query={query} setQuery={setQuery} showProgress={showProgress} />
      {namespaceError ? (
        <PaletteMessage>
          Failed to load resource provider namespaces.
          <PaletteRetry onClick={onRetryNamespaces}>Retry</PaletteRetry>
        </PaletteMessage>
      ) : isSearching ? (
        searchState.status === "error" ? (
          <PaletteMessage>{searchState.message}</PaletteMessage>
        ) : searchState.status === "loaded" && searchGroups.length === 0 ? (
          <PaletteMessage>No matching resource types.</PaletteMessage>
        ) : searchState.status === "loaded" ? (
          <SearchResourceTypeGroups
            groups={searchGroups}
            expandedGroups={searchExpandedGroups}
            highlightQuery={normalizedQuery}
            setExpandedGroups={setSearchExpandedGroups}
            onResourceTypeActivate={onResourceTypeActivate}
            onResourceTypePointerDown={onResourceTypePointerDown}
          />
        ) : null
      ) : !catalogId || !namespaces ? (
        <PaletteMessage>Loading resource provider namespaces...</PaletteMessage>
      ) : namespaces.length === 0 ? (
        <PaletteMessage>No resource types available.</PaletteMessage>
      ) : (
        <LazyResourceTypeGroups
          catalogId={catalogId}
          namespaces={namespaces}
          loadNamespace={loadNamespace}
          onResourceTypeActivate={onResourceTypeActivate}
          onResourceTypePointerDown={onResourceTypePointerDown}
        />
      )}
    </>
  );
}
