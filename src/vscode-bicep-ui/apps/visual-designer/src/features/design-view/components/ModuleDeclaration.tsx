// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzureIcon } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";

export interface ModuleDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    path: string;
  };
}

const $ModuleDelcarton = styled.div`
  flex: 1;
  margin: 4px;
  box-sizing: border-box;
  border: 2px solid #333638;
  border-radius: 4px;
  background: #f9fafa;
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
  color: #242424;
  margin-bottom: 2px;
  margin-left: 8px;
`;

export function ModuleDeclaration({ data }: ModuleDeclarationProps) {
  const { symbolicName } = data;

  return (
    <$ModuleDelcarton>
      <$DeclarationInfo>
        <AzureIcon resourceType={"folder"} size={24} />
        <$SymbolicNameContainer>{symbolicName}</$SymbolicNameContainer>
      </$DeclarationInfo>
    </$ModuleDelcarton>
  );
}
