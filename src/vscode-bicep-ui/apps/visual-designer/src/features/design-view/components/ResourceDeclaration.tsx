// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzureIcon } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { camelCaseToWords } from "../../../utils/text";

export interface ResourceDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    resourceType: string;
    apiVersion: string;
  };
}

const $ResourceDelcarton = styled.div`
  flex: 1;
  display: flex;
  align-items: center;
  padding: 12px 16px;
  margin: 4px;
  box-sizing: border-box;
  border: 2px solid ${({ theme }) => theme.node.border};
  border-radius: 4px;
  background-color: ${({ theme }) => theme.node.background};
  height: 70px;
  min-width: 200px;
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
`;

export function ResourceDeclaration({ data }: ResourceDeclarationProps) {
  const { symbolicName, resourceType } = data;
  const resourceTypeDisplayName = camelCaseToWords(resourceType.split("/").pop());

  return (
    <$ResourceDelcarton>
      <AzureIcon resourceType={resourceType} size={36} />
      <$TextContainer>
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
        <$ResourceTypeContainer>{resourceTypeDisplayName}</$ResourceTypeContainer>
      </$TextContainer>
    </$ResourceDelcarton>
  );
}
