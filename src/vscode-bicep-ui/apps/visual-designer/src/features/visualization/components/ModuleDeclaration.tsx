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

const $ModuleDelcarton = styled.div<{ $hasError?: boolean; $isCollection?: boolean; $isFocused?: boolean }>`
  position: relative;
  flex: 1;
  margin: 4px;
  box-sizing: border-box;
  border: ${({ $hasError, theme }) => ($hasError ? theme.node.errorBorderWidth : theme.node.borderWidth)} solid
    ${({ $hasError, $isFocused, theme }) =>
      $hasError ? theme.error : $isFocused ? theme.node.focusBorder : theme.node.border};
  border-radius: 8px;
  background: ${({ theme }) => theme.node.compoundBackground};
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
      background-color: ${theme.node.compoundBackground};
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

const $DeclarationInfo = styled.div`
  display: flex;
  font-size: 15px;
  font-weight: 600;
  align-items: center;
  padding: 12px 16px;
`;

const $SymbolicNameContainer = styled.div`
  font-size: 15px;
  font-weight: 600;
  color: ${({ theme }) => theme.text.primary};
  letter-spacing: -0.01em;
  margin-left: 8px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
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
