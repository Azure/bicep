// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useAtomValue } from "jotai";
import { isExportPreviewVisibleAtom } from "../atoms";
import { ExportAreaPreview } from "./ExportAreaPreview";
import { ExportOverlay } from "./ExportOverlay";

/** The screen-space export controls and boundary preview shown while export is open. */
export function ExportPreviewLayer() {
  const isVisible = useAtomValue(isExportPreviewVisibleAtom);

  if (!isVisible) {
    return null;
  }

  return (
    <>
      <ExportOverlay />
      <ExportAreaPreview />
    </>
  );
}
