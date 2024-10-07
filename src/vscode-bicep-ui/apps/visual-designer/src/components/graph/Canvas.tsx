import type { MouseEvent } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import styled from "styled-components";
import { isNode, nodesAtom } from "./atoms";
import { Graph } from "./Graph";

const $CanvasPanel = styled(PanZoom)`
  position: absolute;
  left: 0px;
  top: 0px;
  right: 0px;
  bottom: 0px;
  overflow: hidden;
`;

export function Canvas() {
  const layout = useAtomCallback(
    useCallback((get, set, event: MouseEvent<HTMLButtonElement>) => {
      event.stopPropagation();

      const nodes = get(nodesAtom);
      for (const node of Object.values(nodes)) {
        if (isNode(node)) {
          set(node.origin, { ...get(node.origin) });
        }
      }
    }, []),
  );

  return (
    <$CanvasPanel>
      <button onClick={layout}>Layout</button>
      <Graph />
    </$CanvasPanel>
  );
}
