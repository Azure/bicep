// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { styled } from "styled-components";
import { edgesAtom } from "../atoms/edges";
import { EdgeMarkerDefs } from "./EdgeMarkerDefs";
import { StraightEdge } from "./StraightEdge";

const $Svg = styled.svg`
  overflow: visible;
  position: absolute;
  pointer-events: none;
  z-index: 0;
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
