// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { Event } from "vscode";

// https://github.com/microsoft/vscode-azure-account/blob/main/src/azure-account.api.d.ts
// with just the properties we need
export interface AzureAccount {
  readonly status: AzureLoginStatus;
  readonly onStatusChanged: Event<AzureLoginStatus>;
}

export type AzureLoginStatus =
  | "Initializing"
  | "LoggingIn"
  | "LoggedIn"
  | "LoggedOut";
