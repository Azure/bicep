import { getDefaultStore } from "jotai";
import { useEffect, useRef } from "react";
import { styled } from "styled-components";
import { panZoomAtom } from "../atoms/panZoom";

const $Graph = styled.div`
  transform-origin: 0 0;
  height: 0;
  width: 0;
`;

const defaultStore = getDefaultStore();

export function Graph() {
  const ref = useRef<HTMLDivElement>(null);

  useEffect(
    () =>
      defaultStore.sub(panZoomAtom, () => {
        if (ref.current) {
          const { pan, zoom } = defaultStore.get(panZoomAtom);
          const { x, y } = pan;
          ref.current.style.transform = `translate(${x}px,${y}px) scale(${zoom})`;
        }
      }),
    [],
  );

  return <$Graph ref={ref}></$Graph>;
}
