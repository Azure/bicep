// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { atom } from "jotai";

/** The element the canvas renders into, or null before it mounts. */
export const canvasElementAtom = atom<HTMLDivElement | null>(null);
