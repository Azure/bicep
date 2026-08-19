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

type QuickPickState = Pick<Memento, "get" | "update">;
type PersistedQuickPickItem = QuickPickItem & {
  id?: string;
  suppressPersistence?: boolean;
};

export interface ErrorHandlingContext {
  suppressDisplay?: boolean;
  rethrow?: boolean;
  buttons?: ErrorMessageButton[];
}

export interface ErrorMessageButton extends MessageItem {
  callback(): void | Promise<void>;
}

export interface IActionContext {
  errorHandling: ErrorHandlingContext;
  ui: IAzureUserInput;
}

export interface IAzureQuickPickItem<T = undefined> extends QuickPickItem {
  data: T;
  id?: string;
  suppressPersistence?: boolean;
}

export interface UserInputQuickPickOptions extends QuickPickOptions {
  id?: string;
  suppressPersistence?: boolean;
}

export interface IAzureUserInput {
  showQuickPick<T extends QuickPickItem>(items: readonly T[], options: UserInputQuickPickOptions): Promise<T>;
  showInputBox(options: InputBoxOptions): Promise<string>;
  showOpenDialog(options: OpenDialogOptions): Promise<Uri[]>;
  showWarningMessage<T extends string>(message: string, ...items: T[]): Promise<T>;
  showWarningMessage<T extends MessageItem>(message: string, ...items: T[]): Promise<T>;
}

export class UserCancelledError extends Error {
  public constructor(message = "The operation was canceled.") {
    super(message);
    this.name = "UserCancelledError";
  }
}

export interface ParsedError {
  message: string;
  errorType: string;
  isUserCancelledError: boolean;
  stack?: string;
}

class UserInput implements IAzureUserInput {
  public constructor(private readonly globalState?: QuickPickState) {}

  public async showQuickPick<T extends QuickPickItem>(
    items: readonly T[],
    options: UserInputQuickPickOptions,
  ): Promise<T> {
    const recentlyUsedKey = this.getRecentlyUsedKey(options);
    const orderedItems = this.orderByRecentlyUsed(items, recentlyUsedKey);
    const result = ensureUserSelection(
      await window.showQuickPick(orderedItems, {
        ...options,
        ignoreFocusOut: options.ignoreFocusOut ?? true,
      }),
    );

    if (recentlyUsedKey && !(result as PersistedQuickPickItem).suppressPersistence) {
      await this.globalState?.update(recentlyUsedKey, getPseudonymousHash(getQuickPickIdentity(result)));
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

  private getRecentlyUsedKey(options: UserInputQuickPickOptions): string | undefined {
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
        !(item as PersistedQuickPickItem).suppressPersistence &&
        getPseudonymousHash(getQuickPickIdentity(item)) === recentlyUsedValue,
    );
    if (recentlyUsedIndex > 0) {
      const [recentlyUsedItem] = orderedItems.splice(recentlyUsedIndex, 1);
      orderedItems.unshift(recentlyUsedItem);
    }

    return orderedItems;
  }
}

export function createActionContext(globalState?: QuickPickState): IActionContext {
  return {
    errorHandling: {},
    ui: new UserInput(globalState),
  };
}

export async function runWithErrorHandling<T>(
  callback: (context: IActionContext) => T | PromiseLike<T>,
  onError?: (error: unknown) => void,
  globalState?: QuickPickState,
): Promise<T | undefined> {
  const context = createActionContext(globalState);

  try {
    return await callback(context);
  } catch (error) {
    const parsedError = parseError(error);
    if (parsedError.isUserCancelledError) {
      return undefined;
    }

    onError?.(error);

    if (!context.errorHandling.suppressDisplay) {
      const buttons = context.errorHandling.buttons ?? [];
      void window.showErrorMessage(parsedError.message, ...buttons).then(async (selected) => {
        await selected?.callback();
      });
    }

    if (context.errorHandling.rethrow) {
      throw error;
    }

    return undefined;
  }
}

export function parseError(error: unknown): ParsedError {
  if (error instanceof Error) {
    return {
      message: error.message,
      errorType: error.name,
      isUserCancelledError: error instanceof UserCancelledError,
      stack: error.stack,
    };
  }

  return {
    message: String(error),
    errorType: typeof error,
    isUserCancelledError: false,
  };
}

export function nonNullProp<T, K extends keyof T>(value: T, property: K): NonNullable<T[K]> {
  const propertyValue = value[property];
  if (propertyValue === null || propertyValue === undefined) {
    throw new Error(`Property '${String(property)}' is required.`);
  }

  return propertyValue;
}

function ensureUserSelection<T>(value: T | undefined): T {
  if (value === undefined) {
    throw new UserCancelledError();
  }

  return value;
}

function getQuickPickIdentity(item: QuickPickItem): string {
  return (item as PersistedQuickPickItem).id ?? item.label;
}

function getPseudonymousHash(value: string): string {
  return createHash("sha256").update(value).digest("hex");
}
