// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as vscode from "vscode";

import { expectDefined, expectRange } from "../utils/assert";
import { retryWhile, sleep } from "../utils/time";
import { executeCloseAllEditors, executeHoverProvider } from "./commands";
import { readExampleFile } from "./examples";

describe("hover", (): void => {
  let document: vscode.TextDocument;

  beforeAll(async () => {
    const content = readExampleFile("101", "vm-simple-linux");
    document = await vscode.workspace.openTextDocument({
      language: "bicep",
      content,
    });

    await vscode.window.showTextDocument(document);

    // Give the language server sometime to finish compilation. If this is the first test
    // to run it may take long for the compilation to complete because JIT is not "warmed up".
    await sleep(2000);
  });

  afterAll(async () => {
    await executeCloseAllEditors();
  });

  it("should reveal type signature when hovering over a parameter name", async () => {
    const hovers = await executeHoverProviderCommandWithRetry(
      document.uri,
      new vscode.Position(1, 7)
    );

    expectHovers(hovers, {
      startLine: 1,
      startCharacter: 6,
      endLine: 1,
      endCharacter: 12,
      contents: [codeblock("param vmName: string")],
    });
  });

  it("should reveal type signature when hovering over a variable name", async () => {
    const hovers = await executeHoverProviderCommandWithRetry(
      document.uri,
      new vscode.Position(50, 10)
    );

    expectHovers(hovers, {
      startLine: 50,
      startCharacter: 4,
      endLine: 50,
      endCharacter: 22,
      contents: [codeblock("var linuxConfiguration: object")],
    });
  });

  it("should reveal type signature when hovering over a resource symbolic name", async () => {
    const hovers = await executeHoverProviderCommandWithRetry(
      document.uri,
      new vscode.Position(108, 10)
    );

    expectHovers(hovers, {
      startLine: 108,
      startCharacter: 9,
      endLine: 108,
      endCharacter: 13,
      contents: [
        codeblockWithDescription(
          "resource vnet 'Microsoft.Network/virtualNetworks@2020-06-01'",
          "[View Type Documentation](https://docs.microsoft.com/azure/templates/microsoft.network/virtualnetworks?tabs=bicep)"
        ),
      ],
    });
  });

  it("should reveal type signature when hovering over an output name", async () => {
    const hovers = await executeHoverProviderCommandWithRetry(
      document.uri,
      new vscode.Position(183, 14)
    );

    expectHovers(hovers, {
      startLine: 183,
      startCharacter: 7,
      endLine: 183,
      endCharacter: 28,
      contents: [codeblock("output administratorUsername: string")],
    });
  });

  it("should reveal type signature when hovering over a function name", async () => {
    const hovers = await executeHoverProviderCommandWithRetry(
      document.uri,
      new vscode.Position(18, 60)
    );

    expectHovers(hovers, {
      startLine: 18,
      startCharacter: 55,
      endLine: 18,
      endCharacter: 67,
      contents: [
        codeblockWithDescription(
          "function uniqueString(... : string): string",
          "Creates a deterministic hash string based on the values provided as parameters. The returned value is 13 characters long."
        ),
      ],
    });
  });

  function executeHoverProviderCommandWithRetry(
    documentUri: vscode.Uri,
    position: vscode.Position
  ) {
    return retryWhile(
      async () => await executeHoverProvider(documentUri, position),
      (hovers) => hovers === undefined || hovers.length === 0
    );
  }

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
      const { startLine, startCharacter, endLine, endCharacter, contents } =
        expectedHovers[hoverIndex];

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
        expect(normalizeMarkedString(content)).toBe(contents[contentIndex]);
      });
    });
  }

  function normalizeMarkedString(
    content: vscode.MarkedString | vscode.MarkdownString
  ): string {
    return typeof content === "string" ? content : content.value;
  }

  function codeblock(rawString: string): string {
    return "```bicep\n" + rawString + "\n```\n";
  }

  function codeblockWithDescription(
    rawString: string,
    description: string
  ): string {
    return `${codeblock(rawString)}${description}\n`;
  }
});
