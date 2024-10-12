import type { ComponentType } from "react";

import { atomWithNullCheck } from "../../../utils/jotai/atom-creators";

export interface NodeConfig {
  resolveNodeComponent: (data: unknown) => ComponentType<{ id: string, data: unknown }>;
}

export const nodeConfigAtom = atomWithNullCheck<NodeConfig>();
