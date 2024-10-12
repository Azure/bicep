import type { NodeConfig } from "../atoms";

import { useSetAtom } from "jotai";
import { useEffect } from "react";
import { nodeConfigAtom } from "../atoms";

export function useSetNodeConfig(nodeConfig: NodeConfig) {
  const setNodeConfig = useSetAtom(nodeConfigAtom);

  useEffect(() => {
    setNodeConfig(nodeConfig);
  }, [setNodeConfig, nodeConfig]);
}
