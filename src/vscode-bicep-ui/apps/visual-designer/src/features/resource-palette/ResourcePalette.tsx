// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PointerEvent } from "react";

import { AzureIcon, Codicon } from "@vscode-bicep-ui/components";
import { useMemo, useState } from "react";
import styled from "styled-components";

export interface ResourceTypeCatalogEntry {
  resourceType: string;
  apiVersion: string;
}

export interface ResourceTypeCatalogGroup {
  group: string;
  resourceTypes: ResourceTypeCatalogEntry[];
}

export interface ResourceTypeReference {
  fullyQualifiedType: string;
  apiVersion: string;
}

export interface ResourcePaletteProps {
  catalog?: ResourceTypeCatalogGroup[];
  error?: unknown;
  onResourceTypeActivate?: (resourceType: ResourceTypeReference) => void;
  onResourceTypePointerDown?: (resourceType: ResourceTypeReference, event: PointerEvent<HTMLButtonElement>) => void;
}

const $Search = styled.label`
  display: flex;
  height: 30px;
  align-items: center;
  gap: 6px;
  margin: 0 10px 8px;
  padding: 0 8px;
  border: 1px solid var(--vscode-input-border, var(--vscode-widget-border));
  border-radius: 6px;
  color: var(--vscode-input-foreground);
  background: var(--vscode-input-background);

  &:focus-within {
    border-color: var(--vscode-focusBorder);
  }
`;

const $SearchInput = styled.input`
  min-width: 0;
  flex: 1;
  border: 0;
  outline: 0;
  color: inherit;
  background: transparent;
  font: inherit;

  &::placeholder {
    color: var(--vscode-input-placeholderForeground);
  }
`;

const $Groups = styled.div`
  display: flex;
  flex-direction: column;
  gap: 4px;
  padding: 0 6px 8px;
`;

const $Group = styled.details`
  border-radius: 7px;

  &[open] {
    background: color-mix(in srgb, var(--vscode-editorWidget-background) 70%, transparent);
  }

  &[open] > summary > :first-child {
    transform: rotate(90deg);
  }
`;

const $GroupHeader = styled.summary`
  display: flex;
  min-height: 30px;
  align-items: center;
  gap: 6px;
  padding: 0 7px;
  border-radius: 6px;
  color: var(--vscode-foreground);
  font-size: 12px;
  font-weight: 600;
  cursor: pointer;
  list-style: none;

  &::-webkit-details-marker {
    display: none;
  }

  &:hover {
    background: var(--vscode-toolbar-hoverBackground);
  }
`;

const $GroupName = styled.span`
  min-width: 0;
  flex: 1;
  overflow-wrap: anywhere;
`;

const $Count = styled.span`
  color: var(--vscode-descriptionForeground);
  font-size: 11px;
  font-weight: 400;
`;

const $Items = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2px;
  padding: 2px 4px 6px 22px;
`;

const $Item = styled.button`
  display: grid;
  width: 100%;
  min-height: 34px;
  grid-template-columns: 20px minmax(0, 1fr) auto;
  align-items: center;
  gap: 7px;
  padding: 3px 6px;
  border: 1px solid transparent;
  border-radius: 6px;
  color: var(--vscode-foreground);
  text-align: left;
  background: transparent;
  cursor: grab;
  touch-action: none;

  &:hover {
    border-color: var(--vscode-widget-border);
    background: var(--vscode-list-hoverBackground);
  }

  &:active {
    cursor: grabbing;
    color: var(--vscode-list-activeSelectionForeground);
    background: var(--vscode-list-activeSelectionBackground);
  }

  &:active > span:last-child {
    color: inherit;
    background: color-mix(in srgb, currentColor 14%, transparent);
  }

  &:disabled {
    cursor: default;
  }
`;

const $TypeName = styled.span`
  min-width: 0;
  line-height: 15px;
  overflow-wrap: anywhere;
`;

const $Version = styled.span`
  padding: 1px 5px;
  border-radius: 999px;
  color: var(--vscode-badge-foreground);
  background: var(--vscode-badge-background);
  font-size: 10px;
  font-variant-numeric: tabular-nums;
`;

const $Empty = styled.div`
  padding: 18px 12px;
  color: var(--vscode-descriptionForeground);
  text-align: center;
`;

export function ResourcePalette({
  catalog,
  error,
  onResourceTypeActivate,
  onResourceTypePointerDown,
}: ResourcePaletteProps) {
  const [query, setQuery] = useState("");
  const normalizedQuery = query.trim().toLocaleLowerCase();
  const filteredCatalog = useMemo(
    () =>
      catalog
        ?.map((group) => ({
          ...group,
          resourceTypes: group.resourceTypes.filter((resource) =>
            `${group.group}/${resource.resourceType}`.toLocaleLowerCase().includes(normalizedQuery),
          ),
        }))
        .filter((group) => group.resourceTypes.length > 0),
    [catalog, normalizedQuery],
  );

  if (error) {
    return <$Empty>Failed to load resource types.</$Empty>;
  }

  return (
    <>
      <$Search>
        <Codicon name="search" size={14} />
        <$SearchInput
          aria-label="Filter resource types"
          placeholder="Filter resource types"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
      </$Search>
      {!filteredCatalog ? (
        <$Empty>Loading resource types...</$Empty>
      ) : filteredCatalog.length === 0 ? (
        <$Empty>No matching resource types.</$Empty>
      ) : (
        <$Groups>
          {filteredCatalog.map(({ group, resourceTypes }) => (
            <$Group key={group} open={normalizedQuery.length > 0 || undefined}>
              <$GroupHeader>
                <Codicon name="chevron-right" size={14} />
                <$GroupName>{group}</$GroupName>
                <$Count>{resourceTypes.length}</$Count>
              </$GroupHeader>
              <$Items>
                {resourceTypes.map(({ resourceType, apiVersion }) => {
                  const fullyQualifiedType = `${group}/${resourceType}`;
                  return (
                    <$Item
                      key={`${resourceType}@${apiVersion}`}
                      disabled={!onResourceTypeActivate && !onResourceTypePointerDown}
                      onKeyDown={(event) => {
                        if ((event.key === "Enter" || event.key === " ") && onResourceTypeActivate) {
                          event.preventDefault();
                          onResourceTypeActivate({ fullyQualifiedType, apiVersion });
                        }
                      }}
                      onPointerDown={(event) => onResourceTypePointerDown?.({ fullyQualifiedType, apiVersion }, event)}
                    >
                      <AzureIcon resourceType={fullyQualifiedType} size={18} />
                      <$TypeName>{resourceType}</$TypeName>
                      <$Version>{apiVersion}</$Version>
                    </$Item>
                  );
                })}
              </$Items>
            </$Group>
          ))}
        </$Groups>
      )}
    </>
  );
}
