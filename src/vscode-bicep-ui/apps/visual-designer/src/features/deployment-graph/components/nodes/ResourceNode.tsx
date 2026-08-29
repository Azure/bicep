// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Range } from "../../api";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { useAtom, useAtomValue } from "jotai";
import { motion } from "motion/react";
import { useEffect } from "react";
import { styled } from "styled-components";
import { focusedNodeIdAtom } from "@/lib/graph";
import { camelCaseToWords } from "@/lib/utils";
import { RESOURCE_CREATION_TRANSITION } from "../../animations";
import { resourceNodeIsCommittingAtomFamily } from "../../atoms";
import { RESOURCE_NODE_PREVIEW_HEIGHT, RESOURCE_NODE_PREVIEW_WIDTH } from "./ResourceNodePreview";

export interface ResourceNodeProps {
  id: string;
  data: {
    symbolicName: string;
    resourceType: string;
    isCollection?: boolean;
    hasError?: boolean;
    range?: Range;
    filePath?: string;
  };
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
  const { symbolicName, resourceType, isCollection, hasError } = data;
  const normalizedResourceType = resourceType ?? "<unknown>";
  const resourceTypeDisplayName = camelCaseToWords(normalizedResourceType.split("/").pop());
  // Modules demoted to atomic (no children) render here with
  // resourceType "<module>".  Use the folder icon to match the
  // compound module styling.
  const iconType = normalizedResourceType === "<module>" ? "folder" : normalizedResourceType;
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const [isCommitting, setIsCommitting] = useAtom(resourceNodeIsCommittingAtomFamily(id));
  const isFocused = focusedNodeId === id;
  const initialCardScaleX = RESOURCE_NODE_PREVIEW_WIDTH / 220;
  const initialCardScaleY = RESOURCE_NODE_PREVIEW_HEIGHT / 76;
  const initialIconScaleX = 18 / (36 * initialCardScaleX);
  const initialIconScaleY = 18 / (36 * initialCardScaleY);

  useEffect(
    () => () => {
      resourceNodeIsCommittingAtomFamily.remove(id);
    },
    [id],
  );

  return (
    <$ResourceNode
      initial={isCommitting ? { scaleX: initialCardScaleX, scaleY: initialCardScaleY } : false}
      animate={{ scaleX: 1, scaleY: 1 }}
      transition={RESOURCE_CREATION_TRANSITION}
      onAnimationComplete={() => {
        if (isCommitting) {
          setIsCommitting(false);
        }
      }}
      data-committing={isCommitting}
      $hasError={hasError}
      $isCollection={isCollection}
      $isFocused={isFocused}
    >
      <$ResourceIcon
        initial={isCommitting ? { scaleX: initialIconScaleX, scaleY: initialIconScaleY } : false}
        animate={{ scaleX: 1, scaleY: 1 }}
        transition={RESOURCE_CREATION_TRANSITION}
      >
        <AzureIcon resourceType={iconType} size={36} />
      </$ResourceIcon>
      <$TextContainer>
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
        <$ResourceTypeContainer>{resourceTypeDisplayName}</$ResourceTypeContainer>
      </$TextContainer>
    </$ResourceNode>
  );
}
