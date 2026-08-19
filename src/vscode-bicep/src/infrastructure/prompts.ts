// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { createHash } from "crypto";
import {
  InputBoxOptions,
  Memento,
  MessageItem,
  OpenDialogOptions,
  QuickPickItem,
  QuickPickOptions,
  Uri,
  window,
} from "vscode";
import { UserCancelledError } from "./errors";

export type PromptState = Pick<Memento, "get" | "update">;

type PersistedPromptItem = QuickPickItem & {
  id?: string;
  suppressPersistence?: boolean;
};

export interface PromptItem<T = undefined> extends QuickPickItem {
  data: T;
  id?: string;
  suppressPersistence?: boolean;
}

export interface PromptOptions extends QuickPickOptions {
  id?: string;
  suppressPersistence?: boolean;
}

export class Prompts {
  public constructor(private readonly globalState?: PromptState) {}

  public async showQuickPick<T extends QuickPickItem>(items: readonly T[], options: PromptOptions): Promise<T> {
    const recentlyUsedKey = this.getRecentlyUsedKey(options);
    const orderedItems = this.orderByRecentlyUsed(items, recentlyUsedKey);
    const result = ensureUserSelection(
      await window.showQuickPick(orderedItems, {
        ...options,
        ignoreFocusOut: options.ignoreFocusOut ?? true,
      }),
    );

    if (recentlyUsedKey && !(result as PersistedPromptItem).suppressPersistence) {
      await this.globalState?.update(recentlyUsedKey, getPseudonymousHash(getPromptIdentity(result)));
    }

    return result;
  }

  public async showInputBox(options: InputBoxOptions): Promise<string> {
    const result = await window.showInputBox({
      ...options,
      ignoreFocusOut: options.ignoreFocusOut ?? true,
    });
    return ensureUserSelection(result);
  }

  public async showOpenDialog(options: OpenDialogOptions): Promise<Uri[]> {
    const result = await window.showOpenDialog(options);
    return ensureUserSelection(result);
  }

  public async showWarningMessage<T extends string>(message: string, ...items: T[]): Promise<T>;
  public async showWarningMessage<T extends MessageItem>(message: string, ...items: T[]): Promise<T>;
  public async showWarningMessage<T extends string | MessageItem>(message: string, ...items: T[]): Promise<T> {
    const result = await window.showWarningMessage(message, ...(items as MessageItem[]));
    return ensureUserSelection(result as T | undefined);
  }

  private getRecentlyUsedKey(options: PromptOptions): string | undefined {
    const pickerIdentity = options.id ?? options.placeHolder;
    if (!this.globalState || options.canPickMany || options.suppressPersistence || !pickerIdentity) {
      return undefined;
    }

    return `showQuickPick.${getPseudonymousHash(pickerIdentity)}`;
  }

  private orderByRecentlyUsed<T extends QuickPickItem>(items: readonly T[], recentlyUsedKey?: string): T[] {
    const orderedItems = [...items];
    const recentlyUsedValue = recentlyUsedKey ? this.globalState?.get<string>(recentlyUsedKey) : undefined;
    if (!recentlyUsedValue) {
      return orderedItems;
    }

    const recentlyUsedIndex = orderedItems.findIndex(
      (item) =>
        !(item as PersistedPromptItem).suppressPersistence &&
        getPseudonymousHash(getPromptIdentity(item)) === recentlyUsedValue,
    );
    if (recentlyUsedIndex > 0) {
      const [recentlyUsedItem] = orderedItems.splice(recentlyUsedIndex, 1);
      orderedItems.unshift(recentlyUsedItem);
    }

    return orderedItems;
  }
}

function ensureUserSelection<T>(value: T | undefined): T {
  if (value === undefined) {
    throw new UserCancelledError();
  }

  return value;
}

function getPromptIdentity(item: QuickPickItem): string {
  return (item as PersistedPromptItem).id ?? item.label;
}

function getPseudonymousHash(value: string): string {
  return createHash("sha256").update(value).digest("hex");
}
