// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeCatalog, ResourceTypeNamespace } from "../types";

import { useWebviewMessageChannel, useWebviewNotification } from "@vscode-bicep-ui/messaging";
import { useCallback, useEffect, useRef, useState } from "react";
import {
  DOCUMENT_DID_CHANGE_NOTIFICATION,
  GET_RESOURCE_TYPE_NAMESPACES_REQUEST,
  LOAD_RESOURCE_TYPE_CATALOG_REQUEST,
} from "@/lib/messaging";

/** Edits arrive in bursts, so refreshes are debounced. The first load is immediate. */
const REFRESH_DEBOUNCE_MS = 250;

interface ResourceTypeNamespaceCatalog {
  catalogId: string;
  namespaces: ResourceTypeNamespace[];
}

type NamespaceCatalogState =
  | { status: "loading" }
  | { status: "loaded"; catalog: ResourceTypeNamespaceCatalog }
  | { status: "error"; error: unknown };

export interface ResourceTypeCatalogSource {
  catalogId?: string;
  namespaces?: ResourceTypeNamespace[];
  namespaceError?: unknown;
  loadNamespace: (providerNamespace: string) => Promise<ResourceTypeCatalog>;
  search: (query: string) => Promise<ResourceTypeCatalog>;
  refresh: () => void;
}

/**
 * Loads the resource-type catalog from the host and keeps it current as the document changes.
 *
 * The catalog is versioned by `catalogId`: the host may rebuild it at any time, and a response from an
 * older catalog cannot be mixed with newer namespace data. Every load therefore checks the id it came
 * back with and forces a refresh on mismatch, and in-flight namespace requests are matched against a
 * generation counter so a slow response cannot overwrite a newer one.
 */
export function useResourceTypeCatalog(): ResourceTypeCatalogSource {
  const messageChannel = useWebviewMessageChannel();
  const [namespaceCatalogState, setNamespaceCatalogState] = useState<NamespaceCatalogState>({ status: "loading" });
  const [refreshGeneration, setRefreshGeneration] = useState(0);
  const namespaceRequestGenerationRef = useRef(0);
  const searchableCatalogRef = useRef<ResourceTypeCatalog | undefined>(undefined);

  const refresh = useCallback(() => {
    setRefreshGeneration((generation) => generation + 1);
  }, []);

  useWebviewNotification(
    DOCUMENT_DID_CHANGE_NOTIFICATION,
    useCallback(() => refresh(), [refresh]),
  );

  useEffect(() => {
    const requestGeneration = ++namespaceRequestGenerationRef.current;
    const timeout = window.setTimeout(
      () => {
        setNamespaceCatalogState((current) => (current.status === "loaded" ? current : { status: "loading" }));
        void messageChannel
          .sendRequest<ResourceTypeNamespaceCatalog>({ method: GET_RESOURCE_TYPE_NAMESPACES_REQUEST })
          .then(
            (catalog) => {
              if (requestGeneration === namespaceRequestGenerationRef.current) {
                if (searchableCatalogRef.current?.catalogId !== catalog.catalogId) {
                  searchableCatalogRef.current = undefined;
                }
                setNamespaceCatalogState({ status: "loaded", catalog });
              }
            },
            (error: unknown) => {
              if (requestGeneration === namespaceRequestGenerationRef.current) {
                setNamespaceCatalogState({ status: "error", error });
              }
            },
          );
      },
      refreshGeneration === 0 ? 0 : REFRESH_DEBOUNCE_MS,
    );

    return () => window.clearTimeout(timeout);
  }, [messageChannel, refreshGeneration]);

  const requestCatalog = useCallback(
    async (params: { providerNamespace?: string; query?: string; loadAll?: boolean }): Promise<ResourceTypeCatalog> => {
      const catalog = await messageChannel.sendRequest<ResourceTypeCatalog>({
        method: LOAD_RESOURCE_TYPE_CATALOG_REQUEST,
        params,
      });
      const currentCatalogId =
        namespaceCatalogState.status === "loaded" ? namespaceCatalogState.catalog.catalogId : undefined;

      if (!currentCatalogId || currentCatalogId !== catalog.catalogId) {
        refresh();
        throw new Error("The resource type catalog changed. Refreshing the Resource Palette.");
      }

      return catalog;
    },
    [messageChannel, namespaceCatalogState, refresh],
  );

  const loadNamespace = useCallback(
    (providerNamespace: string) => requestCatalog({ providerNamespace }),
    [requestCatalog],
  );

  const search = useCallback(
    async (query: string): Promise<ResourceTypeCatalog> => {
      // Searching needs every namespace, so the full catalog is fetched once and filtered locally.
      let catalog = searchableCatalogRef.current;
      if (!catalog) {
        catalog = await requestCatalog({ loadAll: true });
        searchableCatalogRef.current = catalog;
      }

      const normalizedQuery = query.toLocaleLowerCase();
      return {
        catalogId: catalog.catalogId,
        groups: catalog.groups
          .map((group) => ({
            ...group,
            resourceTypes: group.resourceTypes.filter((resourceType) =>
              `${group.group}/${resourceType.resourceType}`.toLocaleLowerCase().includes(normalizedQuery),
            ),
          }))
          .filter((group) => group.resourceTypes.length > 0),
      };
    },
    [requestCatalog],
  );

  return {
    catalogId: namespaceCatalogState.status === "loaded" ? namespaceCatalogState.catalog.catalogId : undefined,
    namespaces: namespaceCatalogState.status === "loaded" ? namespaceCatalogState.catalog.namespaces : undefined,
    namespaceError: namespaceCatalogState.status === "error" ? namespaceCatalogState.error : undefined,
    loadNamespace,
    search,
    refresh,
  };
}
