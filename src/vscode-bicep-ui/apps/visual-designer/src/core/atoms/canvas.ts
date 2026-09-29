// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/**
 * The element the canvas renders into, or null before it mounts. Shared through core because both the
 * canvas and export need it, and export cannot depend on the canvas feature, which renders its overlays.
 */
export const canvasElementAtom = atom<HTMLDivElement | null>(null);
