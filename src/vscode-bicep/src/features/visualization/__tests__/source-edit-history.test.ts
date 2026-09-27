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

describe("source edit history", () => {
  it("undoes and redoes a resource insertion at exactly its original offset", () => {
    const { document, state, apply } = makeDocument("start-end");
    const history = new SourceEditHistory();
    const prepared = history.prepareInsertion(document, insertionEdit(document, 6, "resource"));
    apply(6, 6, "resource");
    expect(history.recordInsertion(document, "create-1", prepared)).toBe(0);

    const undo = history.planReplay(document, "create-1", "undo");
    expect(undo).toMatchObject({ startOffset: 6, endOffset: 14, newText: "" });
    apply(undo.startOffset, undo.endOffset, undo.newText);
    history.commitReplay(document, undo);
    expect(state.text).toBe("start-end");

    const redo = history.planReplay(document, "create-1", "redo");
    expect(redo).toMatchObject({ startOffset: 6, endOffset: 6, newText: "resource" });
    apply(redo.startOffset, redo.endOffset, redo.newText);
    history.commitReplay(document, redo);
    expect(state.text).toBe("start-resourceend");
  });

  it("replays the document's normalized CRLF text, not the unnormalized proposal", () => {
    const { document, state, apply } = makeDocument("base\r\n");
    const history = new SourceEditHistory();
    const prepared = history.prepareInsertion(document, insertionEdit(document, 0, "\nresource\n"));
    apply(0, 0, "\r\nresource\r\n");
    history.recordInsertion(document, "create-1", prepared);

    const undo = history.planReplay(document, "create-1", "undo");
    expect(undo.endOffset).toBe("\r\nresource\r\n".length);
    apply(undo.startOffset, undo.endOffset, undo.newText);
    history.commitReplay(document, undo);
    expect(state.text).toBe("base\r\n");

    const redo = history.planReplay(document, "create-1", "redo");
    expect(redo.newText).toBe("\r\nresource\r\n");
  });

  it("replays multiple designer creations in stack order despite newer document versions", () => {
    const { document, state, apply } = makeDocument("");
    const history = new SourceEditHistory();
    const first = history.prepareInsertion(document, insertionEdit(document, 0, "first"));
    apply(0, 0, "first");
    history.recordInsertion(document, "first", first);

    const second = history.prepareInsertion(document, insertionEdit(document, 5, "second"));
    apply(5, 5, "second");
    history.recordInsertion(document, "second", second);

    const undoSecond = history.planReplay(document, "second", "undo");
    apply(undoSecond.startOffset, undoSecond.endOffset, undoSecond.newText);
    history.commitReplay(document, undoSecond);
    const undoFirst = history.planReplay(document, "first", "undo");
    apply(undoFirst.startOffset, undoFirst.endOffset, undoFirst.newText);
    history.commitReplay(document, undoFirst);
    expect(state.text).toBe("");

    const redoFirst = history.planReplay(document, "first", "redo");
    apply(redoFirst.startOffset, redoFirst.endOffset, redoFirst.newText);
    history.commitReplay(document, redoFirst);
    const redoSecond = history.planReplay(document, "second", "redo");
    apply(redoSecond.startOffset, redoSecond.endOffset, redoSecond.newText);
    history.commitReplay(document, redoSecond);
    expect(state.text).toBe("firstsecond");
  });

  it("fails closed if the file changes outside the designer", () => {
    const { document, apply } = makeDocument("");
    const history = new SourceEditHistory();
    const prepared = history.prepareInsertion(document, insertionEdit(document, 0, "created"));
    apply(0, 0, "created");
    history.recordInsertion(document, "create-1", prepared);
    apply(0, 0, "outside");

    expect(() => history.planReplay(document, "create-1", "undo")).toThrow(SourceEditHistoryConflict);
    expect(history.epoch).toBe(1);
    expect(() => history.planReplay(document, "create-1", "undo")).toThrow(SourceEditHistoryConflict);
  });

  it("invalidates earlier source steps when a new creation follows an external edit", () => {
    const { document, apply } = makeDocument("");
    const history = new SourceEditHistory();
    const first = history.prepareInsertion(document, insertionEdit(document, 0, "first"));
    apply(0, 0, "first");
    history.recordInsertion(document, "first", first);
    apply(0, 0, "outside");

    const second = history.prepareInsertion(document, insertionEdit(document, 12, "second"));
    apply(12, 12, "second");
    expect(history.recordInsertion(document, "second", second)).toBe(1);
    expect(() => history.planReplay(document, "first", "undo")).toThrow(SourceEditHistoryConflict);
    expect(history.planReplay(document, "second", "undo").newText).toBe("");
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

  it("invalidates history when replay produces unexpected text", () => {
    const { document, apply } = makeDocument("");
    const history = new SourceEditHistory();
    const prepared = history.prepareInsertion(document, insertionEdit(document, 0, "created"));
    apply(0, 0, "created");
    history.recordInsertion(document, "create-1", prepared);
    const undo = history.planReplay(document, "create-1", "undo");

    apply(undo.startOffset, undo.endOffset, "different");
    expect(() => history.commitReplay(document, undo)).toThrow(SourceEditHistoryConflict);
    expect(history.epoch).toBe(1);
  });
});
