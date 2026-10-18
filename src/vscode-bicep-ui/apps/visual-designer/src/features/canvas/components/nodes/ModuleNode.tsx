// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MouseEvent } from "react";
import type { ModuleRestore } from "@/core";
import type { NodeKind } from "@/lib/graph";

import { AzureIcon, Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue, useStore } from "jotai";
import { animate } from "motion";
import { motion } from "motion/react";
import { useCallback, useEffect, useId, useLayoutEffect, useRef, useState } from "react";
import { styled, useTheme } from "styled-components";
import {
  isGraphChangeInProgressAtom,
  MODULE_MORPH_TRANSITION,
  moduleRestoresAtom,
  motionPolicyAtom,
  shouldReduceMotion,
  useGraphActions,
} from "@/core";
import { beginNodeResize, nodesByIdAtom, useIsNodeFocused } from "@/lib/graph";
import { ICON_BUTTON_ICON_SIZE, IconButton, Spinner } from "@/ui";

/** Matches `ResourceNode`, so a collapsed module reads as one more node on the graph. */
const ATOMIC_HEIGHT = 76;
const ATOMIC_ICON_SIZE = 36;
/** The header must fit above a container's children, inside its top padding. */
const HEADER_HEIGHT = 48;
const HEADER_ICON_SIZE = 24;
/** A fixed line height lets the type label animate to an exact height. */
const TYPE_LABEL_LINE_HEIGHT = 16;

/** The size a module had before it changed shape, or `"atomic"` when it is expanding from a single node. */
type MorphFrom = { width: number; height: number } | "atomic";

const LABEL_SHOWN = { height: TYPE_LABEL_LINE_HEIGHT, marginTop: 2, opacity: 1 } as const;
const LABEL_HIDDEN = { height: 0, marginTop: 0, opacity: 0 } as const;

export interface ModuleNodeProps {
  id: string;
  /** `compound` frames the module's children; `atomic` stands in for them when they are hidden. */
  kind: NodeKind;
  data: {
    symbolicName: string;
    isCollection?: boolean;
    hasError?: boolean;
    isCollapsed?: boolean;
    canCollapse?: boolean;
    morphFrom?: MorphFrom;
  };
}

type FrameProps = {
  $hasError?: boolean;
  $isCollection?: boolean;
  $isFocused?: boolean;
  $kind: NodeKind;
};

const $Frame = styled.div<FrameProps>`
  position: relative;
  flex: 1 1 auto;
  display: flex;
  flex-direction: column;
  margin: 4px;
  box-sizing: border-box;
  min-width: ${({ $kind }) => ($kind === "atomic" ? "220px" : "0")};
  border: ${({ $hasError, theme }) => ($hasError ? theme.node.errorBorderWidth : theme.node.borderWidth)} solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 8px;
  background-color: ${({ $kind, theme }) =>
    $kind === "atomic" ? theme.node.background : theme.node.compoundBackground};
  box-shadow: ${({ $isFocused, $hasError, theme }) =>
    $isFocused ? ($hasError ? theme.node.selectedErrorShadow : theme.node.selectedShadow) : theme.node.shadow};
  transition:
    border-color 180ms ease,
    box-shadow 180ms ease;

  &:hover {
    border-color: ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.hoverBorder};
    box-shadow: ${({ $isFocused, $hasError, theme }) => {
      if ($isFocused) return $hasError ? theme.node.selectedErrorShadow : theme.node.selectedShadow;
      return $hasError ? theme.node.hoverErrorShadow : theme.node.hoverShadow;
    }};
  }

  ${({ $isCollection, $hasError, $isFocused, $kind, theme }) => {
    const offset = theme.node.collectionOffset;
    return $isCollection
      ? `
    margin-right: ${4 + offset}px;
    margin-bottom: ${4 + offset}px;
    &::before {
      content: '';
      position: absolute;
      top: ${offset}px;
      left: ${offset}px;
      right: -${offset}px;
      bottom: -${offset}px;
      border: ${$hasError ? theme.node.errorBorderWidth : theme.node.borderWidth} solid ${$hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
      border-radius: 10px;
      background-color: ${$kind === "atomic" ? theme.node.background : theme.node.compoundBackground};
      z-index: -1;
      box-shadow: ${$isFocused ? ($hasError ? theme.node.selectedErrorShadow : theme.node.selectedShadow) : theme.node.shadow};
      transition: border-color 180ms ease, box-shadow 180ms ease;
    }
    &:hover::before {
      border-color: ${$hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.hoverBorder};
    }
  `
      : "";
  }}
`;

const $Row = styled(motion.div)`
  display: flex;
  flex: 0 0 auto;
  align-items: center;
  box-sizing: border-box;
  padding: 0 12px 0 20px;
`;

const $Icon = styled(motion.div)`
  display: flex;
  flex: 0 0 auto;
  transform-origin: left center;
`;

const $Text = styled.div`
  display: flex;
  flex: 1 1 auto;
  flex-direction: column;
  justify-content: center;
  min-width: 0;
  margin: 0 8px 0 12px;
  overflow: hidden;
`;

const $SymbolicName = styled.div`
  font-size: 15px;
  font-weight: 600;
  color: ${({ theme }) => theme.text.primary};
  letter-spacing: -0.01em;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const $TypeLabel = styled(motion.div)`
  font-size: 12px;
  font-weight: 500;
  line-height: ${TYPE_LABEL_LINE_HEIGHT}px;
  color: ${({ theme }) => theme.text.secondary};
  letter-spacing: 0.02em;
  text-transform: uppercase;
  white-space: nowrap;
  overflow: hidden;
`;

/**
 * Hidden until the module is hovered or selected, so a graph full of modules stays uncluttered. Keyboard
 * focus also reveals it, and so does a restore in progress or failed. A disabled toggle stays hidden unless
 * something would otherwise reveal it.
 */
const $ToggleButton = styled(IconButton)<{ $isRevealed: boolean; $isFailure: boolean }>`
  flex: 0 0 auto;
  margin-left: auto;
  opacity: ${({ $isRevealed }) => ($isRevealed ? 1 : 0)};
  ${({ $isFailure, theme }) => ($isFailure ? `color: ${theme.error};` : "")}
  transition:
    opacity 120ms ease,
    background-color 150ms ease,
    transform 150ms ease;

  ${$Frame}:hover &,
  &:focus-visible {
    opacity: 1;
  }

  &:disabled {
    opacity: ${({ $isRevealed }) => ($isRevealed ? 0.45 : 0)};
  }

  ${$Frame}:hover &:disabled {
    opacity: 0.45;
  }
`;

const $RestoreStatus = styled.span`
  position: absolute;
  width: 1px;
  height: 1px;
  overflow: hidden;
  clip-path: inset(50%);
  white-space: nowrap;
`;

/** The row's height for each kind; the morph animates between them. */
function getRowHeight(kind: NodeKind, borderWidth: string) {
  return kind === "atomic" ? ATOMIC_HEIGHT - 2 * parseFloat(borderWidth) : HEADER_HEIGHT;
}

/** What the expand/collapse toggle says and shows, given any restore of the module in progress or failed. */
function getToggleContent(symbolicName: string, isCollapsed: boolean, restore: ModuleRestore | undefined) {
  if (restore?.status === "restoring") {
    return { label: `Restoring module ${symbolicName}`, title: "Restoring module…", icon: "loading" };
  }

  if (restore?.status === "failed") {
    return {
      label: restore.sourceRestored
        ? `Retry expanding module ${symbolicName}`
        : `Retry restoring module ${symbolicName}`,
      title: `${restore.sourceRestored ? "Couldn't load the restored module" : "Couldn't restore the module"}. Click to try again.\n\n${restore.message}`,
      icon: "error",
    };
  }

  const action = isCollapsed ? "Expand" : "Collapse";
  return {
    label: `${action} module ${symbolicName}`,
    title: `${action} module`,
    icon: isCollapsed ? "unfold" : "fold",
  };
}

export function ModuleNode({ id, kind, data }: ModuleNodeProps) {
  const { symbolicName, isCollection, hasError, isCollapsed = false, canCollapse = false } = data;
  const theme = useTheme();
  const frameRef = useRef<HTMLDivElement>(null);
  // Only the mount that replaced the module's other shape morphs; later data updates must not replay it.
  const [morphFrom] = useState(data.morphFrom);
  const isFocused = useIsNodeFocused(id);
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const motionPolicy = useAtomValue(motionPolicyAtom);
  // Expanding a remote module whose source is not cached restores it first.
  const restore = useAtomValue(moduleRestoresAtom).get(id);
  const isRestoring = restore?.status === "restoring";
  const isRestoreFailed = restore?.status === "failed";
  const restoreStatusId = useId();
  const toggle = getToggleContent(symbolicName, isCollapsed, restore);
  const { toggleModuleCollapsed } = useGraphActions();
  const isAtomic = kind === "atomic";
  const borderWidth = hasError ? theme.node.errorBorderWidth : theme.node.borderWidth;
  const rowHeight = getRowHeight(kind, borderWidth);
  const otherRowHeight = getRowHeight(isAtomic ? "compound" : "atomic", borderWidth);
  const iconSize = isAtomic ? ATOMIC_ICON_SIZE : HEADER_ICON_SIZE;
  const otherIconSize = isAtomic ? HEADER_ICON_SIZE : ATOMIC_ICON_SIZE;

  // A collapsing module mounts at the size of the container it replaces, so its node box -- and the
  // edges attached to it -- start there, then shrink to its collapsed size. This runs before the node measures
  // itself, because child layout effects run before their parents'.
  const store = useStore();
  useLayoutEffect(() => {
    const frame = frameRef.current;
    if (!frame || !morphFrom) {
      return;
    }

    const fromBackground = isAtomic ? theme.node.compoundBackground : theme.node.background;
    const toBackground = isAtomic ? theme.node.background : theme.node.compoundBackground;

    if (morphFrom === "atomic") {
      // The container's box grows from the collapsed module on its own; only its color is left to blend.
      const controls = animate(frame, { backgroundColor: [fromBackground, toBackground] }, MODULE_MORPH_TRANSITION);
      return () => {
        controls.stop();
        frame.style.backgroundColor = "";
      };
    }

    const node = frame.closest<HTMLElement>("[data-node-id]");
    // The row starts at the header's height, so the collapsed height comes from its design, not the DOM.
    const naturalWidth = frame.offsetWidth;
    const naturalHeight = ATOMIC_HEIGHT;
    const marginsWidth = (node?.offsetWidth ?? naturalWidth) - naturalWidth;
    const marginsHeight = (node?.offsetHeight ?? frame.offsetHeight) - frame.offsetHeight;
    const fromWidth = Math.max(naturalWidth, morphFrom.width - marginsWidth);
    const fromHeight = Math.max(naturalHeight, morphFrom.height - marginsHeight);

    // The node has not measured itself yet, so its box is still the point it was created at: the
    // container's center. The module shrinks toward it. Writing the box in the same tick as the size
    // keeps them in step; the node's own resize measurement arrives a frame late.
    const graphNode = store.get(nodesByIdAtom)[id];
    const boxAtom = graphNode?.kind === "atomic" ? graphNode.boxAtom : undefined;
    const startBox = boxAtom ? store.get(boxAtom) : undefined;
    const center = startBox && {
      x: (startBox.min.x + startBox.max.x) / 2,
      y: (startBox.min.y + startBox.max.y) / 2,
    };
    const setSize = (width: number, height: number) => {
      frame.style.width = `${width}px`;
      frame.style.height = `${height}px`;
      if (boxAtom && center) {
        const halfWidth = (width + marginsWidth) / 2;
        const halfHeight = (height + marginsHeight) / 2;
        store.set(boxAtom, {
          min: { x: center.x - halfWidth, y: center.y - halfHeight },
          max: { x: center.x + halfWidth, y: center.y + halfHeight },
        });
      }
    };
    const clearMorph = () => {
      frame.style.width = "";
      frame.style.height = "";
      frame.style.backgroundColor = "";
      // After the styles are cleared, so a layout waiting for the resize to end measures the module's own size.
      endResize();
    };

    // A layout must not measure the sizes the module passes through on its way.
    const endResize = beginNodeResize(store);
    setSize(fromWidth, fromHeight);
    const sizeControls = animate(0, 1, {
      ...MODULE_MORPH_TRANSITION,
      onUpdate: (progress) =>
        setSize(
          fromWidth + (naturalWidth - fromWidth) * progress,
          fromHeight + (naturalHeight - fromHeight) * progress,
        ),
    });
    const colorControls = animate(frame, { backgroundColor: [fromBackground, toBackground] }, MODULE_MORPH_TRANSITION);
    void sizeControls.then(clearMorph);

    return () => {
      sizeControls.stop();
      colorControls.stop();
      clearMorph();
    };
    // Theme colors are read once: the morph plays only on mount.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [morphFrom]);

  const toggleButtonRef = useRef<HTMLButtonElement>(null);

  // The canvas reads presses (focus, node drag, pan) and double-clicks (reveal source) with native
  // listeners on the node's ancestors, which React's synthetic stopPropagation runs too late to stop.
  useEffect(() => {
    const button = toggleButtonRef.current;
    if (!button) {
      return;
    }

    const stop = (event: Event) => event.stopPropagation();
    const events = ["pointerdown", "mousedown", "touchstart", "dblclick"] as const;
    for (const type of events) {
      button.addEventListener(type, stop);
    }

    return () => {
      for (const type of events) {
        button.removeEventListener(type, stop);
      }
    };
  }, [canCollapse]);

  const handleToggle = useCallback(
    (event: MouseEvent<HTMLButtonElement>) => {
      event.stopPropagation();
      toggleModuleCollapsed(id);
    },
    [id, toggleModuleCollapsed],
  );

  return (
    <$Frame
      ref={frameRef}
      $hasError={hasError}
      $isCollection={isCollection}
      $isFocused={isFocused}
      $kind={kind}
      data-module-collapsed={isCollapsed}
    >
      <$Row
        initial={morphFrom ? { height: otherRowHeight } : false}
        animate={{ height: rowHeight }}
        transition={MODULE_MORPH_TRANSITION}
      >
        <$Icon
          initial={morphFrom ? { scale: otherIconSize / iconSize } : false}
          animate={{ scale: 1 }}
          transition={MODULE_MORPH_TRANSITION}
        >
          <AzureIcon resourceType="folder" size={iconSize} />
        </$Icon>
        <$Text>
          <$SymbolicName>{symbolicName}</$SymbolicName>
          {/* Only a module shown as a single node names its type: a container's frame already says it is a
              module. While a module changes shape, the label grows or shrinks with it so the name glides. */}
          {(isAtomic || morphFrom === "atomic") && (
            <$TypeLabel
              initial={morphFrom ? (isAtomic ? LABEL_HIDDEN : LABEL_SHOWN) : false}
              animate={isAtomic ? LABEL_SHOWN : LABEL_HIDDEN}
              transition={MODULE_MORPH_TRANSITION}
            >
              Module
            </$TypeLabel>
          )}
        </$Text>
        {canCollapse && (
          <$ToggleButton
            ref={toggleButtonRef}
            type="button"
            aria-label={toggle.label}
            aria-expanded={!isCollapsed}
            aria-busy={isRestoring}
            aria-describedby={isRestoreFailed ? restoreStatusId : undefined}
            title={toggle.title}
            data-export-exclude=""
            disabled={isGraphChangeInProgress || isRestoring}
            $isRevealed={isFocused || isRestoring || isRestoreFailed}
            $isFailure={isRestoreFailed}
            onClick={handleToggle}
          >
            {isRestoring ? (
              <Spinner size={ICON_BUTTON_ICON_SIZE} isAnimated={!shouldReduceMotion(motionPolicy)} />
            ) : (
              <Codicon name={toggle.icon} size={ICON_BUTTON_ICON_SIZE} />
            )}
          </$ToggleButton>
        )}
        <$RestoreStatus id={restoreStatusId} role="status">
          {restore?.status === "failed" ? restore.message : isRestoring ? toggle.label : ""}
        </$RestoreStatus>
      </$Row>
    </$Frame>
  );
}
