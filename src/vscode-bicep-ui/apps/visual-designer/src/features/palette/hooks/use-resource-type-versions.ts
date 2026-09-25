// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PaletteContentProps } from "../components/PaletteContent";

import { useAtomValue, useSetAtom, useStore } from "jotai";
import { useCallback } from "react";
import { getErrorMessage } from "@/utils";
import { resourceVersionsAtom, selectedVersionsAtom, versionCatalogIdAtom } from "../atoms";

export function useResourceTypeVersions(
  fullyQualifiedType: string,
  defaultVersion: string,
  loadVersions: PaletteContentProps["loadVersions"],
) {
  const store = useStore();
  const versions = useAtomValue(resourceVersionsAtom);
  const selections = useAtomValue(selectedVersionsAtom);
  const setSelections = useSetAtom(selectedVersionsAtom);
  const key = fullyQualifiedType.toLocaleLowerCase();
  const state = versions[key];
  const apiVersion = selections[key] ?? defaultVersion;

  const load = useCallback(async () => {
    const current = store.get(resourceVersionsAtom)[key];
    if (current?.status === "loading" || current?.status === "loaded") {
      return;
    }
    const catalogId = store.get(versionCatalogIdAtom);
    store.set(resourceVersionsAtom, (entries) => ({ ...entries, [key]: { status: "loading" } }));
    try {
      const apiVersions = await loadVersions(fullyQualifiedType);
      if (store.get(versionCatalogIdAtom) !== catalogId) {
        return;
      }
      if (apiVersions.length === 0) {
        throw new Error("No API versions available.");
      }
      store.set(resourceVersionsAtom, (entries) => ({
        ...entries,
        [key]: { status: "loaded", apiVersions },
      }));
      store.set(selectedVersionsAtom, (entries) => {
        const selected = entries[key];
        if (!selected || apiVersions.includes(selected)) {
          return entries;
        }
        const remaining = { ...entries };
        delete remaining[key];
        return remaining;
      });
    } catch (error) {
      if (store.get(versionCatalogIdAtom) === catalogId) {
        store.set(resourceVersionsAtom, (entries) => ({
          ...entries,
          [key]: { status: "error", message: getErrorMessage(error, "Failed to load API versions.") },
        }));
      }
    }
  }, [fullyQualifiedType, key, loadVersions, store]);

  const select = useCallback(
    (version: string) => {
      if (state?.status === "loaded" && state.apiVersions.includes(version)) {
        setSelections((entries) => ({ ...entries, [key]: version }));
      }
    },
    [key, setSelections, state],
  );

  return { apiVersion, state, load, select };
}
