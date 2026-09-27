// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { MotionPolicy } from "../api";

import { atom } from "jotai";

/** VS Code settings the webview needs, kept in step with the host. */

/**
 * Whether the experimental `bicep.visualizer.experimental.enableResourceEditing` setting is on.
 *
 * It gates every action that edits Bicep source: creating resources and undoing or redoing them.
 * The extension rechecks it before applying any edit, so this only decides what the UI offers.
 */
export const isResourceEditingEnabledAtom = atom(false);

/** Whether animations should play, snap, or follow the OS preference (`system`). */
export const motionPolicyAtom = atom<MotionPolicy>("system");
