// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { GlobalState } from "../../globalState";
import { sleep } from "../../utils/time";

export class GlobalStateFake implements GlobalState {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  private dictionary = new Map<string, any>();

  public keys(): readonly string[] {
    throw new Error("Method not implemented.");
  }

  public get<T>(key: string): T | undefined;
  public get<T>(key: string, defaultValue: T): T;
  public get<T>(
    key: string,
    defaultValue: T | undefined = undefined,
  ): T | undefined {
    return this.dictionary.get(key) || defaultValue;
  }

  public async update<T>(key: string, value: T): Promise<void> {
    await sleep(1);
    this.dictionary.set(key, value);
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  public setKeysForSync(keys: string[]): void {
    throw new Error("Method not implemented.");
  }
}
