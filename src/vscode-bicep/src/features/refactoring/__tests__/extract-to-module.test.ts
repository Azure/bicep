// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { Position, Range, TextEditor, TextLine } from "vscode";

import { Uri, window, workspace } from "vscode";
import { LanguageClient } from "vscode-languageclient/node";
import { ExtractToModuleCommand } from "../extract-to-module";
import { ExtractToModuleResult } from "../protocol";

vi.mock("vscode", () => ({
  commands: {
    executeCommand: vi.fn(),
  },
  Uri: {
    file: vi.fn((filePath: string) => ({
      fsPath: filePath,
      toString: () => filePath,
    })),
  },
  window: {
    showErrorMessage: vi.fn(),
    showInputBox: vi.fn(),
    showWarningMessage: vi.fn(),
  },
  workspace: {
    applyEdit: vi.fn(),
    fs: {
      createDirectory: vi.fn(),
      stat: vi.fn(),
      writeFile: vi.fn(),
    },
  },
}));

const mockClient = (result: ExtractToModuleResult): LanguageClient => {
  return {
    sendRequest: vi.fn().mockResolvedValue(result),
    code2ProtocolConverter: {
      asTextDocumentIdentifier: vi.fn().mockReturnValue({ uri: "doc" }),
      asRange: vi.fn().mockReturnValue({ start: { line: 0, character: 0 }, end: { line: 0, character: 1 } }),
    },
    protocol2CodeConverter: {
      asWorkspaceEdit: vi.fn().mockReturnValue({}),
      asPosition: vi.fn().mockReturnValue({ line: 0, character: 0 }),
    },
  } as unknown as LanguageClient;
};

describe("ExtractToModuleCommand", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(workspace.fs.stat).mockRejectedValue(new Error("missing"));
  });

  it("sends request and writes module", async () => {
    const response: ExtractToModuleResult = {
      replacementRange: { start: { line: 0, character: 0 }, end: { line: 0, character: 0 } },
      replacementText: "module mod './mod.bicep' = {}\n",
      moduleFileContents: "param p string\n",
      renamePosition: { line: 0, character: 7 },
    };

    const client = mockClient(response);
    const command = new ExtractToModuleCommand(client);

    const documentUri = Uri.file("/tmp/main.bicep");
    window.activeTextEditor = {
      document: {
        uri: documentUri,
        languageId: "bicep",
        fileName: "",
        isUntitled: false,
        encoding: "",
        version: 0,
        isDirty: false,
        isClosed: false,
        save: function (): Thenable<boolean> {
          throw new Error("Function not implemented.");
        },
        eol: 1 as unknown as import("vscode").EndOfLine,
        lineCount: 0,
        lineAt: function (): TextLine {
          throw new Error("Function not implemented.");
        },
        offsetAt: function (): number {
          throw new Error("Function not implemented.");
        },
        positionAt: function (): Position {
          throw new Error("Function not implemented.");
        },
        getText: function (): string {
          throw new Error("Function not implemented.");
        },
        getWordRangeAtPosition: function (): Range | undefined {
          throw new Error("Function not implemented.");
        },
        validateRange: function (): Range {
          throw new Error("Function not implemented.");
        },
        validatePosition: function (): Position {
          throw new Error("Function not implemented.");
        },
      },
      selection: {
        isEmpty: false,
        anchor: { line: 0, character: 0 } as Position,
        active: { line: 0, character: 0 } as Position,
        isReversed: false,
        start: { line: 0, character: 0 } as Position,
        end: { line: 0, character: 0 } as Position,
        isSingleLine: false,
        contains: function (): boolean {
          throw new Error("Function not implemented.");
        },
        isEqual: function (): boolean {
          throw new Error("Function not implemented.");
        },
        intersection: function (): Range | undefined {
          throw new Error("Function not implemented.");
        },
        union: function (): Range {
          throw new Error("Function not implemented.");
        },
        with: function (): Range {
          throw new Error("Function not implemented.");
        },
      },
    } as unknown as TextEditor;

    vi.mocked(window.showInputBox).mockResolvedValue("module.bicep");

    await command.execute(undefined);

    expect(client.sendRequest).toHaveBeenCalled();
    expect(workspace.fs.writeFile).toHaveBeenCalledOnce();
    expect(workspace.applyEdit).toHaveBeenCalledOnce();
  });
});
