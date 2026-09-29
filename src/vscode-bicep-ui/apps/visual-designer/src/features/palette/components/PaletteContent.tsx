// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent } from "react";
import type { ResourceTypeReference } from "@/core";
import type { ResourceTypeGroup } from "../types";

import { useState } from "react";
import { OverlayScrollArea } from "@/ui";
import { PaletteScrollRootContext } from "../hooks/use-progressive-budget";
import { useResourceTypeSearch } from "../hooks/use-resource-type-search";
import { PaletteControls } from "./PaletteControls";
import { PaletteMessage, PaletteRetry, ResourceTypeGroups } from "./ResourceTypeGroups";

export interface PaletteContentProps {
  catalogId?: string;
  groups?: ResourceTypeGroup[];
  error?: unknown;
  loadVersions: (fullyQualifiedType: string) => Promise<string[]>;
  onRetry: () => void;
  onResourceTypePointerDown?: (resourceType: ResourceTypeReference, event: PointerEvent<HTMLElement>) => void;
}

export function PaletteContent({
  catalogId,
  groups,
  error,
  loadVersions,
  onRetry,
  onResourceTypePointerDown,
}: PaletteContentProps) {
  const search = useResourceTypeSearch(groups);
  const [scrollRoot, setScrollRoot] = useState<HTMLDivElement | null>(null);
  // Browsing starts with every group collapsed, and again whenever the catalog changes.
  const [browseExpansion, setBrowseExpansion] = useState<{ catalogId?: string; groups: readonly string[] }>({
    groups: [],
  });
  const browseExpandedGroups = browseExpansion.catalogId === catalogId ? browseExpansion.groups : [];

  return (
    <>
      <PaletteControls query={search.query} setQuery={search.setQuery} showProgress={!groups && !error} />
      <OverlayScrollArea viewportRef={setScrollRoot} viewportTestId="resource-palette-list">
        <PaletteScrollRootContext.Provider value={scrollRoot}>
          {error ? (
            <PaletteMessage>
              Failed to load resource types.
              <PaletteRetry onClick={onRetry}>Retry</PaletteRetry>
            </PaletteMessage>
          ) : !catalogId || !groups ? (
            <PaletteMessage>Loading resource types...</PaletteMessage>
          ) : search.isSearching ? (
            search.searchQuery && search.results.length === 0 ? (
              <PaletteMessage>No matching resource types.</PaletteMessage>
            ) : (
              <ResourceTypeGroups
                groups={search.results}
                expandedGroups={search.expandedGroups}
                setExpandedGroups={search.setExpandedGroups}
                budgetKey={`search:${search.searchQuery}`}
                highlightQuery={search.searchQuery}
                loadVersions={loadVersions}
                onResourceTypePointerDown={onResourceTypePointerDown}
              />
            )
          ) : groups.length === 0 ? (
            <PaletteMessage>No resource types available.</PaletteMessage>
          ) : (
            <ResourceTypeGroups
              groups={groups}
              expandedGroups={browseExpandedGroups}
              setExpandedGroups={(expanded) => setBrowseExpansion({ catalogId, groups: expanded })}
              budgetKey={`browse:${catalogId}`}
              keepHeadersMounted
              loadVersions={loadVersions}
              onResourceTypePointerDown={onResourceTypePointerDown}
            />
          )}
        </PaletteScrollRootContext.Provider>
      </OverlayScrollArea>
    </>
  );
}
