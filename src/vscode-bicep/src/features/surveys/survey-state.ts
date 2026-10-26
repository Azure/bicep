// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { Memento } from "vscode";

export const annualSurveyStateKey = "bicep.surveys.annualSurveyState";

export type GlobalState = Memento & {
  setKeysForSync(keys: readonly string[]): void;
};

export function setGlobalStateKeysToSyncBetweenMachines(globalState: GlobalState): void {
  globalState.setKeysForSync([annualSurveyStateKey]);
}
