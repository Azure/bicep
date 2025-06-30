// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { panZoomControlAtom, useAtomValue } from "./atoms";

/**
 * A hook that provides access to the pan-zoom control, allowing for zooming in, zooming out, and resetting the pan-zoom transform values.
 *
 * @returns The pan-zoom control object, which includes functions for zooming in, zooming out, and resetting.
 */
export function usePanZoomControl() {
  const panZoomControl = useAtomValue(panZoomControlAtom);

  return panZoomControl;
}
