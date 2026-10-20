// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { workspace } from "vscode";

export type VisualizerMotionPolicy = "system" | "reduce" | "animate";

export function resolveVisualizerMotionPolicy(setting: unknown): VisualizerMotionPolicy {
  return setting === "on" ? "reduce" : setting === "off" ? "animate" : "system";
}

export function getVisualizerMotionPolicy(): VisualizerMotionPolicy {
  return resolveVisualizerMotionPolicy(workspace.getConfiguration("workbench").get<unknown>("reduceMotion", "auto"));
}
