import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
import { StraightEdge } from "./StraightEdge";
import { EdgeMarkerDefs } from "./EdgeMarkerDefs";

const $Svg = styled.svg`
  overflow: visible;
  position: absolute;
`;

export function EdgeLayer() {
  const edges = useAtomValue(edgesAtom);

  return (
    <$Svg>
      <EdgeMarkerDefs />
      {edges.map((edge) => (
        <StraightEdge key={edge.id} {...edge} />
      ))}
    </$Svg>
  );
}
