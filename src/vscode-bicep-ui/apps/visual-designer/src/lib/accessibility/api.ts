// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { defineNotification, defineRequest } from "@vscode-bicep-ui/messaging";

// ── Motion policy ──
// The host resolves the user's effective motion preference, combining the VS Code setting with the
// OS-level reduced-motion preference.

export type MotionPolicy = "system" | "reduce" | "animate";

export const getMotionPolicy = defineRequest<void, MotionPolicy>("motionPolicy/get");

export const motionPolicyDidChange = defineNotification<MotionPolicy>("motionPolicy/didChange");
