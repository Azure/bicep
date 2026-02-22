// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Range } from "../../../messages";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { focusedNodeIdAtom } from "../../graph-engine/atoms/nodes";
import { camelCaseToWords } from "../../../utils/text";

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

const STACK_OFFSET = 8;

const $ResourceDelcarton = styled.div<{ $hasError?: boolean; $isCollection?: boolean; $isFocused?: boolean }>`
  position: relative;
  flex: 1;
  display: flex;
  align-items: center;
  padding: 12px 16px;
  margin: 4px;
  box-sizing: border-box;
  border: 2px solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 4px;
  background-color: ${({ theme }) => theme.node.background};
  height: 70px;
  min-width: 200px;
  box-shadow: ${({ $isFocused, $hasError, theme }) =>
    $isFocused ? `inset 0 0 0 1px ${$hasError ? theme.error : theme.node.focusBorder}` : "none"};
  transition: border-color 0.15s ease, box-shadow 0.15s ease;

  &:hover {
    border-color: ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.hoverBorder};
    box-shadow: ${({ $isFocused, $hasError, theme }) => {
      const focusShadow = $isFocused ? `inset 0 0 0 1px ${$hasError ? theme.error : theme.node.focusBorder}` : null;
      const hoverShadow = $hasError ? theme.node.hoverErrorShadow : theme.node.hoverShadow;
      return [focusShadow, hoverShadow !== "none" ? hoverShadow : null].filter(Boolean).join(", ") || "none";
    }};
  }

  ${({ $isCollection, $hasError, $isFocused, theme }) =>
    $isCollection
      ? `
    margin-right: ${4 + STACK_OFFSET}px;
    margin-bottom: ${4 + STACK_OFFSET}px;
    &::before {
      content: '';
      position: absolute;
      top: ${STACK_OFFSET}px;
      left: ${STACK_OFFSET}px;
      right: -${STACK_OFFSET}px;
      bottom: -${STACK_OFFSET}px;
      border: 2px solid ${$hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
      border-radius: 4px;
      background-color: ${theme.node.background};
      z-index: -1;
      box-shadow: ${$isFocused ? `inset 0 0 0 1px ${$hasError ? theme.error : theme.node.focusBorder}` : "none"};
      transition: border-color 0.15s ease, box-shadow 0.15s ease;
    }
    &:hover::before {
      border-color: ${$hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.hoverBorder};
    }
  `
      : ""}
`;

const $TextContainer = styled.div`
  display: flex;
  flex-direction: column;
  justify-content: center;
  margin-left: 12px;
  margin-right: 2px;
  margin-bottom: 4px;
  height: 100%;
`;

const $SymbolicNameContainer = styled.div`
  font-size: 18px;
  font-weight: 500;
  color: ${({ theme }) => theme.text.primary};
`;

const $ResourceTypeContainer = styled.div`
  font-size: 12px;
  font-weight: 500;
  color: ${({ theme }) => theme.text.secondary};
  text-transform: uppercase;
  white-space: nowrap;
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
