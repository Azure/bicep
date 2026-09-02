// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @vitest-environment happy-dom
// The export atoms reach `ui/theme`, which reads `document.body` at module scope.

import { createStore } from "jotai";
import { describe, expect, it } from "vitest";
import { documentUriAtom } from "@/hooks";
import { DEFAULT_EXPORT_FILE_STEM, exportFileStemAtom } from "../atoms";

describe("export file stem", () => {
  it.each([
    ["file:///c:/src/main.bicep", "main"],
    ["file:///src/my.module.bicep", "my.module"],
    ["c:\\src\\windows-path.bicep", "windows-path"],
    ["file:///noextension", "noextension"],
  ])("derives %s as %s", (documentUri, expected) => {
    const store = createStore();
    store.set(documentUriAtom, documentUri);

    expect(store.get(exportFileStemAtom)).toBe(expected);
  });

  it.each([[null], [""], ["file:///.bicep"]])("falls back to the default for %s", (documentUri) => {
    const store = createStore();
    store.set(documentUriAtom, documentUri);

    expect(store.get(exportFileStemAtom)).toBe(DEFAULT_EXPORT_FILE_STEM);
  });

  it("tracks the document without anyone pushing the name across", () => {
    const store = createStore();
    store.set(documentUriAtom, "file:///first.bicep");
    expect(store.get(exportFileStemAtom)).toBe("first");

    store.set(documentUriAtom, "file:///second.bicep");
    expect(store.get(exportFileStemAtom)).toBe("second");
  });
});
