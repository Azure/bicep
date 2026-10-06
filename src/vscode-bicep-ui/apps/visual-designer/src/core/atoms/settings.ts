// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Settings } from "../api";

import { atom } from "jotai";

/** VS Code settings the webview needs, as the host last sent them. */
export const settingsAtom = atom<Settings>({ motionPolicy: "system", isResourceEditingEnabled: false });

/**
 * Whether the experimental `bicep.visualizer.experimental.enableResourceEditing` setting is on.
 *
 * It gates every action that edits Bicep source: creating resources and undoing or redoing them.
 * The extension rechecks it before applying any edit, so this only decides what the UI offers.
 */
export const isResourceEditingEnabledAtom = atom((get) => get(settingsAtom).isResourceEditingEnabled);

/** Whether animations should play, snap, or follow the OS preference (`system`). */
export const motionPolicyAtom = atom((get) => get(settingsAtom).motionPolicy);
