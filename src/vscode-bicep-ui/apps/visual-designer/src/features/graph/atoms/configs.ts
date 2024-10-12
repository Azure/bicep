import type { ComponentType } from "react";

import { atomWithNullCheck } from "../../../utils/jotai/atom-creators";

export interface NodeConfig {
  resolveNodeContentComponent: (data: unknown) => ComponentType<{ id: string, data: unknown }>;
}

export const nodeConfigAtom = atomWithNullCheck<NodeConfig>();
