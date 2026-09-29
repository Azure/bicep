// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createContext } from "react";

export interface NodeDragCallbacks {
  /** A node started moving. A press that never moves is a click and does not call this. */
  onNodeDragStart: (nodeId: string) => void;
  /** A node that started moving was released. */
  onNodeDragEnd: (nodeId: string) => void;
}

export function ignoreNodeDrag() {}

/** Provided by `Graph` from its props, so nodes can report drags without prop drilling. */
export const NodeDragCallbacksContext = createContext<NodeDragCallbacks>({
  onNodeDragStart: ignoreNodeDrag,
  onNodeDragEnd: ignoreNodeDrag,
});
