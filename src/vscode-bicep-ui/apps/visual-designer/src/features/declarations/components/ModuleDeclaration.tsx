import { styled } from "styled-components";

export interface ModuleDeclarationProps {
  id: string;
  data: {
    symbolicName: string;
    path: string;
  };
}

const $ModuleDelcarton = styled.div`
  flex: 1;
  display: flex;
  flex-direction: column;
  border: 1px solid #ccc;
  border-radius: 4px;
`;


export function ModuleDeclaration({ data }: ModuleDeclarationProps) {
  const { symbolicName, path } = data;

  return (
    <$ModuleDelcarton>
      <div>{symbolicName}</div>
      <div>{path}</div>
    </$ModuleDelcarton>
  );
}
