// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { window } from "vscode";
import { OperationError, runWithErrorHandling, UserCancelledError } from "../errors";

describe("runWithErrorHandling", () => {
  test("ignores user cancellation", async () => {
    const onError = vi.fn();
    const showErrorMessage = vi.fn();
    window.showErrorMessage = showErrorMessage;

    const result = await runWithErrorHandling(
      async () => {
        throw new UserCancelledError();
      },
      { onError },
    );

    expect(result).toBeUndefined();
    expect(onError).not.toHaveBeenCalled();
    expect(showErrorMessage).not.toHaveBeenCalled();
  });

  test("uses operation-specific presentation and actions", async () => {
    const originalError = new Error("internal details");
    const action = { title: "Retry", run: vi.fn() };
    const onError = vi.fn();
    const showErrorMessage = vi.fn(async () => action);
    window.showErrorMessage = showErrorMessage;

    await runWithErrorHandling(
      async () => {
        throw new OperationError(originalError, {
          actions: [action],
          message: "The operation failed.",
        });
      },
      { onError },
    );
    await vi.waitFor(() => expect(action.run).toHaveBeenCalled());

    expect(onError).toHaveBeenCalledWith(originalError);
    expect(showErrorMessage).toHaveBeenCalledWith("The operation failed.", action);
  });

  test("suppresses display when requested", async () => {
    const showErrorMessage = vi.fn();
    window.showErrorMessage = showErrorMessage;

    await runWithErrorHandling(async () => {
      throw new OperationError(new Error("already handled"), { display: false });
    });

    expect(showErrorMessage).not.toHaveBeenCalled();
  });

  test("rethrows the original error when requested", async () => {
    const originalError = new Error("failure");
    window.showErrorMessage = vi.fn(async () => undefined);

    await expect(
      runWithErrorHandling(
        async () => {
          throw originalError;
        },
        { rethrow: true },
      ),
    ).rejects.toBe(originalError);
  });
});
