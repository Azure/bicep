// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { KeyboardEvent, MouseEvent } from "react";
import type { ResourceVersionsState } from "../atoms";

import { Codicon } from "@vscode-bicep-ui/components";
import { useCallback, useEffect, useId, useLayoutEffect, useRef, useState } from "react";
import styled from "styled-components";

const POPUP_GAP = 4;
const VIEWPORT_MARGIN = 8;
const PAGE_SIZE = 8;

const $Trigger = styled.div`
  display: inline-flex;
  min-width: 0;
  max-width: 50%;
  flex-shrink: 0;
  align-items: center;
  height: 20px;
  padding: 0 8px;
  /* Quiet at rest; the owning row reveals the pill chrome via --api-version-pill-border. */
  border: 1px solid var(--api-version-pill-border, transparent);
  border-radius: 10px;
  color: ${({ theme }) => theme.text.secondary};
  font-size: 11px;
  font-variant-numeric: tabular-nums;
  white-space: nowrap;
  cursor: pointer;
  user-select: none;
  transition:
    background-color 150ms ease,
    border-color 150ms ease,
    color 150ms ease;

  &[data-customized="true"] {
    border-color: ${({ theme }) => theme.panel.border};
    color: ${({ theme }) => theme.text.primary};
  }

  &:hover {
    border-color: ${({ theme }) => theme.panel.border};
    color: ${({ theme }) => theme.text.primary};
    background: ${({ theme }) => theme.iconButton.hoverBackground};
  }

  &[aria-expanded="true"] {
    border-color: ${({ theme }) => theme.panel.border};
    color: ${({ theme }) => theme.text.primary};
    background: ${({ theme }) => theme.iconButton.activeBackground};
  }

  &:focus {
    outline: none;
  }

  &:focus-visible {
    outline: 1px solid ${({ theme }) => theme.focusBorder};
    outline-offset: 1px;
  }
`;

const $TriggerValue = styled.span`
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const $Popup = styled.div`
  inset: auto;
  box-sizing: border-box;
  max-height: 240px;
  margin: 0;
  padding: 4px;
  overflow-y: auto;
  border: 1px solid ${({ theme }) => theme.panel.border};
  border-radius: 6px;
  color: ${({ theme }) => theme.text.primary};
  background: ${({ theme }) => theme.panel.popoverBackground};
  box-shadow: ${({ theme }) => theme.panel.popoverShadow};
  backdrop-filter: ${({ theme }) => theme.panel.popoverBackdropFilter};
  font-size: 12px;
  scrollbar-width: none;

  &::-webkit-scrollbar {
    display: none;
  }
`;

const $Listbox = styled.ul`
  margin: 0;
  padding: 0;
  list-style: none;
`;

const $Option = styled.li`
  display: flex;
  align-items: center;
  gap: 6px;
  height: 24px;
  padding: 0 8px 0 4px;
  border: 1px solid transparent;
  border-radius: 4px;
  font-variant-numeric: tabular-nums;
  white-space: nowrap;
  cursor: pointer;

  &[data-active="true"] {
    border-color: var(--vscode-contrastActiveBorder, transparent);
    background: ${({ theme }) => theme.iconButton.hoverBackground};
  }
`;

const $Check = styled.span<{ $visible: boolean }>`
  display: inline-flex;
  width: 14px;
  visibility: ${({ $visible }) => ($visible ? "visible" : "hidden")};
`;

const $Suffix = styled.span`
  color: ${({ theme }) => theme.text.secondary};
`;

const $Status = styled.div`
  padding: 4px 8px;
  color: ${({ theme }) => theme.text.secondary};
  white-space: nowrap;
`;

const $Retry = styled.button`
  margin-left: 8px;
  padding: 0;
  border: 0;
  color: var(--vscode-textLink-foreground);
  background: transparent;
  font: inherit;
  cursor: pointer;

  &:hover {
    text-decoration: underline;
  }
`;

/** Dims qualifiers such as `-preview` so the release date stays scannable. */
function VersionLabel({ version }: { version: string }) {
  const match = /^(\d{4}-\d{2}-\d{2})(.+)$/.exec(version);
  if (!match) {
    return version;
  }

  return (
    <>
      {match[1]}
      <$Suffix>{match[2]}</$Suffix>
    </>
  );
}

function placePopup(trigger: HTMLElement, popup: HTMLElement) {
  const anchor = trigger.getBoundingClientRect();
  popup.style.minWidth = `${anchor.width}px`;
  const { offsetWidth: width, offsetHeight: height } = popup;
  const below = anchor.bottom + POPUP_GAP;
  const top =
    below + height <= window.innerHeight - VIEWPORT_MARGIN
      ? below
      : Math.max(VIEWPORT_MARGIN, anchor.top - POPUP_GAP - height);
  const left = Math.max(VIEWPORT_MARGIN, Math.min(anchor.right - width, window.innerWidth - VIEWPORT_MARGIN - width));
  popup.style.top = `${top}px`;
  popup.style.left = `${left}px`;
}

export function ApiVersionPicker({
  fullyQualifiedType,
  apiVersion,
  defaultVersion,
  state,
  load,
  select,
}: {
  fullyQualifiedType: string;
  apiVersion: string;
  /** The host's default for this type; a different selection stays emphasized at rest. */
  defaultVersion: string;
  state: ResourceVersionsState | undefined;
  load: () => void;
  select: (version: string) => void;
}) {
  const [open, setOpen] = useState(false);
  const [activeVersion, setActiveVersion] = useState<string>();
  const triggerRef = useRef<HTMLDivElement>(null);
  const popupRef = useRef<HTMLDivElement>(null);
  const id = useId();
  const listboxId = `${id}-versions`;
  const versions = state?.status === "loaded" ? state.apiVersions : undefined;
  const activeIndex = versions ? Math.max(0, versions.indexOf(activeVersion ?? apiVersion)) : -1;
  const optionId = (index: number) => `${id}-version-${index}`;

  const openPopup = useCallback(() => {
    setActiveVersion(undefined);
    setOpen(true);
    load();
  }, [load]);

  const closePopup = useCallback(() => setOpen(false), []);

  const commit = useCallback(
    (version: string) => {
      select(version);
      setOpen(false);
    },
    [select],
  );

  useLayoutEffect(() => {
    const trigger = triggerRef.current;
    const popup = popupRef.current;
    if (!open || !trigger || !popup) {
      return;
    }
    // The top layer escapes the palette's scroll clipping and transformed ancestors.
    if (typeof popup.showPopover === "function" && !popup.matches(":popover-open")) {
      popup.showPopover();
    }
    placePopup(trigger, popup);
  }, [open, state]);

  useLayoutEffect(() => {
    const popup = popupRef.current;
    const option =
      open && activeIndex >= 0 ? popup?.querySelectorAll<HTMLElement>('[role="option"]')[activeIndex] : null;
    if (!popup || !option) {
      return;
    }
    if (option.offsetTop < popup.scrollTop) {
      popup.scrollTop = option.offsetTop - 4;
    } else if (option.offsetTop + option.offsetHeight > popup.scrollTop + popup.clientHeight) {
      popup.scrollTop = option.offsetTop + option.offsetHeight - popup.clientHeight + 4;
    }
  }, [activeIndex, open, versions]);

  useEffect(() => {
    if (!open) {
      return;
    }
    const onScroll = (event: Event) => {
      if (!(event.target instanceof Node && popupRef.current?.contains(event.target))) {
        setOpen(false);
      }
    };
    window.addEventListener("scroll", onScroll, true);
    window.addEventListener("resize", closePopup);
    return () => {
      window.removeEventListener("scroll", onScroll, true);
      window.removeEventListener("resize", closePopup);
    };
  }, [closePopup, open]);

  const handleKeyDown = useCallback(
    (event: KeyboardEvent<HTMLDivElement>) => {
      if (!open) {
        if (["ArrowDown", "ArrowUp", "Enter", " ", "Home", "End", "F4"].includes(event.key)) {
          event.preventDefault();
          openPopup();
        }
        return;
      }

      switch (event.key) {
        case "Escape":
          // Keep the surrounding palette open; only the version list closes.
          event.preventDefault();
          setOpen(false);
          return;
        case "Tab":
          setOpen(false);
          return;
        case "Enter":
        case " ":
          event.preventDefault();
          if (versions?.[activeIndex]) {
            commit(versions[activeIndex]);
          }
          return;
      }

      if (!versions) {
        return;
      }
      if (event.altKey && event.key === "ArrowUp") {
        event.preventDefault();
        commit(versions[activeIndex] ?? apiVersion);
        return;
      }
      const last = versions.length - 1;
      const next =
        event.key === "ArrowDown"
          ? Math.min(activeIndex + 1, last)
          : event.key === "ArrowUp"
            ? Math.max(activeIndex - 1, 0)
            : event.key === "Home"
              ? 0
              : event.key === "End"
                ? last
                : event.key === "PageDown"
                  ? Math.min(activeIndex + PAGE_SIZE, last)
                  : event.key === "PageUp"
                    ? Math.max(activeIndex - PAGE_SIZE, 0)
                    : undefined;
      if (next !== undefined) {
        event.preventDefault();
        setActiveVersion(versions[next]);
      }
    },
    [activeIndex, apiVersion, commit, open, openPopup, versions],
  );

  const handleClick = useCallback(() => (open ? closePopup() : openPopup()), [closePopup, open, openPopup]);

  // Keep focus on the combobox while interacting with the popup.
  const keepFocus = useCallback((event: MouseEvent) => event.preventDefault(), []);

  return (
    <>
      <$Trigger
        ref={triggerRef}
        tabIndex={0}
        role="combobox"
        title={`API version for ${fullyQualifiedType}`}
        aria-label={`API version for ${fullyQualifiedType}`}
        aria-haspopup="listbox"
        aria-expanded={open}
        aria-controls={listboxId}
        aria-activedescendant={open && activeIndex >= 0 ? optionId(activeIndex) : undefined}
        aria-busy={state?.status === "loading"}
        data-testid="api-version-picker"
        data-customized={apiVersion !== defaultVersion}
        onFocus={load}
        onBlur={closePopup}
        onClick={handleClick}
        onKeyDown={handleKeyDown}
      >
        <$TriggerValue>
          <VersionLabel version={apiVersion} />
        </$TriggerValue>
      </$Trigger>
      {open && (
        <$Popup ref={popupRef} popover="manual" data-testid="api-version-popup" onMouseDown={keepFocus}>
          {/* Always rendered so the combobox's aria-controls resolves while versions load or fail. */}
          <$Listbox
            id={listboxId}
            role="listbox"
            aria-label={`API versions for ${fullyQualifiedType}`}
            aria-busy={state?.status === "loading"}
          >
            {versions?.map((version, index) => (
              <$Option
                key={version}
                id={optionId(index)}
                role="option"
                aria-selected={version === apiVersion}
                data-active={index === activeIndex}
                onMouseEnter={() => setActiveVersion(version)}
                onClick={() => commit(version)}
              >
                <$Check $visible={version === apiVersion}>
                  <Codicon name="check" size={14} />
                </$Check>
                <span>
                  <VersionLabel version={version} />
                </span>
              </$Option>
            ))}
          </$Listbox>
          {versions ? null : state?.status === "error" ? (
            <$Status role="status">
              {state.message}
              <$Retry
                type="button"
                title="Retry loading API versions"
                aria-label={`Retry API versions for ${fullyQualifiedType}`}
                onClick={load}
              >
                Retry
              </$Retry>
            </$Status>
          ) : (
            <$Status role="status">Loading API versions…</$Status>
          )}
        </$Popup>
      )}
    </>
  );
}
