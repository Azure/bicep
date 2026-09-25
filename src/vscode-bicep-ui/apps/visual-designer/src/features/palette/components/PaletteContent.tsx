// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent } from "react";
import type { ResourceTypeReference } from "@/features/canvas";
import type { ResourceTypeCatalog, ResourceTypeNamespace } from "../types";

import { useAtomValue } from "jotai";
import { useEffect, useState } from "react";
import { OverlayScrollArea } from "@/ui";
import {
  getNamespaceResourceTypesKey,
  namespaceResourceTypesAtomFamily,
  resourceTypeCatalogLoadingCountAtom,
} from "../atoms";
import { PaletteScrollRootContext } from "../hooks/use-progressive-budget";
import { useResourceTypeSearch } from "../hooks/use-resource-type-search";
import { PaletteControls } from "./PaletteControls";
import { LazyResourceTypeGroups, PaletteMessage, PaletteRetry, SearchResourceTypeGroups } from "./ResourceTypeGroups";

export interface PaletteContentProps {
  catalogId?: string;
  namespaces?: ResourceTypeNamespace[];
  namespaceError?: unknown;
  loadNamespace: (providerNamespace: string) => Promise<ResourceTypeCatalog>;
  loadVersions: (fullyQualifiedType: string) => Promise<string[]>;
  search: (query: string) => Promise<ResourceTypeCatalog>;
  onRetryNamespaces: () => void;
  onResourceTypePointerDown?: (resourceType: ResourceTypeReference, event: PointerEvent<HTMLElement>) => void;
}

export function PaletteContent({
  catalogId,
  namespaces,
  namespaceError,
  loadNamespace,
  loadVersions,
  search,
  onRetryNamespaces,
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
  } = useResourceTypeSearch(search, catalogId);
  const namespaceLoadingCount = useAtomValue(resourceTypeCatalogLoadingCountAtom);
  const [scrollRoot, setScrollRoot] = useState<HTMLDivElement | null>(null);

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
      <OverlayScrollArea viewportRef={setScrollRoot} viewportTestId="resource-palette-list">
        <PaletteScrollRootContext.Provider value={scrollRoot}>
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
                loadVersions={loadVersions}
                groups={searchGroups}
                expandedGroups={searchExpandedGroups}
                highlightQuery={normalizedQuery}
                setExpandedGroups={setSearchExpandedGroups}
                onResourceTypePointerDown={onResourceTypePointerDown}
              />
            ) : null
          ) : !catalogId || !namespaces ? (
            <PaletteMessage>Loading resource provider namespaces...</PaletteMessage>
          ) : namespaces.length === 0 ? (
            <PaletteMessage>No resource types available.</PaletteMessage>
          ) : (
            <LazyResourceTypeGroups
              key={catalogId}
              loadVersions={loadVersions}
              catalogId={catalogId}
              namespaces={namespaces}
              loadNamespace={loadNamespace}
              onResourceTypePointerDown={onResourceTypePointerDown}
            />
          )}
        </PaletteScrollRootContext.Provider>
      </OverlayScrollArea>
    </>
  );
}
