// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Event } from "vscode";

import { TokenCredential } from "@azure/identity";
import { Environment } from "@azure/ms-rest-azure-env";

import type { TokenCredentialsBase } from "@azure/ms-rest-nodeauth";
// https://github.com/microsoft/vscode-azure-account/blob/main/src/azure-account.api.d.ts
// with just the properties we need
export interface AzureAccount {
  readonly sessions: AzureSession[];
  readonly status: AzureLoginStatus;
  readonly onStatusChanged: Event<AzureLoginStatus>;
}

export interface AzureSession {
  readonly environment: Environment;
  readonly userId: string;
  readonly tenantId: string;

  /**
   * The credentials object for azure-sdk-for-js modules https://github.com/azure/azure-sdk-for-js
   */
  readonly credentials2: TokenCredential;
}

export type AzureLoginStatus =
  | "Initializing"
  | "LoggingIn"
  | "LoggedIn"
  | "LoggedOut";
