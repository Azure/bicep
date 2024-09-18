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
  color: var(--vscode-editor-foreground);
  background-color: var(--vscode-editor-background);
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
