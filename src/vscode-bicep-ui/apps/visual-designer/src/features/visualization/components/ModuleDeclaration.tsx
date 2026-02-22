// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Range } from "../../../messages";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";

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

const STACK_OFFSET = 7;

const $ModuleDelcarton = styled.div<{ $hasError?: boolean; $isCollection?: boolean }>`
  position: relative;
  flex: 1;
  margin: 4px;
  box-sizing: border-box;
  border: 2px solid ${({ $hasError, theme }) => ($hasError ? theme.error : theme.node.border)};
  border-radius: 4px;
  background: ${({ theme }) => theme.node.compoundBackground};
  transition: border-color 0.15s ease, box-shadow 0.15s ease;

  &:hover {
    border-color: ${({ $hasError, theme }) => ($hasError ? theme.error : theme.node.hoverBorder)};
    box-shadow: ${({ $hasError, theme }) => ($hasError ? theme.node.hoverErrorShadow : theme.node.hoverShadow)};
  }

  ${({ $isCollection, $hasError, theme }) =>
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
      border: 2px solid ${$hasError ? theme.error : theme.node.border};
      border-radius: 4px;
      background-color: ${theme.node.background};
      z-index: -1;
      transition: border-color 0.15s ease, box-shadow 0.15s ease;
    }
    &:hover::before {
      border-color: ${$hasError ? theme.error : theme.node.hoverBorder};
      box-shadow: ${$hasError ? theme.node.hoverErrorShadow : theme.node.hoverShadow};
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

export function ModuleDeclaration({ data }: ModuleDeclarationProps) {
  const { symbolicName, isCollection, hasError } = data;

  return (
    <$ModuleDelcarton $hasError={hasError} $isCollection={isCollection}>
      <$DeclarationInfo>
        <AzureIcon resourceType={"folder"} size={24} />
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
      </$DeclarationInfo>
    </$ModuleDelcarton>
  );
}
