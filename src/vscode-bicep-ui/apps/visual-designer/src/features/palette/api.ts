// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeCatalog, ResourceTypeNamespace } from "./types";

import { defineNotification, defineRequest, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

// ── Experimental resource creation ──
// The palette is hidden entirely when the host reports the feature as disabled.

export const getResourceCreationEnablement = defineRequest<void, boolean>("resourceCreation/isEnabled");

export const resourceCreationEnablementDidChange = defineNotification<boolean>("resourceCreation/enablementDidChange");

// ── Resource type catalog ──
// The catalog is versioned by `catalogId`. The host derives it from the document's resource type
// provider, so editing the file (adding an `extension` declaration, say) can mint a new catalog;
// responses carrying a stale id must be discarded rather than merged.

export interface GetResourceTypeNamespacesResult {
  catalogId: string;
  namespaces: ResourceTypeNamespace[];
}

export const getResourceTypeNamespaces = defineRequest<void, GetResourceTypeNamespacesResult>(
  "resourceTypeCatalog/namespaces",
);

export interface LoadResourceTypeCatalogParams {
  providerNamespace?: string;
  query?: string;
  /** Load every namespace at once, so searching can filter locally instead of round-tripping. */
  loadAll?: boolean;
}

export const loadResourceTypeCatalog = defineRequest<LoadResourceTypeCatalogParams, ResourceTypeCatalog>(
  "resourceTypeCatalog/load",
);

/**
 * The palette's operations against the extension host.
 *
 * Callers get bound methods rather than a channel and a descriptor to combine themselves, so this is
 * the only place in the feature that touches the transport, and a test can substitute the whole
 * surface by stubbing this hook.
 *
 * Only imperative calls belong here. Subscriptions stay declarative at the call site via
 * `useNotification(descriptor, handler)`, which composes better with React's lifecycle.
 */
export function usePaletteApi() {
  const channel = useWebviewMessageChannel();

  return useMemo(
    () => ({
      getNamespaces: () => channel.request(getResourceTypeNamespaces),
      loadCatalog: (params: LoadResourceTypeCatalogParams) => channel.request(loadResourceTypeCatalog, params),
    }),
    [channel],
  );
}
