// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

import {
  expectDefined,
  expectRange,
  getExampleBicepFilePath,
  marked,
  normalizeMarkedString,
  sleep,
} from "../utils";

describe("Hover tests", (): void => {
  let document: vscode.TextDocument;

  beforeAll(async (done) => {
    const exampleFilePath = getExampleBicepFilePath("101", "vm-simple-linux");
    document = await vscode.workspace.openTextDocument(exampleFilePath);
    await vscode.window.showTextDocument(document);

    // Wait for language server to be ready.
    await sleep(6000);
    done();
  }, 8000);

  afterAll(async () => {
    await vscode.commands.executeCommand("workbench.action.closeActiveEditor");
  });

  test("hovering over a parameter name should reveal the type signature of the parameter", async () => {
    const hovers = await vscode.commands.executeCommand<vscode.Hover[]>(
      "vscode.executeHoverProvider",
      document.uri,
      new vscode.Position(1, 7)
    );

    expectHovers(hovers, {
      startLine: 1,
      startCharacter: 6,
      endLine: 1,
      endCharacter: 12,
      contents: ["param vmName: string"],
    });
  });

  test("hovering over a variable name should reveal the type signature of the variable", async () => {
    const hovers = await vscode.commands.executeCommand<vscode.Hover[]>(
      "vscode.executeHoverProvider",
      document.uri,
      new vscode.Position(53, 10)
    );

    expectHovers(hovers, {
      startLine: 53,
      startCharacter: 4,
      endLine: 53,
      endCharacter: 22,
      contents: ["var linuxConfiguration: object"],
    });
  });

  test("hovering over a resource symbolic name should reveal the type signature of the resource", async () => {
    const hovers = await vscode.commands.executeCommand<vscode.Hover[]>(
      "vscode.executeHoverProvider",
      document.uri,
      new vscode.Position(111, 10)
    );

    expectHovers(hovers, {
      startLine: 111,
      startCharacter: 9,
      endLine: 111,
      endCharacter: 13,
      contents: ["resource vnet\nMicrosoft.Network/virtualNetworks@2020-06-01"],
    });
  });

  test("hovering over an output name should reveal the type signature of the output", async () => {
    const hovers = await vscode.commands.executeCommand<vscode.Hover[]>(
      "vscode.executeHoverProvider",
      document.uri,
      new vscode.Position(186, 14)
    );

    expectHovers(hovers, {
      startLine: 186,
      startCharacter: 7,
      endLine: 186,
      endCharacter: 28,
      contents: ["output administratorUsername: string"],
    });
  });

  test("hovering over a function name should reveal the type signature of the function", async () => {
    const hovers = await vscode.commands.executeCommand<vscode.Hover[]>(
      "vscode.executeHoverProvider",
      document.uri,
      new vscode.Position(19, 60)
    );

    expectHovers(hovers, {
      startLine: 19,
      startCharacter: 55,
      endLine: 19,
      endCharacter: 67,
      contents: ["function uniqueString(any): string"],
    });
  });

  function expectHovers(
    hovers: vscode.Hover[] | undefined,
    ...expectedHovers: Array<{
      startLine: number;
      startCharacter: number;
      endLine: number;
      endCharacter: number;
      contents: string[];
    }>
  ) {
    expectDefined(hovers);
    expect(hovers).toHaveLength(expectedHovers.length);
    hovers.forEach((hover, hoverIndex) => {
      const {
        startLine,
        startCharacter,
        endLine,
        endCharacter,
        contents,
      } = expectedHovers[hoverIndex];

      expectDefined(hover.range);
      expectRange(
        hover.range,
        startLine,
        startCharacter,
        endLine,
        endCharacter
      );
      expect(hover.contents).toHaveLength(contents.length);
      hover.contents.forEach((content, contentIndex) => {
        expect(normalizeMarkedString(content)).toBe(
          marked(contents[contentIndex])
        );
      });
    });
  }
});
