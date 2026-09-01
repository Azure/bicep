// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { getBicepConfiguration } from "../../infrastructure/configuration";

export const resourceCreationSetting = "visualizer.experimental.enableResourceCreation";

export function isResourceCreationEnabled(): boolean {
  return getBicepConfiguration().get<boolean>(resourceCreationSetting, false);
}
