// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @vitest-environment happy-dom

import type { Root } from "react-dom/client";

import { createStore, Provider, useAtomValue } from "jotai";
import { act, StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { activeThemeAtom } from "../atoms";

let root: Root | undefined;
let rendered: string[] = [];

function Harness() {
  rendered.push(useAtomValue(activeThemeAtom).name);
  return null;
}

async function mount() {
  const container = document.createElement("div");
  document.body.append(container);
  root = createRoot(container);
  await act(() =>
    root?.render(
      // The app renders under StrictMode; its mount/unmount/remount of effects is part of the race.
      <StrictMode>
        <Provider store={createStore()}>
          <Harness />
        </Provider>
      </StrictMode>,
    ),
  );
}

beforeEach(() => {
  vi.stubGlobal("IS_REACT_ACT_ENVIRONMENT", true);
  rendered = [];
});

afterEach(async () => {
  await act(() => root?.unmount());
  root = undefined;
  document.body.replaceChildren();
  delete document.body.dataset.vscodeThemeKind;
  vi.unstubAllGlobals();
});

describe("activeThemeAtom", () => {
  it("uses the theme kind present at first mount, not the one at module evaluation", async () => {
    // The atom's initial value was computed at import time, when no theme kind was set (light).
    document.body.dataset.vscodeThemeKind = "vscode-dark";

    await mount();

    expect(rendered.at(-1)).toBe("dark");
  });

  it("follows later theme kind changes", async () => {
    await mount();

    await act(async () => {
      document.body.dataset.vscodeThemeKind = "vscode-high-contrast";
      await Promise.resolve();
    });

    expect(rendered.at(-1)).toBe("high-contrast");
  });
});
