import { List } from "@vscode-bicep-ui/components";
import { styled } from "styled-components";
import { ResourceTypeGroupListItem } from "./ResourceTypeGroupListItem";

interface ResourceTypeListProps {
  group: string;
  resourceTypes: string[];
}

const $ResourceTypeList = styled(List)`
  padding: 0;
`;

export function ResourceTypeGroupList({ group, resourceTypes }: ResourceTypeListProps) {
  return (
    <$ResourceTypeList>
      {resourceTypes.map((resourceType) => (
        <ResourceTypeGroupListItem
          key={resourceType}
          group={group}
          resourceType={resourceType}
        />
      ))}
    </$ResourceTypeList>
  );
}
