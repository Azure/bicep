// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeGroup } from "../types";

import { useCallback, useDeferredValue, useMemo, useState } from "react";
import { filterResourceTypeGroups } from "../resource-type-groups";

/**
 * Filters the catalog by the search query. Every match's group starts expanded, and collapsing one lasts until
 * the query changes.
 */
export function useResourceTypeSearch(groups: readonly ResourceTypeGroup[] | undefined) {
  const [query, setQuery] = useState("");
  const normalizedQuery = query.trim();
  // Filtering thousands of types on every keystroke would delay typing, so the results trail the input.
  const searchQuery = useDeferredValue(normalizedQuery);
  const results = useMemo(
    () => (groups && searchQuery ? filterResourceTypeGroups(groups, searchQuery) : []),
    [groups, searchQuery],
  );
  const [expansion, setExpansion] = useState<{ query: string; groups: readonly string[] } | null>(null);
  const expandedGroups = useMemo(
    () => (expansion?.query === searchQuery ? expansion.groups : results.map(({ group }) => group)),
    [expansion, results, searchQuery],
  );
  const setExpandedGroups = useCallback(
    (expanded: readonly string[]) => setExpansion({ query: searchQuery, groups: expanded }),
    [searchQuery],
  );

  return {
    query,
    setQuery,
    isSearching: normalizedQuery.length > 0,
    searchQuery,
    results,
    expandedGroups,
    setExpandedGroups,
  };
}
