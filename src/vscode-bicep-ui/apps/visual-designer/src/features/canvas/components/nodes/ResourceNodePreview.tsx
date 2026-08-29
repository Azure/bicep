// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { AzureIcon } from "@vscode-bicep-ui/components";
import styled from "styled-components";

export interface ResourceNodePreviewProps {
  fullyQualifiedType: string;
  testId?: string;
}

export const RESOURCE_NODE_PREVIEW_WIDTH = 140;
export const RESOURCE_NODE_PREVIEW_HEIGHT = 42;

const $Card = styled.div`
  display: grid;
  width: ${RESOURCE_NODE_PREVIEW_WIDTH}px;
  height: ${RESOURCE_NODE_PREVIEW_HEIGHT}px;
  grid-template-columns: 20px minmax(0, 1fr);
  align-items: center;
  gap: 8px;
  padding: 0 9px;
  border: 1px solid var(--vscode-focusBorder);
  border-radius: 7px;
  color: var(--vscode-foreground);
  background: var(--vscode-editorWidget-background);
  box-shadow: 0 6px 18px var(--vscode-widget-shadow);
`;

const $TypeName = styled.span`
  min-width: 0;
  overflow: hidden;
  max-height: 30px;
  max-width: 92px;
  font-size: 12px;
  font-weight: 500;
  line-height: 15px;
  overflow-wrap: anywhere;
`;

export function ResourceNodePreview({ fullyQualifiedType, testId }: ResourceNodePreviewProps) {
  return (
    <$Card data-testid={testId}>
      <AzureIcon resourceType={fullyQualifiedType} size={18} />
      <$TypeName>{fullyQualifiedType.split("/").slice(-1)[0]}</$TypeName>
    </$Card>
  );
}
