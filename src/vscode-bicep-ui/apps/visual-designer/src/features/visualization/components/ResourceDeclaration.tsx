// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Range } from "../../../messages";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { camelCaseToWords } from "../../../utils/text";
import { focusedNodeIdAtom } from "../../graph-engine/atoms/nodes";

export interface ResourceDeclarationProps {
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

const $ResourceDelcarton = styled.div<{ $hasError?: boolean; $isCollection?: boolean; $isFocused?: boolean }>`
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
    box-shadow 180ms ease,
    transform 180ms ease;

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

export function ResourceDeclaration({ id, data }: ResourceDeclarationProps) {
  const { symbolicName, resourceType, isCollection, hasError } = data;
  const resourceTypeDisplayName = camelCaseToWords(resourceType.split("/").pop());
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const isFocused = focusedNodeId === id;

  return (
    <$ResourceDelcarton $hasError={hasError} $isCollection={isCollection} $isFocused={isFocused}>
      <AzureIcon resourceType={resourceType} size={36} />
      <$TextContainer>
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
        <$ResourceTypeContainer>{resourceTypeDisplayName}</$ResourceTypeContainer>
      </$TextContainer>
    </$ResourceDelcarton>
  );
}
