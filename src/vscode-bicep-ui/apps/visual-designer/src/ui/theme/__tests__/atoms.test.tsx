// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// @vitest-environment happy-dom

import type { Root } from "react-dom/client";
import type { DefaultTheme } from "styled-components";

import { createStore, Provider, useAtomValue } from "jotai";
import { act, StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { activeThemeAtom, isColorThemeMatchedAtom } from "../atoms";
import { darkTheme } from "../themes";

let root: Root | undefined;
let rendered: DefaultTheme[] = [];
let store = createStore();

function Harness() {
  rendered.push(useAtomValue(activeThemeAtom));
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
        <Provider store={store}>
          <Harness />
        </Provider>
      </StrictMode>,
    ),
  );
}

/** Applies a host theme the way VS Code does: the kind on <body>, the colors on <html>. */
async function applyHostTheme(kind: string, colors: Record<string, string>) {
  await act(async () => {
    document.body.dataset.vscodeThemeKind = kind;
    for (const [name, value] of Object.entries(colors)) {
      document.documentElement.style.setProperty(`--vscode-${name}`, value);
    }
    await Promise.resolve();
  });
}

beforeEach(() => {
  vi.stubGlobal("IS_REACT_ACT_ENVIRONMENT", true);
  rendered = [];
  store = createStore();
});

afterEach(async () => {
  await act(() => root?.unmount());
  root = undefined;
  document.body.replaceChildren();
  delete document.body.dataset.vscodeThemeKind;
  document.documentElement.removeAttribute("style");
  vi.unstubAllGlobals();
});

describe("activeThemeAtom", () => {
  it("uses the theme kind present at first mount, not the one at module evaluation", async () => {
    // The atom's initial value was computed at import time, when no theme kind was set (light).
    document.body.dataset.vscodeThemeKind = "vscode-dark";

    await mount();

    expect(rendered.at(-1)?.name).toBe("dark");
  });

  it("follows later theme kind changes", async () => {
    await mount();

    await act(async () => {
      document.body.dataset.vscodeThemeKind = "vscode-high-contrast";
      await Promise.resolve();
    });

    expect(rendered.at(-1)?.name).toBe("high-contrast");
  });

  it("uses the curated palette for the theme kind unless color matching is on", async () => {
    await applyHostTheme("vscode-dark", { "editor-background": "#272822", "editor-foreground": "#f8f8f2" });
    await mount();

    expect(rendered.at(-1)).toBe(darkTheme);
  });

  it("follows color changes between two themes of the same kind when matching", async () => {
    store.set(isColorThemeMatchedAtom, true);
    await applyHostTheme("vscode-dark", { "editor-background": "#272822", "editor-foreground": "#f8f8f2" });
    await mount();
    expect(rendered.at(-1)?.viewport.background).toBe("#272822");

    await applyHostTheme("vscode-dark", { "editor-background": "#002b36", "editor-foreground": "#93a1a1" });

    expect(rendered.at(-1)?.name).toBe("dark");
    expect(rendered.at(-1)?.viewport.background).toBe("#002b36");
  });

  it("switches between curated and matched colors when the preference changes", async () => {
    await applyHostTheme("vscode-dark", { "editor-background": "#272822", "editor-foreground": "#f8f8f2" });
    await mount();

    await act(() => store.set(isColorThemeMatchedAtom, true));
    expect(rendered.at(-1)?.viewport.background).toBe("#272822");

    await act(() => store.set(isColorThemeMatchedAtom, false));
    expect(rendered.at(-1)).toBe(darkTheme);
  });

  it("does not produce a new theme for style changes that leave the colors alone", async () => {
    store.set(isColorThemeMatchedAtom, true);
    await applyHostTheme("vscode-dark", { "editor-background": "#272822" });
    await mount();
    const theme = rendered.at(-1);

    await act(async () => {
      document.documentElement.style.setProperty("--vscode-font-family", "monospace");
      await Promise.resolve();
    });

    expect(rendered.at(-1)).toBe(theme);
  });
});
