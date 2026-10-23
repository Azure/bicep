// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CanvasNodeData } from "@/core";

import { AzureIcon, Codicon } from "@vscode-bicep-ui/components";
import { animate } from "motion";
import { AnimatePresence, motion } from "motion/react";
import { useCallback, useLayoutEffect, useRef } from "react";
import { styled, useTheme } from "styled-components";
import { useGraphActions, useIsMotionReduced } from "@/core";
import { graphInteractiveProps, NODE_MORPH_TRANSITION } from "@/lib/graph";
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

const LABEL_SHOWN = { height: TYPE_LABEL_LINE_HEIGHT, marginTop: 2, opacity: 1 } as const;
const LABEL_HIDDEN = { height: 0, marginTop: 0, opacity: 0 } as const;
const NO_TRANSITION = { duration: 0 } as const;

export interface ModuleNodeProps {
  id: string;
  /** `container` frames the module's children; `card` stands in for them when they are hidden. */
  variant: "container" | "card";
  data: CanvasNodeData;
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
  /* A card's height is fixed by its design, so it measures the same while its header animates. */
  min-height: ${({ $variant }) => ($variant === "card" ? `${CARD_HEIGHT}px` : "0")};
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

const $Icon = styled.div`
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
 * focus also reveals it.
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
`;

/** The row's height in each variant; the morph animates between them. */
function getRowHeight(variant: ModuleNodeProps["variant"], borderWidth: string) {
  return variant === "card" ? CARD_HEIGHT - 2 * parseFloat(borderWidth) : HEADER_HEIGHT;
}

export function ModuleNode({ id, variant, data }: ModuleNodeProps) {
  const { symbolicName, isCollection, hasError, isCollapsed, canCollapse } = data;
  const theme = useTheme();
  const frameRef = useRef<HTMLDivElement>(null);
  const iconRef = useRef<HTMLDivElement>(null);
  const isFocused = useIsNodeFocused(id);
  const reduceMotion = useIsMotionReduced();
  const { toggleModuleCollapsed } = useGraphActions();
  const isCard = variant === "card";
  const borderWidth = hasError ? theme.node.errorBorderWidth : theme.node.borderWidth;
  const rowHeight = getRowHeight(variant, borderWidth);
  const iconSize = isCard ? CARD_ICON_SIZE : HEADER_ICON_SIZE;
  const action = isCollapsed ? "Expand" : "Collapse";
  const transition = reduceMotion ? NO_TRANSITION : NODE_MORPH_TRANSITION;

  // The graph morphs the node's box when the module opens or closes; the icon and background change
  // with it. The header and type label animate through their own props.
  const previousVariantRef = useRef(variant);
  useLayoutEffect(() => {
    const previousVariant = previousVariantRef.current;
    previousVariantRef.current = variant;
    const frame = frameRef.current;
    const icon = iconRef.current;

    if (previousVariant === variant || reduceMotion || !frame || !icon) {
      return;
    }

    const background = (shape: ModuleNodeProps["variant"]) =>
      shape === "card" ? theme.node.background : theme.node.compoundBackground;
    const otherIconSize = isCard ? HEADER_ICON_SIZE : CARD_ICON_SIZE;
    const iconControls = animate(icon, { scale: [otherIconSize / iconSize, 1] }, NODE_MORPH_TRANSITION);
    const backgroundControls = animate(
      frame,
      { backgroundColor: [background(previousVariant), background(variant)] },
      NODE_MORPH_TRANSITION,
    );
    // Afterwards the theme sets the background again, so a theme change applies.
    void backgroundControls.then(() => {
      frame.style.backgroundColor = "";
    });

    return () => {
      iconControls.stop();
      backgroundControls.stop();
      frame.style.backgroundColor = "";
    };
  }, [iconSize, isCard, reduceMotion, theme, variant]);

  const handleToggle = useCallback(() => toggleModuleCollapsed(id), [id, toggleModuleCollapsed]);

  return (
    <$Frame
      ref={frameRef}
      $hasError={hasError}
      $isCollection={isCollection}
      $isFocused={isFocused}
      $variant={variant}
      data-focused={isFocused}
      data-module-collapsed={isCollapsed}
    >
      <$Row initial={false} animate={{ height: rowHeight }} transition={transition}>
        <$Icon ref={iconRef}>
          <AzureIcon resourceType="folder" size={iconSize} />
        </$Icon>
        <$Text>
          <$SymbolicName>{symbolicName}</$SymbolicName>
          {/* Only the card names its type: a container's frame already says it is a module. */}
          <AnimatePresence initial={false}>
            {isCard && (
              <$TypeLabel
                key="type"
                initial={LABEL_HIDDEN}
                animate={LABEL_SHOWN}
                exit={LABEL_HIDDEN}
                transition={transition}
              >
                Module
              </$TypeLabel>
            )}
          </AnimatePresence>
        </$Text>
        {canCollapse && (
          <$ToggleButton
            {...graphInteractiveProps}
            type="button"
            aria-label={`${action} module ${symbolicName}`}
            aria-expanded={!isCollapsed}
            title={`${action} module`}
            data-export-exclude=""
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
