// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PaletteContentProps } from "../components/PaletteContent";
import type { ResourceTypeCatalogGroup } from "../types";

import { useEffect, useRef, useState } from "react";
import { getErrorMessage } from "@/lib/utils";

type SearchState =
  | { status: "idle" }
  | { status: "loading"; query: string }
  | { status: "loaded"; query: string; groups: ResourceTypeCatalogGroup[] }
  | { status: "error"; query: string; message: string };

export function useResourceTypeSearch(search: PaletteContentProps["search"]) {
  const [query, setQuery] = useState("");
  const [expandedGroups, setExpandedGroups] = useState<readonly string[]>([]);
  const [state, setState] = useState<SearchState>({ status: "idle" });
  const requestGenerationRef = useRef(0);

  useEffect(() => {
    const normalizedQuery = query.trim();
    const generation = ++requestGenerationRef.current;
    if (!normalizedQuery) {
      return;
    }

    const timeout = window.setTimeout(() => {
      setState({ status: "loading", query: normalizedQuery });
      void search(normalizedQuery).then(
        (catalog) => {
          if (generation === requestGenerationRef.current) {
            setState({ status: "loaded", query: normalizedQuery, groups: catalog.groups });
            setExpandedGroups(catalog.groups.map((group) => group.group));
          }
        },
        (error: unknown) => {
          if (generation === requestGenerationRef.current) {
            setState({
              status: "error",
              query: normalizedQuery,
              message: getErrorMessage(error, "Resource type search failed."),
            });
          }
        },
      );
    }, 250);

    return () => window.clearTimeout(timeout);
  }, [query, search]);

  const normalizedQuery = query.trim();
  const activeState =
    state.status !== "idle" && state.query === normalizedQuery ? state : ({ status: "idle" } as const);

  return {
    activeState,
    expandedGroups,
    isSearching: normalizedQuery.length > 0,
    normalizedQuery,
    query,
    setExpandedGroups,
    setQuery,
  };
}
