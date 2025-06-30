// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { panZoomTransformAtom, useAtomValue } from "./atoms";

/**
 * A hook that returns the current pan-zoom transform value.
 *
 * @returns The current pan-zoom transform value.
 */
/**
 * Retrieves the pan-zoom transform value.
 *
 * @returns The pan-zoom transform value.
 */
export function usePanZoomTransform() {
  return useAtomValue(panZoomTransformAtom);
}
