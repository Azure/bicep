// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { List } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { ResourceTypeGroupListItem } from "./ResourceTypeGroupListItem";
import type { ResourceTypes } from "./App";

interface ResourceTypeListProps {
  group: string;
  resourceTypes: ResourceTypes[];
}

const $ResourceTypeList = styled(List)`
  padding: 0;
  color: var(--vscode-sideBar-foreground);
  background-color: var(--vscode-sideBar-background);
`;

export function ResourceTypeGroupList({ group, resourceTypes }: ResourceTypeListProps) {
  return (
    <$ResourceTypeList>
      {resourceTypes.map((resourceType) => (
        <ResourceTypeGroupListItem
          key={resourceType.resourceType}
          group={group}
          resourceType={resourceType.resourceType}
          apiVersion={resourceType.apiVersion}
        />
      ))}
    </$ResourceTypeList>
  );
}
