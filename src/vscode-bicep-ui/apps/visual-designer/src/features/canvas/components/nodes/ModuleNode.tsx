// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MouseEvent } from "react";

import { AzureIcon, Codicon } from "@vscode-bicep-ui/components";
import { useAtomValue, useStore } from "jotai";
import { animate } from "motion";
import { motion } from "motion/react";
import { useCallback, useLayoutEffect, useRef, useState } from "react";
import { styled, useTheme } from "styled-components";
import { isGraphChangeInProgressAtom, MODULE_MORPH_TRANSITION, useGraphActions } from "@/core";
import { graphInteractiveProps, nodesByIdAtom } from "@/lib/graph";
import { ICON_BUTTON_ICON_SIZE, IconButton } from "@/ui";
import { useIsNodeFocused } from "../../hooks/use-is-node-focused";

/** Matches `ResourceNode`, so a collapsed module reads as one more card on the graph. */
const CARD_HEIGHT = 76;
const CARD_ICON_SIZE = 36;
/** The header must fit above a container's children, inside its top padding. */
const HEADER_HEIGHT = 48;
const HEADER_ICON_SIZE = 24;
/** A fixed line height lets the type label animate to an exact height. */
const TYPE_LABEL_LINE_HEIGHT = 16;

/** The size a module had before it changed shape, or `"card"` when it is expanding from its card. */
type MorphFrom = { width: number; height: number } | "card";

const LABEL_SHOWN = { height: TYPE_LABEL_LINE_HEIGHT, marginTop: 2, opacity: 1 } as const;
const LABEL_HIDDEN = { height: 0, marginTop: 0, opacity: 0 } as const;

export interface ModuleNodeProps {
  id: string;
  /** `container` frames the module's children; `card` stands in for them when they are hidden. */
  variant: "container" | "card";
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
  $variant: ModuleNodeProps["variant"];
};

const $Frame = styled.div<FrameProps>`
  position: relative;
  flex: 1 1 auto;
  display: flex;
  flex-direction: column;
  margin: 4px;
  box-sizing: border-box;
  min-width: ${({ $variant }) => ($variant === "card" ? "220px" : "0")};
  border: ${({ $hasError, theme }) => ($hasError ? theme.node.errorBorderWidth : theme.node.borderWidth)} solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 8px;
  background-color: ${({ $variant, theme }) =>
    $variant === "card" ? theme.node.background : theme.node.compoundBackground};
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

  ${({ $isCollection, $hasError, $isFocused, $variant, theme }) => {
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
      background-color: ${$variant === "card" ? theme.node.background : theme.node.compoundBackground};
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
 * focus also reveals it. A disabled toggle stays hidden unless something would otherwise reveal it.
 */
const $ToggleButton = styled(IconButton)<{ $isRevealed: boolean }>`
  flex: 0 0 auto;
  margin-left: auto;
  opacity: ${({ $isRevealed }) => ($isRevealed ? 1 : 0)};
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

/** The row's height in each variant; the morph animates between them. */
function getRowHeight(variant: ModuleNodeProps["variant"], borderWidth: string) {
  return variant === "card" ? CARD_HEIGHT - 2 * parseFloat(borderWidth) : HEADER_HEIGHT;
}

export function ModuleNode({ id, variant, data }: ModuleNodeProps) {
  const { symbolicName, isCollection, hasError, isCollapsed = false, canCollapse = false } = data;
  const theme = useTheme();
  const frameRef = useRef<HTMLDivElement>(null);
  // Only the mount that replaced the module's other shape morphs; later data updates must not replay it.
  const [morphFrom] = useState(data.morphFrom);
  const isFocused = useIsNodeFocused(id);
  const isGraphChangeInProgress = useAtomValue(isGraphChangeInProgressAtom);
  const { toggleModuleCollapsed } = useGraphActions();
  const isCard = variant === "card";
  const borderWidth = hasError ? theme.node.errorBorderWidth : theme.node.borderWidth;
  const rowHeight = getRowHeight(variant, borderWidth);
  const otherRowHeight = getRowHeight(isCard ? "container" : "card", borderWidth);
  const iconSize = isCard ? CARD_ICON_SIZE : HEADER_ICON_SIZE;
  const otherIconSize = isCard ? HEADER_ICON_SIZE : CARD_ICON_SIZE;
  const action = isCollapsed ? "Expand" : "Collapse";

  // A collapsing module mounts at the size of the container it replaces, so its node box -- and the
  // edges attached to it -- start there, then shrink to the card. This runs before the node measures
  // itself, because child layout effects run before their parents'.
  const store = useStore();
  useLayoutEffect(() => {
    const frame = frameRef.current;
    if (!frame || !morphFrom) {
      return;
    }

    const fromBackground = isCard ? theme.node.compoundBackground : theme.node.background;
    const toBackground = isCard ? theme.node.background : theme.node.compoundBackground;

    if (morphFrom === "card") {
      // The container's box grows from the card on its own; only its color is left to blend.
      const controls = animate(frame, { backgroundColor: [fromBackground, toBackground] }, MODULE_MORPH_TRANSITION);
      return () => {
        controls.stop();
        frame.style.backgroundColor = "";
      };
    }

    const node = frame.closest<HTMLElement>("[data-node-id]");
    // The row starts at the header's height, so the card's height comes from its design, not the DOM.
    const naturalWidth = frame.offsetWidth;
    const naturalHeight = CARD_HEIGHT;
    const marginsWidth = (node?.offsetWidth ?? naturalWidth) - naturalWidth;
    const marginsHeight = (node?.offsetHeight ?? frame.offsetHeight) - frame.offsetHeight;
    const fromWidth = Math.max(naturalWidth, morphFrom.width - marginsWidth);
    const fromHeight = Math.max(naturalHeight, morphFrom.height - marginsHeight);

    // The node has not measured itself yet, so its box is still the point it was created at: the
    // container's center. The card shrinks toward it. Writing the box in the same tick as the size
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
    };

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
      data-focused={isFocused}
      $variant={variant}
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
          {/* Only the card names its type: a container's frame already says it is a module. While a
              module changes shape, the label grows into or out of the card so the name glides. */}
          {(isCard || morphFrom === "card") && (
            <$TypeLabel
              initial={morphFrom ? (isCard ? LABEL_HIDDEN : LABEL_SHOWN) : false}
              animate={isCard ? LABEL_SHOWN : LABEL_HIDDEN}
              transition={MODULE_MORPH_TRANSITION}
            >
              Module
            </$TypeLabel>
          )}
        </$Text>
        {canCollapse && (
          <$ToggleButton
            {...graphInteractiveProps}
            type="button"
            aria-label={`${action} module ${symbolicName}`}
            aria-expanded={!isCollapsed}
            title={`${action} module`}
            data-export-exclude=""
            disabled={isGraphChangeInProgress}
            $isRevealed={isFocused}
            onClick={handleToggle}
          >
            <Codicon name={isCollapsed ? "unfold" : "fold"} size={ICON_BUTTON_ICON_SIZE} />
          </$ToggleButton>
        )}
      </$Row>
    </$Frame>
  );
}
