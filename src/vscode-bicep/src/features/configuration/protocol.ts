// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProtocolRequestType } from "vscode-languageserver-protocol";

export interface CreateBicepConfigParams {
  destinationPath: string;
}

export interface GetRecommendedConfigLocationParams {
  bicepFilePath?: string;
}

export interface GetRecommendedConfigLocationResult {
  recommendedFolder?: string;
  error?: string;
}

export const getRecommendedConfigLocationRequestType = new ProtocolRequestType<
  GetRecommendedConfigLocationParams,
  GetRecommendedConfigLocationResult,
  never,
  void,
  void
>("bicep/getRecommendedConfigLocation");
