// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
export interface Command {
  readonly id: string;

  execute(...args: unknown[]): unknown;
}
