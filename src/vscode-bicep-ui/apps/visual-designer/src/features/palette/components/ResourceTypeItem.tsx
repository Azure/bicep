// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent, ReactNode } from "react";
import type { PaletteContentProps } from "./PaletteContent";

import { AzureIcon } from "@vscode-bicep-ui/components";
import { useCallback } from "react";
import styled from "styled-components";
import { useResourceTypeVersions } from "../hooks/use-resource-type-versions";
import { ApiVersionPicker } from "./ApiVersionPicker";

/** Pointer-only drag source; resources are added by dragging them onto the canvas. */
const $DragHandle = styled.div`
  display: flex;
  min-width: 0;
  flex: 1;
  align-items: center;
  align-self: stretch;
  gap: 8px;
  padding: 4px 0 4px 8px;
  cursor: grab;
  touch-action: none;
  user-select: none;

  &:active {
    cursor: grabbing;
  }
`;

const $Row = styled.div`
  display: flex;
  align-items: center;
  gap: 8px;
  min-height: 28px;
  padding-right: 8px;
  border: 1px solid transparent;
  border-radius: 4px;

  &:hover,
  &:focus-within {
    --api-version-pill-border: ${({ theme }) => theme.panel.border};
    border-color: var(--vscode-contrastActiveBorder, transparent);
    background: ${({ theme }) => theme.iconButton.hoverBackground};
  }
`;

const $TypeName = styled.span`
  min-width: 0;
  line-height: 16px;
  overflow-wrap: anywhere;
`;

export function ResourceTypeItem({
  fullyQualifiedType,
  defaultVersion,
  children,
  loadVersions,
  onResourceTypePointerDown,
}: {
  fullyQualifiedType: string;
  defaultVersion: string;
  children: ReactNode;
  loadVersions: PaletteContentProps["loadVersions"];
  onResourceTypePointerDown: PaletteContentProps["onResourceTypePointerDown"];
}) {
  const { apiVersion, state, load, select } = useResourceTypeVersions(fullyQualifiedType, defaultVersion, loadVersions);
  const handlePointerDown = useCallback(
    (event: PointerEvent<HTMLDivElement>) => onResourceTypePointerDown?.({ fullyQualifiedType, apiVersion }, event),
    [apiVersion, fullyQualifiedType, onResourceTypePointerDown],
  );
  const handleLoad = useCallback(() => void load(), [load]);

  return (
    <$Row data-testid="resource-type-row">
      <$DragHandle
        title={`Drag ${fullyQualifiedType} onto the canvas`}
        data-testid="resource-type-drag-handle"
        data-resource-type={fullyQualifiedType}
        onPointerDown={handlePointerDown}
      >
        <AzureIcon resourceType={fullyQualifiedType} size={16} />
        <$TypeName>{children}</$TypeName>
      </$DragHandle>
      <ApiVersionPicker
        fullyQualifiedType={fullyQualifiedType}
        apiVersion={apiVersion}
        defaultVersion={defaultVersion}
        state={state}
        load={handleLoad}
        select={select}
      />
    </$Row>
  );
}
