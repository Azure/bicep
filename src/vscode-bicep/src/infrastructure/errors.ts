// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { MessageItem, window } from "vscode";

export interface ErrorAction extends MessageItem {
  run(): void | Promise<void>;
}

export interface ErrorBoundaryOptions {
  displayErrors?: boolean;
  onError?: (error: unknown) => void;
  rethrow?: boolean;
}

export interface OperationErrorOptions {
  actions?: readonly ErrorAction[];
  display?: boolean;
  message?: string;
  rethrow?: boolean;
}

export class UserCancelledError extends Error {
  public constructor(message = "The operation was canceled.") {
    super(message);
    this.name = "UserCancelledError";
  }
}

export class OperationError extends Error {
  public constructor(
    public readonly originalError: unknown,
    public readonly options: OperationErrorOptions,
  ) {
    super(options.message ?? parseError(originalError).message);
    this.name = "OperationError";
  }
}

export interface ParsedError {
  message: string;
  errorType: string;
  isUserCancelledError: boolean;
  stack?: string;
}

export async function runWithErrorHandling<T>(
  callback: () => T | PromiseLike<T>,
  options: ErrorBoundaryOptions = {},
): Promise<T | undefined> {
  try {
    return await callback();
  } catch (caughtError) {
    const operationError = caughtError instanceof OperationError ? caughtError : undefined;
    const error = operationError?.originalError ?? caughtError;
    const parsedError = parseError(error);
    if (parsedError.isUserCancelledError) {
      return undefined;
    }

    options.onError?.(error);

    if (operationError?.options.display ?? options.displayErrors ?? true) {
      const actions = operationError?.options.actions ?? [];
      void window
        .showErrorMessage(operationError?.message ?? parsedError.message, ...actions)
        .then(async (selected) => {
          await selected?.run();
        });
    }

    if (operationError?.options.rethrow ?? options.rethrow ?? false) {
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
