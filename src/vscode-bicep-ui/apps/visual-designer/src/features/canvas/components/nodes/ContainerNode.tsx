// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { DefaultTheme } from "styled-components";
import type { CanvasNodeData } from "@/core";
import type { NodeKind } from "@/lib/graph";

import { AzureIcon, Codicon } from "@vscode-bicep-ui/components";
import { animate } from "motion";
import { motion } from "motion/react";
import { useCallback, useId, useLayoutEffect, useRef } from "react";
import { keyframes, styled, useTheme } from "styled-components";
import { useGraphActions, useIsMotionReduced } from "@/core";
import { graphInteractiveProps, NODE_MORPH_TRANSITION } from "@/lib/graph";
import { ICON_BUTTON_ICON_SIZE, IconButton } from "@/ui";
import { camelCaseToWords } from "@/utils";
import { useIsModuleRestoring } from "../../hooks/use-is-module-restoring";
import { useIsNodeFocused } from "../../hooks/use-is-node-focused";

/** Matches `ResourceNode`, so a collapsed container reads as one more node on the graph. */
const ATOMIC_HEIGHT = 76;
const ATOMIC_ICON_SIZE = 36;
/** The header must fit above a compound node's children, inside its top padding. */
const HEADER_HEIGHT = 48;
const HEADER_ICON_SIZE = 24;
/** A fixed line height lets the type label animate to an exact height. */
const TYPE_LABEL_LINE_HEIGHT = 16;

const LABEL_SHOWN = { height: TYPE_LABEL_LINE_HEIGHT, marginTop: 2, opacity: 1 } as const;
const NO_TRANSITION = { duration: 0 } as const;

export interface ContainerNodeProps {
  id: string;
  /** `compound` frames the container's children; `atomic` stands in for them when they are hidden. */
  kind: NodeKind;
  data: CanvasNodeData;
}

type FrameProps = {
  $hasError?: boolean;
  $isCollection?: boolean;
  $isFocused?: boolean;
  $kind: NodeKind;
};

/** Every container frame, module or resource, sits a step below the nodes inside it; collapsed, it is a node. */
function getFrameBackground(kind: NodeKind, theme: DefaultTheme) {
  return kind === "atomic" ? theme.node.background : theme.node.compoundBackground;
}

/**
 * An expanded frame is a region rather than an object, so only nodes cast shadows and stand above the frames around
 * them. High-contrast themes cast none and mark hover with a ring, which frames keep.
 */
function isFlat(kind: NodeKind, theme: DefaultTheme) {
  return kind !== "atomic" && theme.node.shadow !== "none";
}

function getRestingShadow({ $kind, $isFocused, $hasError, theme }: FrameProps & { theme: DefaultTheme }) {
  if ($isFocused) {
    return $hasError ? theme.node.selectedErrorShadow : theme.node.selectedShadow;
  }

  return $kind === "atomic" ? theme.node.shadow : "none";
}

function getHoverShadow({ $kind, $isFocused, $hasError, theme }: FrameProps & { theme: DefaultTheme }) {
  if ($isFocused) {
    return $hasError ? theme.node.selectedErrorShadow : theme.node.selectedShadow;
  }

  if (isFlat($kind, theme)) {
    return "none";
  }

  return $hasError ? theme.node.hoverErrorShadow : theme.node.hoverShadow;
}

const $Frame = styled.div<FrameProps>`
  position: relative;
  flex: 1 1 auto;
  display: flex;
  flex-direction: column;
  margin: 4px;
  box-sizing: border-box;
  min-width: ${({ $kind }) => ($kind === "atomic" ? "220px" : "0")};
  /* An atomic node's height is fixed by its design, so it measures the same while its header animates. */
  min-height: ${({ $kind }) => ($kind === "atomic" ? `${ATOMIC_HEIGHT}px` : "0")};
  border: ${({ $hasError, theme }) => ($hasError ? theme.node.errorBorderWidth : theme.node.borderWidth)} solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 8px;
  background-color: ${({ $kind, theme }) => getFrameBackground($kind, theme)};
  box-shadow: ${getRestingShadow};
  transition:
    border-color 180ms ease,
    box-shadow 180ms ease;

  &:hover {
    border-color: ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.hoverBorder};
    box-shadow: ${getHoverShadow};
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
      background-color: ${getFrameBackground($kind, theme)};
      z-index: -1;
      box-shadow: ${getRestingShadow({ $kind, $isFocused, $hasError, theme })};
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
  position: relative;
  display: flex;
  flex: 0 0 auto;
  transform-origin: left center;
`;

/**
 * The module folder with a cloud pressed into its front panel, marking a module from a registry or template spec. It is
 * the Azure folder icon with one more shape in the folder's own gold, so it reads as one icon in every theme.
 */
function RemoteFolderIcon({ size }: { size: number }) {
  // Each icon needs its own gradient id: an export clones the canvas, and ids must stay unique within a document.
  const gradientId = useId();

  return (
    <svg width={size} height={size} viewBox="0 0 18 18" role="img" data-testid="remote-module-icon">
      <title>Remote module</title>
      <defs>
        <linearGradient id={gradientId} x1="9.252" y1="0.485" x2="8.842" y2="16.966" gradientUnits="userSpaceOnUse">
          <stop offset="0" stopColor="#ffd400" />
          <stop offset="0.415" stopColor="#ffd000" />
          <stop offset="0.845" stopColor="#ffc301" />
          <stop offset="1" stopColor="#ffbd02" />
        </linearGradient>
      </defs>
      <path
        d="M17.579,3.283H9.727a.419.419,0,0,1-.233-.07L7.251,1.721a.42.42,0,0,0-.233-.071H.421A.42.42,0,0,0,0,2.07V15.93a.42.42,0,0,0,.421.42H17.579A.42.42,0,0,0,18,15.93V3.7A.42.42,0,0,0,17.579,3.283Z"
        fill="#dfa500"
      />
      <rect x="1.636" y="2.455" width="4.091" height="0.818" rx="0.172" fill="#fff" />
      <path
        d="M17.579,3.263H8.956a.421.421,0,0,0-.3.123L7.272,4.773a.42.42,0,0,1-.3.123H.421A.42.42,0,0,0,0,5.316V15.91a.42.42,0,0,0,.421.419H17.579A.42.42,0,0,0,18,15.91V3.683A.42.42,0,0,0,17.579,3.263Z"
        fill={`url(#${gradientId})`}
      />
      <path
        d="M6.1 13.4 H12.1 A1.9 1.9 0 0 0 12.3 9.6 A2.7 2.7 0 0 0 7.2 8.8 A2.3 2.3 0 0 0 6.1 13.4 Z"
        fill="#c98f00"
      />
    </svg>
  );
}

const $Text = styled.div<{ $isInline?: boolean }>`
  display: flex;
  flex: 1 1 auto;
  flex-direction: ${({ $isInline }) => ($isInline ? "row" : "column")};
  align-items: ${({ $isInline }) => ($isInline ? "baseline" : "stretch")};
  gap: ${({ $isInline }) => ($isInline ? "8px" : "0")};
  justify-content: ${({ $isInline }) => ($isInline ? "flex-start" : "center")};
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
 * focus also reveals it. It is disabled with `aria-disabled` rather than `disabled`, so its tooltip still shows.
 */
const $ToggleButton = styled(IconButton)<{ $isRevealed: boolean; $isError: boolean }>`
  flex: 0 0 auto;
  margin-left: auto;
  opacity: ${({ $isRevealed }) => ($isRevealed ? 1 : 0)};
  color: ${({ $isError, theme }) => ($isError ? theme.error : theme.iconButton.color)};
  transition:
    opacity 120ms ease,
    background-color 150ms ease,
    transform 150ms ease;

  ${$Frame}:hover &,
  &:focus-visible {
    opacity: 1;
  }

  &[aria-disabled="true"] {
    cursor: default;
    background-color: transparent;
    transform: none;
  }

  &[aria-busy="true"] {
    cursor: progress;
  }

  &[aria-disabled="true"]:not([aria-busy="true"]) > * {
    opacity: 0.45;
  }
`;

const spin = keyframes`
  to {
    transform: rotate(360deg);
  }
`;

/** Replaces the toggle's icon while the module is restored, so the user sees that expanding it is under way. */
const $ProgressRing = styled.div`
  display: flex;
  animation: ${spin} 1s linear infinite;
`;

type Toggle = {
  action: "Expand" | "Collapse";
  title: string;
  isDisabled?: boolean;
  isError?: boolean;
};

/**
 * The module's one toggle, or null when there is nothing to toggle. It opens or closes the module, and a remote module
 * whose children are not loaded is restored first, showing progress until it opens; when its source cannot be shown,
 * the toggle is disabled and says so.
 */
function getToggle(data: CanvasNodeData, isRestoring: boolean): Toggle | null {
  const noun = data.nodeKind === "module" ? "module" : "resource";

  // A restore lasts until the module has opened, so it wins even once the module has children.
  if (isRestoring) {
    return { action: "Expand", title: "Restoring module...", isDisabled: true };
  }

  if (data.canCollapse) {
    return data.isCollapsed
      ? { action: "Expand", title: `Expand ${noun}` }
      : { action: "Collapse", title: `Collapse ${noun}` };
  }

  switch (data.sourceStatus) {
    case "unrestored":
      return { action: "Expand", title: "Expand module" };
    case "error":
      return {
        action: "Expand",
        title: `${data.sourceError ?? "The module's source could not be loaded."} Expand the module to try again.`,
        isError: true,
      };
    case "unavailable":
      return { action: "Expand", title: "Bicep source is unavailable for this module.", isDisabled: true };
    default:
      return null;
  }
}

/** The row's height for each kind of node; the morph animates between them. */
function getRowHeight(kind: NodeKind, borderWidth: string) {
  return kind === "atomic" ? ATOMIC_HEIGHT - 2 * parseFloat(borderWidth) : HEADER_HEIGHT;
}

/**
 * A node that can be drawn around its children: a module, or a resource with child resources or declarations scoped to it.
 * Both kinds share one frame and differ only in the header's icon and type label.
 */
export function ContainerNode({ id, kind, data }: ContainerNodeProps) {
  const {
    symbolicName,
    isCollection,
    hasError,
    isCollapsed,
    canCollapse,
    sourceStatus,
    nodeKind,
    resourceType,
    isRemote,
  } = data;
  const isResource = nodeKind === "resource";
  const typeLabel = isResource ? camelCaseToWords(resourceType.split("/").pop()) : "Module";
  const theme = useTheme();
  const frameRef = useRef<HTMLDivElement>(null);
  const iconRef = useRef<HTMLDivElement>(null);
  const isFocused = useIsNodeFocused(id);
  const reduceMotion = useIsMotionReduced();
  const { toggleModuleCollapsed, restoreModule } = useGraphActions();
  const isRestoring = useIsModuleRestoring(id);
  const isAtomic = kind === "atomic";
  const borderWidth = hasError ? theme.node.errorBorderWidth : theme.node.borderWidth;
  const rowHeight = getRowHeight(kind, borderWidth);
  const iconSize = isAtomic ? ATOMIC_ICON_SIZE : HEADER_ICON_SIZE;
  const toggle = getToggle(data, isRestoring);
  const transition = reduceMotion ? NO_TRANSITION : NODE_MORPH_TRANSITION;

  // The graph morphs the node's box when the module opens or closes; the icon and background change
  // with it. The header and type label animate through their own props.
  const previousKindRef = useRef(kind);
  useLayoutEffect(() => {
    const previousKind = previousKindRef.current;
    previousKindRef.current = kind;
    const frame = frameRef.current;
    const icon = iconRef.current;

    if (previousKind === kind || reduceMotion || !frame || !icon) {
      return;
    }

    const background = (nodeKind: NodeKind) => getFrameBackground(nodeKind, theme);
    const otherIconSize = isAtomic ? HEADER_ICON_SIZE : ATOMIC_ICON_SIZE;
    const iconControls = animate(icon, { scale: [otherIconSize / iconSize, 1] }, NODE_MORPH_TRANSITION);
    const backgroundControls = animate(
      frame,
      { backgroundColor: [background(previousKind), background(kind)] },
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
  }, [iconSize, isAtomic, kind, reduceMotion, theme]);

  const isToggleDisabled = toggle?.isDisabled === true;
  const handleToggle = useCallback(() => {
    if (isToggleDisabled) {
      return;
    }

    if (canCollapse) {
      toggleModuleCollapsed(id);
      return;
    }

    void restoreModule(id);
  }, [canCollapse, id, isToggleDisabled, restoreModule, toggleModuleCollapsed]);

  return (
    <$Frame
      ref={frameRef}
      $hasError={hasError}
      $isCollection={isCollection}
      $isFocused={isFocused}
      $kind={kind}
      data-focused={isFocused}
      data-module-collapsed={isCollapsed}
    >
      <$Row initial={false} animate={{ height: rowHeight }} transition={transition}>
        <$Icon ref={iconRef}>
          {isRemote && !isResource ? (
            <RemoteFolderIcon size={iconSize} />
          ) : (
            <AzureIcon resourceType={isResource ? resourceType : "folder"} size={iconSize} />
          )}
        </$Icon>
        <$Text $isInline={!isAtomic}>
          <$SymbolicName>{symbolicName}</$SymbolicName>
          {/* Every header names the kind of node, collapsed or not: a resource type, or "Module". */}
          <$TypeLabel
            initial={false}
            animate={isAtomic ? LABEL_SHOWN : { ...LABEL_SHOWN, marginTop: 0 }}
            transition={transition}
          >
            {typeLabel}
          </$TypeLabel>
        </$Text>
        {toggle && (
          <$ToggleButton
            {...graphInteractiveProps}
            type="button"
            aria-label={`${toggle.action} ${isResource ? "resource" : "module"} ${symbolicName}`}
            aria-expanded={toggle.action === "Collapse"}
            aria-busy={isRestoring}
            aria-disabled={isToggleDisabled}
            title={toggle.title}
            data-export-exclude=""
            data-source-status={canCollapse || isRestoring ? undefined : (sourceStatus ?? undefined)}
            $isRevealed={isFocused || isRestoring || toggle.isError === true}
            $isError={toggle.isError === true}
            onClick={handleToggle}
          >
            {isRestoring ? (
              <$ProgressRing data-testid="module-restore-progress">
                <Codicon name="loading" size={ICON_BUTTON_ICON_SIZE} />
              </$ProgressRing>
            ) : (
              <Codicon name={toggle.action === "Expand" ? "unfold" : "fold"} size={ICON_BUTTON_ICON_SIZE} />
            )}
          </$ToggleButton>
        )}
      </$Row>
    </$Frame>
  );
}
