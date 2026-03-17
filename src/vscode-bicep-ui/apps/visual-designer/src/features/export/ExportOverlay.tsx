// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { useSetAtom } from "jotai";
import { useCallback, useLayoutEffect } from "react";
import { styled } from "styled-components";
import { closeExportOverlayAtom } from "./atoms";
import { ExportToolbar } from "./ExportToolbar";

const $OverlayContainer = styled.div`
  position: absolute;
  top: 16px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 200;
`;

/**
 * Floating export toolbar that appears over the existing canvas.
 * No modal, no isolated store — the user works on the live graph
 * and exports it directly.
 */
export function ExportOverlay() {
  const closeExportOverlay = useSetAtom(closeExportOverlayAtom);

  const handleClose = useCallback(() => {
    closeExportOverlay();
  }, [closeExportOverlay]);

  // Close on Escape key.
  useLayoutEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (e.key === "Escape") handleClose();
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [handleClose]);

  return (
    <$OverlayContainer>
      <ExportToolbar />
    </$OverlayContainer>
  );
}
