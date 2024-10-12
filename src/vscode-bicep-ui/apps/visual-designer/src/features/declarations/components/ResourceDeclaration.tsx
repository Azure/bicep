import { styled } from "styled-components";

export interface ResourceDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    resourceType: string;
    apiVersion: string;
  }
}

const $ResourceDelcarton = styled.div`
  flex: 1;
  display: flex;
  flex-direction: column;
  border: 1px solid #ccc;
  border-radius: 4px;
`;

export function ResourceDeclaration({ data }: ResourceDeclarationProps) {
  const { symbolicName, resourceType } = data;

  return (
    <$ResourceDelcarton>
      <div>{symbolicName}</div>
      <div>{resourceType}</div>
    </$ResourceDelcarton>
  );
}
