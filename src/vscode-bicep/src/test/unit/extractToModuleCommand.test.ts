// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import { commands, Uri, window, workspace } from "vscode";
import type { Position, Range, TextEditor, TextLine } from "vscode";
import { ExtractToModuleCommand } from "../../commands/extractToModule";
import { LanguageClient } from "vscode-languageclient/node";
import { ExtractToModuleResult } from "../../language";

const mockClient = (result: ExtractToModuleResult): LanguageClient => {
  return {
    sendRequest: jest.fn().mockResolvedValue(result),
    code2ProtocolConverter: {
      asTextDocumentIdentifier: jest.fn().mockReturnValue({ uri: "doc" }),
      asRange: jest.fn().mockReturnValue({ start: { line: 0, character: 0 }, end: { line: 0, character: 1 } }),
    },
    protocol2CodeConverter: {
      asWorkspaceEdit: jest.fn().mockReturnValue({}),
      asPosition: jest.fn().mockReturnValue({ line: 0, character: 0 }),
    },
  } as unknown as LanguageClient;
};

describe("ExtractToModuleCommand", () => {
  beforeEach(() => {
    window.showInputBox = jest.fn();
    window.showErrorMessage = jest.fn();
    window.showWarningMessage = jest.fn();
    workspace.applyEdit = jest.fn();
    (workspace as unknown as { fs: { writeFile: jest.Mock; stat: jest.Mock } }).fs = {
      writeFile: jest.fn().mockResolvedValue(undefined),
      stat: jest.fn().mockRejectedValue(new Error("missing")),
    };
    commands.executeCommand = jest.fn();
    (Uri as unknown as { file: jest.Mock }).file = jest.fn((p: string) => ({
      fsPath: p,
      toString: () => p,
    } as unknown as Uri));
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

    const documentUri = Uri.file("/workspaces/bicep/main.bicep");
    window.activeTextEditor = {
      document: {
        uri: documentUri, languageId: "bicep",
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
        }
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
        }
      },
    } as unknown as TextEditor;

    window.showInputBox = jest.fn().mockResolvedValue("module.bicep");

    await command.execute({ telemetry: { properties: {} } } as never, undefined);

    expect(client.sendRequest).toHaveBeenCalled();
    expect((workspace.fs.writeFile as jest.Mock).mock.calls).toHaveLength(1);
    expect((workspace.applyEdit as jest.Mock).mock.calls).toHaveLength(1);
  });
});
