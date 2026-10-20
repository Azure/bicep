// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent, ReactNode } from "react";
import type { ResourceTypeReference } from "@/core";
import type { ResourceTypeGroup } from "../types";

import { useAtomValue, useSetAtom } from "jotai";
import { useCallback, useMemo, useState } from "react";
import { OverlayScrollArea } from "@/ui";
import { paletteViewAtom, recentResourceTypesAtom } from "../atoms";
import { PaletteScrollRootContext } from "../hooks/use-progressive-budget";
import { useResourceTypeSearch } from "../hooks/use-resource-type-search";
import { allResourceTypeGroups, featuredResourceTypeGroups, findResourceTypes } from "../resource-type-groups";
import { PaletteControls } from "./PaletteControls";
import { PALETTE_VIEW_PANEL_ID, paletteViewTabId, PaletteViewTabs } from "./PaletteViewTabs";
import { RecentResourceTypes } from "./RecentResourceTypes";
import { PaletteAction, PaletteMessage, ResourceTypeGroups } from "./ResourceTypeGroups";

export interface PaletteContentProps {
  catalogId?: string;
  groups?: ResourceTypeGroup[];
  error?: unknown;
  loadVersions: (fullyQualifiedType: string) => Promise<string[]>;
  onRetry: () => void;
  onResourceTypePointerDown?: (resourceType: ResourceTypeReference, event: PointerEvent<HTMLElement>) => void;
}

/** Which groups are expanded in each grouped view, for one catalog. */
interface BrowseExpansion {
  catalogId?: string;
  featured: readonly string[];
  all: readonly string[];
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
  const view = useAtomValue(paletteViewAtom);
  const setView = useSetAtom(paletteViewAtom);
  const recentTypes = useAtomValue(recentResourceTypesAtom);
  const [scrollRoot, setScrollRoot] = useState<HTMLDivElement | null>(null);
  // Browsing starts with every group collapsed, and again whenever the catalog changes.
  const [browseExpansion, setBrowseExpansion] = useState<BrowseExpansion>({ featured: [], all: [] });
  const groupedView = view === "recent" ? undefined : view;
  const expandedGroups = groupedView && browseExpansion.catalogId === catalogId ? browseExpansion[groupedView] : [];
  const setExpandedGroups = useCallback(
    (expanded: readonly string[]) => {
      if (groupedView) {
        setBrowseExpansion((current) => ({
          ...(current.catalogId === catalogId ? current : { featured: [], all: [] }),
          catalogId,
          [groupedView]: expanded,
        }));
      }
    },
    [catalogId, groupedView],
  );
  const showAll = useCallback(() => setView("all"), [setView]);

  const viewGroups = useMemo(
    () =>
      !groups || !groupedView
        ? []
        : groupedView === "featured"
          ? featuredResourceTypeGroups(groups)
          : allResourceTypeGroups(groups),
    [groupedView, groups],
  );
  const recentResourceTypes = useMemo(
    () => (groups && view === "recent" ? findResourceTypes(groups, recentTypes) : []),
    [groups, recentTypes, view],
  );

  let browseContent: ReactNode;
  if (view === "recent") {
    browseContent =
      recentResourceTypes.length === 0 ? (
        <PaletteMessage>Resource types you drag onto the canvas appear here.</PaletteMessage>
      ) : (
        <RecentResourceTypes
          resourceTypes={recentResourceTypes}
          loadVersions={loadVersions}
          onResourceTypePointerDown={onResourceTypePointerDown}
        />
      );
  } else if (viewGroups.length === 0) {
    browseContent = (
      <PaletteMessage>
        No featured providers offer resource types for this file.
        <PaletteAction onClick={showAll}>Show all</PaletteAction>
      </PaletteMessage>
    );
  } else {
    browseContent = (
      <ResourceTypeGroups
        groups={viewGroups}
        expandedGroups={expandedGroups}
        setExpandedGroups={setExpandedGroups}
        budgetKey={`${view}:${catalogId}`}
        keepHeadersMounted
        loadVersions={loadVersions}
        onResourceTypePointerDown={onResourceTypePointerDown}
      />
    );
  }

  return (
    <>
      <PaletteControls query={search.query} setQuery={search.setQuery} showProgress={!groups && !error}>
        {/* Search covers the whole catalog, so the views apply only while browsing. */}
        {!search.isSearching && <PaletteViewTabs />}
      </PaletteControls>
      <OverlayScrollArea viewportRef={setScrollRoot} viewportTestId="resource-palette-list">
        <PaletteScrollRootContext.Provider value={scrollRoot}>
          {error ? (
            <PaletteMessage>
              Failed to load resource types.
              <PaletteAction onClick={onRetry}>Retry</PaletteAction>
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
            <div role="tabpanel" id={PALETTE_VIEW_PANEL_ID} aria-labelledby={paletteViewTabId(view)}>
              {browseContent}
            </div>
          )}
        </PaletteScrollRootContext.Provider>
      </OverlayScrollArea>
    </>
  );
}
