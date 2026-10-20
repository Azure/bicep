// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ResourceTypeReference } from "@/core";
import type { PaletteContentProps } from "./PaletteContent";

import styled from "styled-components";
import { ResourceTypeItem } from "./ResourceTypeItem";

const $List = styled.div`
  display: flex;
  flex-direction: column;
  padding: 4px 8px 8px;
`;

const $Namespace = styled.span`
  margin-left: 6px;
  color: ${({ theme }) => theme.text.secondary};
`;

/** Recently added types as a flat list, since they span providers. Each row names its provider. */
export function RecentResourceTypes({
  resourceTypes,
  loadVersions,
  onResourceTypePointerDown,
}: {
  resourceTypes: readonly ResourceTypeReference[];
  loadVersions: PaletteContentProps["loadVersions"];
  onResourceTypePointerDown?: PaletteContentProps["onResourceTypePointerDown"];
}) {
  return (
    <$List>
      {resourceTypes.map(({ fullyQualifiedType, apiVersion }) => {
        const separator = fullyQualifiedType.indexOf("/");

        return (
          <ResourceTypeItem
            key={fullyQualifiedType}
            fullyQualifiedType={fullyQualifiedType}
            defaultVersion={apiVersion}
            loadVersions={loadVersions}
            onResourceTypePointerDown={onResourceTypePointerDown}
          >
            {fullyQualifiedType.slice(separator + 1)}
            <$Namespace>{fullyQualifiedType.slice(0, separator)}</$Namespace>
          </ResourceTypeItem>
        );
      })}
    </$List>
  );
}
