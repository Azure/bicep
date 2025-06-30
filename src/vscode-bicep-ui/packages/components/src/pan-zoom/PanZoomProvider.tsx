// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { PropsWithChildren } from "react";

import { Provider } from "./atoms";

export function PanZoomProvider({ children }: PropsWithChildren) {
  return <Provider>{children}</Provider>;
}
