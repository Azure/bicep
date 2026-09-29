// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { TargetScope } from "../api";

import { atom } from "jotai";

/** What the host has told the webview about the Bicep file being visualized. */

/** The Bicep file's URI, or null before the host has sent one. */
export const documentUriAtom = atom<string | null>(null);

/** The Bicep file's `targetScope`, or null when there is no graph. */
export const targetScopeAtom = atom<TargetScope | null>(null);

/** The errors reported for the Bicep file, including those that belong to no node. */
export const documentErrorCountAtom = atom(0);
