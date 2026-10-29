// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { CanvasNodeData } from "@/core";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { motion } from "motion/react";
import { styled } from "styled-components";
import { EXPAND_TRANSITION } from "@/ui";
import { camelCaseToWords } from "@/utils";
import { useIsNodeFocused } from "../../hooks/use-is-node-focused";
import { RESOURCE_NODE_PREVIEW_HEIGHT, RESOURCE_NODE_PREVIEW_WIDTH } from "./ResourceNodePreview";

export interface ResourceNodeProps {
  id: string;
  data: CanvasNodeData;
}

const $ResourceNode = styled(motion.div)<{
  $hasError?: boolean;
  $isCollection?: boolean;
  $isFocused?: boolean;
}>`
  position: relative;
  flex: 1;
  display: flex;
  align-items: center;
  padding: 14px 20px;
  margin: 4px;
  box-sizing: border-box;
  border: ${({ $hasError, theme }) => ($hasError ? theme.node.errorBorderWidth : theme.node.borderWidth)} solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 8px;
  background-color: ${({ theme }) => theme.node.background};
  height: 76px;
  min-width: 220px;
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

  ${({ $isCollection, $hasError, $isFocused, theme }) => {
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
      background-color: ${theme.node.background};
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

const $TextContainer = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: center;
  margin-left: 12px;
  margin-right: 4px;
  gap: 2px;
  height: 100%;
  overflow: hidden;
`;

const $ResourceIcon = styled(motion.div)`
  display: flex;
  width: 36px;
  height: 36px;
  flex: 0 0 auto;
`;

const $SymbolicNameContainer = styled.div`
  font-size: 15px;
  font-weight: 600;
  color: ${({ theme }) => theme.text.primary};
  letter-spacing: -0.01em;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const $ResourceTypeContainer = styled.div`
  font-size: 12px;
  font-weight: 500;
  color: ${({ theme }) => theme.text.secondary};
  letter-spacing: 0.02em;
  text-transform: uppercase;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

export function ResourceNode({ id, data }: ResourceNodeProps) {
  const { symbolicName, resourceType, isCollection, hasError, replacesPlaceholder } = data;
  const resourceTypeDisplayName = camelCaseToWords(resourceType.split("/").pop());
  const isFocused = useIsNodeFocused(id);
  const initialScaleX = RESOURCE_NODE_PREVIEW_WIDTH / 220;
  const initialScaleY = RESOURCE_NODE_PREVIEW_HEIGHT / 76;
  const initialIconScaleX = 18 / (36 * initialScaleX);
  const initialIconScaleY = 18 / (36 * initialScaleY);

  return (
    <$ResourceNode
      // Motion reads `initial` only on mount, so later updates to the node never replay the growth.
      initial={replacesPlaceholder ? { scaleX: initialScaleX, scaleY: initialScaleY } : false}
      animate={{ scaleX: 1, scaleY: 1 }}
      transition={EXPAND_TRANSITION}
      $hasError={hasError}
      $isCollection={isCollection}
      $isFocused={isFocused}
      data-focused={isFocused}
    >
      <$ResourceIcon
        initial={replacesPlaceholder ? { scaleX: initialIconScaleX, scaleY: initialIconScaleY } : false}
        animate={{ scaleX: 1, scaleY: 1 }}
        transition={EXPAND_TRANSITION}
      >
        <AzureIcon resourceType={resourceType} size={36} />
      </$ResourceIcon>
      <$TextContainer>
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
        <$ResourceTypeContainer>{resourceTypeDisplayName}</$ResourceTypeContainer>
      </$TextContainer>
    </$ResourceNode>
  );
}
