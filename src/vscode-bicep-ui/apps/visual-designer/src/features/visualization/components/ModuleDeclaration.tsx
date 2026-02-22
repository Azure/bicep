// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Range } from "../../../messages";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { focusedNodeIdAtom } from "../../graph-engine/atoms/nodes";

export interface ModuleDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    path: string;
    isCollection?: boolean;
    hasError?: boolean;
    range?: Range;
    filePath?: string;
  };
}

const STACK_OFFSET = 8;

const $ModuleDelcarton = styled.div<{ $hasError?: boolean; $isCollection?: boolean; $isFocused?: boolean }>`
  position: relative;
  flex: 1;
  margin: 4px;
  box-sizing: border-box;
  border: 2px solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 4px;
  background: ${({ theme }) => theme.node.compoundBackground};
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

const $DeclarationInfo = styled.div`
  display: flex;
  font-size: 14px;
  font-weight: 500;
  align-items: center;
  margin: 12px;
`;

const $SymbolicNameContainer = styled.div`
  font-size: 14px;
  color: ${({ theme }) => theme.text.primary};
  margin-bottom: 2px;
  margin-left: 8px;
`;

export function ModuleDeclaration({ id, data }: ModuleDeclarationProps) {
  const { symbolicName, isCollection, hasError } = data;
  const focusedNodeId = useAtomValue(focusedNodeIdAtom);
  const isFocused = focusedNodeId === id;

  return (
    <$ModuleDelcarton $hasError={hasError} $isCollection={isCollection} $isFocused={isFocused}>
      <$DeclarationInfo>
        <AzureIcon resourceType={"folder"} size={24} />
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
      </$DeclarationInfo>
    </$ModuleDelcarton>
  );
}
