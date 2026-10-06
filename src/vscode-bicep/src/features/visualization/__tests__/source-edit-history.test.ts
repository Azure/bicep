// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { TextDocument, WorkspaceEdit } from "vscode";

import { SourceEditHistory, SourceEditHistoryConflict } from "../source-edit-history";

function makeDocument(initialText = "param location string\n") {
  const state = { text: initialText, version: 1 };
  const uri = { toString: () => "file:///main.bicep" };
  const document = {
    uri,
    get version() {
      return state.version;
    },
    getText: () => state.text,
    offsetAt: (position: { line: number; character: number }) => position.character,
  } as TextDocument;
  const apply = (start: number, end: number, text: string) => {
    state.text = state.text.slice(0, start) + text + state.text.slice(end);
    state.version++;
  };
  return { document, state, apply };
}

function insertionEdit(document: TextDocument, offset: number, newText: string): WorkspaceEdit {
  return {
    entries: () => [[document.uri, [{ range: { start: { line: 0, character: offset }, isEmpty: true }, newText }]]],
  } as unknown as WorkspaceEdit;
}

function createTracked(initialText: string, offset: number, text: string, appliedText = text) {
  const { document, state, apply } = makeDocument(initialText);
  const history = new SourceEditHistory();
  const pending = history.prepareInsertion(document, insertionEdit(document, offset, text));
  apply(offset, offset, appliedText);
  history.recordInsertion(document, "create-1", "storage", pending);
  return { document, state, apply, history };
}

describe("source edit history", () => {
  it("tracks what a creation inserted so the language server can find it", () => {
    const { history } = createTracked("base", 4, "\nresource");

    expect(
      history.getReplayQueries([
        { operationId: "create-1", direction: "undo" },
        { operationId: "unknown", direction: "undo" },
      ]),
    ).toEqual([{ operationId: "create-1", direction: "undo", nodeId: "storage", insertedText: "\nresource" }]);
    expect(history.hasOperation("create-1")).toBe(true);
  });

  it("tracks the document's normalized CRLF text, not the unnormalized proposal", () => {
    const { history } = createTracked("base\r\n", 0, "\nresource\n", "\r\nresource\r\n");

    expect(history.getReplayQueries([{ operationId: "create-1", direction: "undo" }])[0]?.insertedText).toBe(
      "\r\nresource\r\n",
    );
  });

  it("refuses to track a creation that left the file in an unexpected state", () => {
    const { document, apply } = makeDocument("base");
    const history = new SourceEditHistory();
    const pending = history.prepareInsertion(document, insertionEdit(document, 4, "\nresource"));
    apply(0, 0, "outside");
    apply(11, 11, "\nresource");

    expect(() => history.recordInsertion(document, "create-1", "storage", pending)).toThrow(SourceEditHistoryConflict);
    expect(history.hasOperation("create-1")).toBe(false);
  });

  it("verifies applied undo and redo edits and keeps the text redo inserted", () => {
    const { document, state, apply, history } = createTracked("base", 4, "\nresource");

    const undo = history.prepareEdit(document, 4, 13, "");
    apply(4, 13, "");
    history.commitReplay(document, "create-1", "undo", undo);
    expect(state.text).toBe("base");

    const redo = history.prepareEdit(document, 0, 0, "\nresource");
    apply(0, 0, "\r\nresource");
    history.commitReplay(document, "create-1", "redo", redo);
    expect(history.getReplayQueries([{ operationId: "create-1", direction: "undo" }])[0]?.insertedText).toBe(
      "\r\nresource",
    );
  });

  it("stops tracking a creation whose replay left the file in an unexpected state", () => {
    const { document, apply, history } = createTracked("base", 4, "\nresource");

    const undo = history.prepareEdit(document, 4, 13, "");
    apply(4, 13, "different");

    expect(() => history.commitReplay(document, "create-1", "undo", undo)).toThrow(SourceEditHistoryConflict);
    expect(history.getReplayQueries([{ operationId: "create-1", direction: "redo" }])).toEqual([]);
  });

  it("rejects non-insertion and multi-file edits before they change the source", () => {
    const { document } = makeDocument("source");
    const history = new SourceEditHistory();
    const invalid = {
      entries: () => [
        [document.uri, [{ range: { start: { line: 0, character: 0 }, isEmpty: false }, newText: "changed" }]],
      ],
    } as unknown as WorkspaceEdit;
    expect(() => history.prepareInsertion(document, invalid)).toThrow(/must insert text/);

    const multiFile = {
      entries: () => [...insertionEdit(document, 0, "new").entries(), [{ toString: () => "file:///other.bicep" }, []]],
    } as unknown as WorkspaceEdit;
    expect(() => history.prepareInsertion(document, multiFile)).toThrow(/opened Bicep file only/);
  });
});
