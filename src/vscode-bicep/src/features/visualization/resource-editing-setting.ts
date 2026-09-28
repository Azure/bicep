// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { getBicepConfiguration } from "../../infrastructure/configuration";

export const resourceEditingSetting = "visualizer.experimental.enableResourceEditing";

export function isResourceEditingEnabled(): boolean {
  return getBicepConfiguration().get<boolean>(resourceEditingSetting, false);
}
