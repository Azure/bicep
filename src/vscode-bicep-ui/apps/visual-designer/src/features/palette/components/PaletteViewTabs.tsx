// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { KeyboardEvent, MouseEvent } from "react";
import type { PaletteView } from "../atoms";

import { useAtomValue, useSetAtom } from "jotai";
import { useCallback, useRef } from "react";
import styled from "styled-components";
import { paletteViewAtom } from "../atoms";

const VIEWS: readonly { view: PaletteView; label: string; title: string }[] = [
  { view: "featured", label: "Featured", title: "Common Azure resource providers" },
  { view: "recent", label: "Recent", title: "Resource types recently added to the canvas" },
  { view: "all", label: "All", title: "Every resource provider, alphabetically" },
];

export const PALETTE_VIEW_PANEL_ID = "resource-palette-view";

export function paletteViewTabId(view: PaletteView): string {
  return `resource-palette-view-${view}`;
}

const $Tabs = styled.div`
  display: flex;
  gap: 2px;
  margin-top: 6px;
`;

const $Tab = styled.button<{ $selected: boolean }>`
  height: 22px;
  padding: 0 10px;
  border: 1px solid
    ${({ $selected }) => ($selected ? "var(--vscode-contrastActiveBorder, transparent)" : "transparent")};
  border-radius: 11px;
  color: ${({ theme, $selected }) => ($selected ? theme.text.primary : theme.text.secondary)};
  background: ${({ theme, $selected }) => ($selected ? theme.iconButton.activeBackground : "transparent")};
  font: inherit;
  font-size: 11px;
  font-weight: ${({ $selected }) => ($selected ? 600 : 400)};
  cursor: pointer;
  transition:
    background-color 150ms ease,
    color 150ms ease;

  &:hover {
    color: ${({ theme }) => theme.text.primary};
    background: ${({ theme, $selected }) =>
      $selected ? theme.iconButton.activeBackground : theme.iconButton.hoverBackground};
  }

  &:focus-visible {
    outline: 1px solid ${({ theme }) => theme.focusBorder};
    outline-offset: -1px;
  }
`;

/** Switches what browsing shows. Arrow keys, Home, and End move between views, as in any tab list. */
export function PaletteViewTabs() {
  const selectedView = useAtomValue(paletteViewAtom);
  const setView = useSetAtom(paletteViewAtom);
  const tabsRef = useRef<HTMLDivElement>(null);

  const selectView = useCallback(
    (event: MouseEvent<HTMLButtonElement>) => setView(event.currentTarget.dataset.view as PaletteView),
    [setView],
  );

  const handleKeyDown = useCallback(
    (event: KeyboardEvent<HTMLDivElement>) => {
      const index = VIEWS.findIndex(({ view }) => view === selectedView);
      const nextIndex =
        event.key === "ArrowRight"
          ? (index + 1) % VIEWS.length
          : event.key === "ArrowLeft"
            ? (index - 1 + VIEWS.length) % VIEWS.length
            : event.key === "Home"
              ? 0
              : event.key === "End"
                ? VIEWS.length - 1
                : undefined;
      const next = nextIndex === undefined ? undefined : VIEWS[nextIndex];
      if (!next) {
        return;
      }

      event.preventDefault();
      setView(next.view);
      tabsRef.current?.querySelector<HTMLElement>(`#${paletteViewTabId(next.view)}`)?.focus();
    },
    [selectedView, setView],
  );

  return (
    <$Tabs ref={tabsRef} role="tablist" aria-label="Resource type views" onKeyDown={handleKeyDown}>
      {VIEWS.map(({ view, label, title }) => (
        <$Tab
          key={view}
          id={paletteViewTabId(view)}
          type="button"
          role="tab"
          title={title}
          aria-selected={view === selectedView}
          aria-controls={PALETTE_VIEW_PANEL_ID}
          tabIndex={view === selectedView ? 0 : -1}
          data-view={view}
          $selected={view === selectedView}
          onClick={selectView}
        >
          {label}
        </$Tab>
      ))}
    </$Tabs>
  );
}
