// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { ProgressLocation } from "vscode";
import { sleep } from "../../utils/time";
import { withProgressIfSlow } from "../../utils/withProgressIfSlow";
import { createWithProgressMock } from "../utils/vscodeMocks";

describe("withProgressIfSlow", () => {
  it("should not show progress notification if task is short - default delay", async () => {
    const withProgressMock = createWithProgressMock<string>();
    let isDone = false;

    const result: string = await withProgressIfSlow<string>(
      {
        location: ProgressLocation.Notification,
        mocks: { withProgress: withProgressMock },
      },
      async () => {
        await sleep(1);
        isDone = true;
        return "hi";
      }
    );

    expect(isDone).toBeTruthy();
    expect(withProgressMock).toHaveBeenCalledTimes(0);
    expect(result).toBe("hi");
  });

  it("should not show progress notification if task is short - short delay", async () => {
    const withProgressMock = createWithProgressMock<string>();
    let isDone = false;

    const result: string = await withProgressIfSlow<string>(
      {
        location: ProgressLocation.Notification,
        delayBeforeShowingMs: 10,
        mocks: { withProgress: withProgressMock },
      },
      async () => {
        await sleep(1);
        isDone = true;
        return "hi";
      }
    );

    expect(isDone).toBeTruthy();
    expect(withProgressMock).toHaveBeenCalledTimes(0);
    expect(result).toBe("hi");
  });


  it("should show progress notification if task takes longer than delay", async () => {
    const withProgressMock = createWithProgressMock<number>();
    let isDone = false;

    const result: number = await withProgressIfSlow(
      {
        location: ProgressLocation.Notification,
        delayBeforeShowingMs: 1,
        mocks: { withProgress: withProgressMock },
      },
      async () => {
        await sleep(10);
        isDone = true;
        return 123;
      }
    );

    expect(isDone).toBeTruthy();
    expect(withProgressMock).toHaveBeenCalledTimes(1);
    expect(result).toBe(123);
  });

  it("should handle throw before notification shows", async () => {
    const withProgressMock = createWithProgressMock();

    const func = async () =>
      withProgressIfSlow(
        {
          location: ProgressLocation.Notification,
          mocks: { withProgress: withProgressMock },
        },
        async () => {
          throw new Error("hah!");
        }
      );
    await expect(func).rejects.toThrow("hah!");

    expect(withProgressMock).toHaveBeenCalledTimes(0);
  });

  it("should handle throw after notification shows", async () => {
    const withProgressMock = createWithProgressMock();

    const func = async () =>
      withProgressIfSlow(
        {
          location: ProgressLocation.Notification,
          delayBeforeShowingMs: 1,
          mocks: { withProgress: withProgressMock },
        },
        async () => {
          await sleep(10);
          expect(withProgressMock).toHaveBeenCalledTimes(1);
          throw new Error("hah!");
        }
      );
    await expect(func).rejects.toThrow("hah!");

    expect(withProgressMock).toHaveBeenCalledTimes(1);
  });
});
