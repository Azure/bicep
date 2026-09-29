// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/core";

import { defineRequest, useWebviewMessageChannel } from "@vscode-bicep-ui/messaging";
import { useMemo } from "react";

// ── Resource type catalog ──
// The catalog is versioned by `catalogId`. The host derives it from the document's resource type
// provider and target scope, so editing the file (adding an `extension` declaration, say) can mint a
// new catalog; responses carrying a stale id must be discarded rather than merged.

export interface ListResourceTypesParams {
  /** The catalog the webview already holds, so the host can skip sending it again. */
  knownCatalogId?: string;
}

export interface ListResourceTypesResult {
  catalogId: string;
  /** Every type deployable at the document's target scope, at its default API version, or null if unchanged. */
  resourceTypes: ResourceTypeReference[] | null;
}

export const listResourceTypes = defineRequest<ListResourceTypesParams, ListResourceTypesResult>("resourceTypes/list");

export interface ResourceTypeVersions {
  catalogId: string;
  apiVersions: string[];
}

export const getResourceTypeVersions = defineRequest<{ fullyQualifiedType: string }, ResourceTypeVersions>(
  "resourceTypes/versions",
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
      listResourceTypes: (knownCatalogId?: string) => channel.request(listResourceTypes, { knownCatalogId }),
      getVersions: (fullyQualifiedType: string) => channel.request(getResourceTypeVersions, { fullyQualifiedType }),
    }),
    [channel],
  );
}
