import styled from "styled-components";
import { CollapseIndicator } from "./CollapseIndicator";

interface ResourceProviderHeaderProps {
  resourceProvider: string;
}

const $ResourceProviderHeader = styled.div`
  display: flex;
  flex-direction: row;
  gap: 2px;
`;

export function ResourceProviderHeader({
  resourceProvider,
}: ResourceProviderHeaderProps) {
  return (
    <$ResourceProviderHeader>
      <CollapseIndicator />
      <span>{resourceProvider}</span>
    </$ResourceProviderHeader>
  );
}
