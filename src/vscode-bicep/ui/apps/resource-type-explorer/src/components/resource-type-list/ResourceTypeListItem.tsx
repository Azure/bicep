import { DragEvent } from "react";
import { AzureIcon, List } from "@vscode-bicep/ui-components";
import styled from "styled-components";

interface ResourceTypeListItemProps {
  resourceProvider: string;
  resourceType: string;
}

const $ResourceTypeListItem = styled(List.Item)`
  height: 22px;
  line-height: 22px;
  padding-left: 10px;
  gap: 6px;
  cursor: grab;
`;

export function ResourceTypeListItem({
  resourceProvider,
  resourceType,
}: ResourceTypeListItemProps) {
  function handleDragStart(event: DragEvent<HTMLLIElement>) {
    event.dataTransfer.setData("text", `${resourceProvider}/${resourceType}`);
  }

  return (
    <$ResourceTypeListItem draggable onDragStart={handleDragStart}>
      <AzureIcon
        resourceType={`${resourceProvider}/${resourceType}`}
        size={16}
      />
      <span>{resourceType}</span>
    </$ResourceTypeListItem>
  );
}
