// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceVersionsState, VersionCatalog } from "../atoms";
import type { PaletteContentProps } from "../components/PaletteContent";

import { atom, useAtomValue, useStore } from "jotai";
import { useCallback, useMemo } from "react";
import { getErrorMessage } from "@/utils";
import { versionCatalogAtom } from "../atoms";

export function useResourceTypeVersions(
  fullyQualifiedType: string,
  defaultVersion: string,
  loadVersions: PaletteContentProps["loadVersions"],
) {
  const store = useStore();
  const key = fullyQualifiedType.toLocaleLowerCase();
  // Each row reads only its own entries, so loading one type's versions re-renders only that row.
  const stateAtom = useMemo(() => atom((get) => get(versionCatalogAtom).versions[key]), [key]);
  const selectionAtom = useMemo(() => atom((get) => get(versionCatalogAtom).selections[key]), [key]);
  const state = useAtomValue(stateAtom);
  const apiVersion = useAtomValue(selectionAtom) ?? defaultVersion;

  const load = useCallback(async () => {
    const { catalogId, versions } = store.get(versionCatalogAtom);
    const current = versions[key];
    if (current?.status === "loading" || current?.status === "loaded") {
      return;
    }

    // A response for a catalog that has since been replaced is dropped.
    const update = (change: (catalog: VersionCatalog) => VersionCatalog) =>
      store.set(versionCatalogAtom, (catalog) => (catalog.catalogId === catalogId ? change(catalog) : catalog));
    const setState = (versionsState: ResourceVersionsState) =>
      update((catalog) => ({ ...catalog, versions: { ...catalog.versions, [key]: versionsState } }));

    setState({ status: "loading" });
    try {
      const apiVersions = await loadVersions(fullyQualifiedType);
      if (apiVersions.length === 0) {
        throw new Error("No API versions available.");
      }
      update(({ selections, ...catalog }) => {
        // Forget a choice the host no longer offers.
        const { [key]: selected, ...others } = selections;
        return {
          ...catalog,
          versions: { ...catalog.versions, [key]: { status: "loaded", apiVersions } },
          selections: !selected || apiVersions.includes(selected) ? selections : others,
        };
      });
    } catch (error) {
      setState({ status: "error", message: getErrorMessage(error, "Failed to load API versions.") });
    }
  }, [fullyQualifiedType, key, loadVersions, store]);

  const select = useCallback(
    (version: string) => {
      if (state?.status === "loaded" && state.apiVersions.includes(version)) {
        store.set(versionCatalogAtom, (catalog) => ({
          ...catalog,
          selections: { ...catalog.selections, [key]: version },
        }));
      }
    },
    [key, state, store],
  );

  return { apiVersion, state, load, select };
}
