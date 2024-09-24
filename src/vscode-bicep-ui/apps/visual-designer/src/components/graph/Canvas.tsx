import type { MouseEvent } from "react";

import { PanZoom } from "@vscode-bicep-ui/components";
import { useAtomCallback } from "jotai/utils";
import { useCallback } from "react";
import styled from "styled-components";
import { nodesAtom } from "./atoms";
import { Graph } from "./Graph";

const $PanZoom = styled(PanZoom)`
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
        set(node.origin, { ...get(node.origin) });
      }
    }, []),
  );

  return (
    <$PanZoom>
      <button onClick={layout}>Layout</button>
      <Graph />
    </$PanZoom>
  );
}
