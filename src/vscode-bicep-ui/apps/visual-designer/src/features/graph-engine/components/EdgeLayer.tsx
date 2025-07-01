// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
import { StraightEdge } from "./StraightEdge";
import { EdgeMarkerDefs } from "./EdgeMarkerDefs";

const $Svg = styled.svg`
  overflow: visible;
  position: absolute;
  pointer-events: none;
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
