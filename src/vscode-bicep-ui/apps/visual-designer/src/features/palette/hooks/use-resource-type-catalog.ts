// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeGroup } from "../types";

import { useNotification } from "@vscode-bicep-ui/messaging";
import { useSetAtom } from "jotai";
import { useCallback, useEffect, useRef, useState } from "react";
import { documentDidChange } from "@/core";
import { usePaletteApi } from "../api";
import { acceptVersionCatalogAtom } from "../atoms";
import { groupResourceTypes } from "../resource-type-groups";

/** Edits arrive in bursts, so refreshes are debounced. The first load is immediate. */
const REFRESH_DEBOUNCE_MS = 250;

type CatalogState =
  | { status: "loading" }
  | { status: "loaded"; catalogId: string; groups: ResourceTypeGroup[] }
  | { status: "error"; error: unknown };

export interface ResourceTypeCatalogSource {
  catalogId?: string;
  groups?: ResourceTypeGroup[];
  error?: unknown;
  loadVersions: (fullyQualifiedType: string) => Promise<string[]>;
  refresh: () => void;
}

/**
 * Loads the resource type catalog from the host and keeps it current as the document changes.
 *
 * The whole catalog arrives in one response and is grouped and searched locally. A document change
 * re-checks it by `catalogId`, and the host resends the types only when the catalog changed. Loads are
 * matched against a generation counter so a slow response cannot overwrite a newer one.
 */
export function useResourceTypeCatalog(): ResourceTypeCatalogSource {
  const api = usePaletteApi();
  const acceptVersionCatalog = useSetAtom(acceptVersionCatalogAtom);
  const [state, setState] = useState<CatalogState>({ status: "loading" });
  const [refreshGeneration, setRefreshGeneration] = useState(0);
  const requestGenerationRef = useRef(0);
  const currentCatalogIdRef = useRef<string | undefined>(undefined);

  const refresh = useCallback(() => {
    setRefreshGeneration((generation) => generation + 1);
  }, []);

  useNotification(documentDidChange, refresh);

  useEffect(() => {
    const requestGeneration = ++requestGenerationRef.current;
    const timeout = window.setTimeout(
      () => {
        void api.listResourceTypes(currentCatalogIdRef.current).then(
          ({ catalogId, resourceTypes }) => {
            if (requestGeneration !== requestGenerationRef.current) {
              return;
            }

            // The host omits the types only when the webview already holds this catalog.
            if (resourceTypes === null) {
              if (catalogId !== currentCatalogIdRef.current) {
                currentCatalogIdRef.current = undefined;
                refresh();
              }
              return;
            }

            currentCatalogIdRef.current = catalogId;
            acceptVersionCatalog(catalogId);
            setState({ status: "loaded", catalogId, groups: groupResourceTypes(resourceTypes) });
          },
          (error: unknown) => {
            if (requestGeneration === requestGenerationRef.current) {
              // The catalog is no longer shown, so a retry must ask for all of it.
              currentCatalogIdRef.current = undefined;
              setState({ status: "error", error });
            }
          },
        );
      },
      refreshGeneration === 0 ? 0 : REFRESH_DEBOUNCE_MS,
    );

    return () => {
      window.clearTimeout(timeout);
      requestGenerationRef.current = requestGeneration + 1;
    };
  }, [acceptVersionCatalog, api, refresh, refreshGeneration]);

  const loadVersions = useCallback(
    async (fullyQualifiedType: string) => {
      const requestedCatalogId = currentCatalogIdRef.current;
      const result = await api.getVersions(fullyQualifiedType);
      if (
        !requestedCatalogId ||
        requestedCatalogId !== currentCatalogIdRef.current ||
        result.catalogId !== currentCatalogIdRef.current
      ) {
        refresh();
        throw new Error("The resource type catalog changed. Refreshing the Resource Palette.");
      }
      return result.apiVersions;
    },
    [api, refresh],
  );

  return {
    catalogId: state.status === "loaded" ? state.catalogId : undefined,
    groups: state.status === "loaded" ? state.groups : undefined,
    error: state.status === "error" ? state.error : undefined,
    loadVersions,
    refresh,
  };
}
