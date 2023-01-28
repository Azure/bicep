// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Memento } from "vscode";

export enum GlobalStateKeys {
  neverShowSurveyKey = "bicep.neverShowSurveys",
  annualSurveyStateKey = "bicep.annualSurveyState",
}

export const globalStateKeys = {};

export type GlobalState = Memento & {
  setKeysForSync(keys: readonly string[]): void;
};

/**
 * Call this once on activation to set the keys that should be synced between machines.
 */
export function setGlobalStateKeysToSyncBetweenMachines(
  globalState: GlobalState
) {
  // Any keys not in this list will remain separate for each local machine.
  globalState.setKeysForSync([
    GlobalStateKeys.neverShowSurveyKey,
    GlobalStateKeys.annualSurveyStateKey,
  ]);
}
