// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

describe("Foo", () => {
  test("1 + 1 should equal 2", () => {
    vscode.window.showInformationMessage("1 + 1 = 2");

    expect(1 + 1).toBe(2);
  });
});
