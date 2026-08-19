// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { QuickPickItem } from "vscode";

import { window } from "vscode";
import { createActionContext } from "../action-context";

describe("UserInput", () => {
  test("ShowQuickPick_WithPersistedSelection_MovesSelectionToFrontAndRetainsFocus", async () => {
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
    window.showQuickPick = showQuickPick as typeof window.showQuickPick;

    await createActionContext(globalState).ui.showQuickPick([first, second], {
      id: "picker",
      placeHolder: "Pick a value",
    });
    await createActionContext(globalState).ui.showQuickPick([first, second], {
      id: "picker",
      placeHolder: "Pick a value",
    });

    expect(showQuickPick.mock.calls[1][0]).toEqual([second, first]);
    expect(showQuickPick.mock.calls[1][1].ignoreFocusOut).toBe(true);
  });

  test("ShowInputBox_WithDefaultOptions_RetainsFocus", async () => {
    const showInputBox = vi.fn(async () => "value");
    window.showInputBox = showInputBox;

    await createActionContext().ui.showInputBox({ prompt: "Enter a value" });

    expect(showInputBox).toHaveBeenCalledWith({ prompt: "Enter a value", ignoreFocusOut: true });
  });
});
