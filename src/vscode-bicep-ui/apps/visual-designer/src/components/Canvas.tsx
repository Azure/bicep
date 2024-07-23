import { frame } from "framer-motion";
import { useSetAtom } from "jotai";
import { useCallback } from "react";
import { styled } from "styled-components";
import { useD3PanZoom } from "../hooks/useD3PanZoom";
import { panZoomAtom } from "../atoms/panZoom";

const $Canvas = styled.div`
  position: absolute;
  left: 0px;
  top: 0px;
  right: 0px;
  bottom: 0px;
  overflow: hidden;
`;

export function Canvas() {
  const setPanZoom = useSetAtom(panZoomAtom);
  const ref = useD3PanZoom(
    useCallback(
      (event) => {
        const { x, y, k: zoom } = event.transform;
        frame.update(() => {
          setPanZoom({ pan: { x, y }, zoom });
        });
      },
      [setPanZoom],
    ),
  );

  return <$Canvas ref={ref}></$Canvas>;
}
