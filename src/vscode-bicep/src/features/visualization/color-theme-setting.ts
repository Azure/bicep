// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { getBicepConfiguration } from "../../infrastructure/configuration";

export const colorThemeSetting = "visualizer.matchColorTheme";

export function isColorThemeMatched(): boolean {
  return getBicepConfiguration().get<boolean>(colorThemeSetting, false);
}
