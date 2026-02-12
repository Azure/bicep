// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzureIcon } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { camelCaseToWords } from "../../../utils/text";
import type { Range } from "../../../messages";

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

const STACK_OFFSET = 7;

const $ResourceDelcarton = styled.div<{ $hasError?: boolean; $isCollection?: boolean }>`
  position: relative;
  flex: 1;
  display: flex;
  align-items: center;
  padding: 12px 16px;
  margin: 4px;
  box-sizing: border-box;
  border: 2px solid ${({ $hasError, theme }) => ($hasError ? theme.error : theme.node.border)};
  border-radius: 4px;
  background-color: ${({ theme }) => theme.node.background};
  height: 70px;
  min-width: 200px;

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
    }
  `
      : ''}
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

export function ResourceDeclaration({ data }: ResourceDeclarationProps) {
  const { symbolicName, resourceType, isCollection, hasError } = data;
  const resourceTypeDisplayName = camelCaseToWords(resourceType.split("/").pop());

  return (
    <$ResourceDelcarton $hasError={hasError} $isCollection={isCollection}>
      <AzureIcon resourceType={resourceType} size={36} />
      <$TextContainer>
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
        <$ResourceTypeContainer>{resourceTypeDisplayName}</$ResourceTypeContainer>
      </$TextContainer>
    </$ResourceDelcarton>
  );
}
