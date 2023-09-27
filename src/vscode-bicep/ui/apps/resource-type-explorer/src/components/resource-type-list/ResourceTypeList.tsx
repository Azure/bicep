import { List } from "@vscode-bicep/ui-components";
import styled from "styled-components";

import { ResourceTypeListItem } from "./ResourceTypeListItem";

interface ResourceTypeListProps {
  resourceProvider: string;
  resourceTypes: string[];
}

const $ResourceTypeList = styled(List)`
  padding: 0;
`;

export function ResourceTypeList({
  resourceProvider,
  resourceTypes,
}: ResourceTypeListProps) {
  return (
    <$ResourceTypeList>
      {resourceTypes.map((resourceType, i) => (
        <ResourceTypeListItem
          key={i}
          resourceProvider={resourceProvider}
          resourceType={resourceType}
        />
      ))}
    </$ResourceTypeList>
  );
}
