import { styled } from "styled-components";

import { EdgeLayer } from "./EdgeLayer";
import { NodeLayer } from "./NodeLayer";
import { useStore } from "../stores";
// import { store } from "../stores";

const $Graph = styled.div.attrs<{
  $x: number;
  $y: number;
  $scale: number;
}>(({ $x, $y, $scale }) => ({
  style: {
    transform: `translate(${$x}px,${$y}px) scale(${$scale})`,
  },
}))`
  transform-origin: 0 0;
  height: 0;
  width: 0;
`;

// const $Svg = styled.svg`
//   overflow: visible;
// `;

export function Graph() {
  const { x, y } = useStore(x => x.position);
  const scale = useStore(x => x.scale);

  return (
    <$Graph $x={x} $y={y} $scale={scale}>
      <NodeLayer />
      <EdgeLayer />
    </$Graph>
  );
}
