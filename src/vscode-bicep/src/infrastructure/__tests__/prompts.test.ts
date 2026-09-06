// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { QuickPickItem } from "vscode";

import { window } from "vscode";
import { Prompts } from "../prompts";

describe("Prompts", () => {
  test("moves the persisted selection to the front and retains focus", async () => {
    const values = new Map<string, unknown>();
    const globalState = {
      get: vi.fn(<T>(key: string): T | undefined => values.get(key) as T | undefined),
      update: vi.fn(async (key: string, value: unknown): Promise<void> => {
        values.set(key, value);
      }),
    };
    const first = { label: "First" };
    const second = { label: "Second" };
    const showQuickPick = vi
      .fn<(items: readonly QuickPickItem[], options: { ignoreFocusOut?: boolean }) => Promise<QuickPickItem>>()
      .mockImplementationOnce(async (items) => items[1])
      .mockImplementationOnce(async (items) => items[0]);
    window.showQuickPick = showQuickPick as unknown as typeof window.showQuickPick;

    await new Prompts(globalState).showQuickPick([first, second], {
      id: "picker",
      placeHolder: "Pick a value",
    });
    await new Prompts(globalState).showQuickPick([first, second], {
      id: "picker",
      placeHolder: "Pick a value",
    });

    expect(showQuickPick.mock.calls[1][0]).toEqual([second, first]);
    expect(showQuickPick.mock.calls[1][1].ignoreFocusOut).toBe(true);
  });

  test("retains focus for input boxes by default", async () => {
    const showInputBox = vi.fn(async () => "value");
    window.showInputBox = showInputBox;

    await new Prompts().showInputBox({ prompt: "Enter a value" });

    expect(showInputBox).toHaveBeenCalledWith({ prompt: "Enter a value", ignoreFocusOut: true });
  });
});
