// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import React, {
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import {
  editor,
  KeyCode,
  type IKeyboardEvent,
} from "monaco-editor/editor/editor.api";
import type { DotnetInterop } from "../compiler/compiler-client";
import type { ConsoleResult } from "../compiler/compiler-protocol";
import { useColorMode } from "../theme/color-mode";
import { CodeEditor, type CodeEditorHandle } from "./CodeEditor";

const inputOptions: editor.IStandaloneEditorConstructionOptions = {
  ariaLabel: "Bicep console input",
  language: "bicep",
  automaticLayout: true,
  minimap: { enabled: false },
  lineNumbers: "off",
  glyphMargin: false,
  folding: false,
  lineDecorationsWidth: 0,
  overviewRulerLanes: 0,
  renderLineHighlight: "none",
  scrollBeyondLastLine: false,
  wordWrap: "on",
  fontFamily: 'Consolas, "Courier New", Courier, monospace',
  fontSize: 13,
  lineHeight: 20,
  padding: { top: 4, bottom: 4 },
  tabSize: 4,
  tabFocusMode: true,
  autoIndent: "none",
  autoClosingBrackets: "never",
  autoClosingQuotes: "never",
  autoSurround: "never",
  quickSuggestions: false,
  suggestOnTriggerCharacters: false,
  wordBasedSuggestions: "off",
  "semanticHighlighting.enabled": false,
  placeholder: "Try: toUpper('hello bicep')",
};

type Entry = {
  input: string;
  result?: ConsoleResult;
  error?: string;
};

interface Props {
  interop: DotnetInterop;
  open: boolean;
  initialCommands?: readonly string[];
  history: readonly string[];
  setHistory: React.Dispatch<React.SetStateAction<readonly string[]>>;
  busy: boolean;
  setBusy: React.Dispatch<React.SetStateAction<boolean>>;
}

export const ConsolePanel: React.FC<Props> = ({
  interop,
  open,
  initialCommands,
  history,
  setHistory,
  busy,
  setBusy,
}) => {
  const [input, setInput] = useState("");
  const [entries, setEntries] = useState<Entry[]>([]);
  const [highlightError, setHighlightError] = useState<string>();
  const [replayError, setReplayError] = useState<string>();
  const inputRef = useRef<CodeEditorHandle>(null);
  const transcriptRef = useRef<HTMLDivElement>(null);
  const sessionId = useRef<string>(undefined);
  const running = useRef(false);
  const operationId = useRef(0);
  const visible = useRef(open);
  visible.current = open;
  const historyIndex = useRef(0);
  const draft = useRef("");
  const options = useMemo(() => ({ ...inputOptions, readOnly: busy }), [busy]);

  useEffect(
    () => () => {
      ++operationId.current;
    },
    [],
  );

  useEffect(() => {
    if (open) {
      inputRef.current?.focus();
    }
  }, [open]);

  useEffect(() => {
    const transcript = transcriptRef.current;
    if (transcript) {
      transcript.scrollTop = transcript.scrollHeight;
    }
  }, [entries, busy, open]);

  const finishOperation = useCallback(
    (operation: number) => {
      if (operationId.current === operation) {
        running.current = false;
        setBusy(false);
        if (visible.current) {
          inputRef.current?.focus();
        }
      }
    },
    [setBusy],
  );

  const resetSession = useCallback(
    async (operation: number) => {
      const newSession = await interop.resetConsole();
      if (operationId.current !== operation) {
        return false;
      }
      sessionId.current = newSession;
      setEntries([]);
      inputRef.current?.setValue("");
      historyIndex.current = 0;
      draft.current = "";
      setReplayError(undefined);
      setHistory([]);
      return true;
    },
    [interop, setHistory],
  );

  const evaluateCommand = useCallback(
    async (command: string, operation: number) => {
      if (operationId.current !== operation) {
        return false;
      }
      setHistory((previous) => [...previous, command]);
      ++historyIndex.current;
      draft.current = "";
      setEntries((previous) => [...previous, { input: command }]);

      try {
        if (sessionId.current === undefined) {
          const newSession = await interop.resetConsole();
          if (operationId.current !== operation) {
            return false;
          }
          sessionId.current = newSession;
        }
        const result = await interop.evaluateConsole(
          command,
          sessionId.current,
        );
        if (operationId.current !== operation) {
          return false;
        }
        setEntries((previous) => [
          ...previous.slice(0, -1),
          { input: command, result },
        ]);
        return true;
      } catch (error) {
        if (operationId.current === operation) {
          setEntries((previous) => [
            ...previous.slice(0, -1),
            {
              input: command,
              error:
                error instanceof Error
                  ? error.message
                  : "Console evaluation failed.",
            },
          ]);
        }
        return false;
      }
    },
    [interop, setHistory],
  );

  useEffect(() => {
    if (initialCommands === undefined) {
      return;
    }
    const operation = ++operationId.current;
    running.current = true;
    setBusy(true);
    void (async () => {
      try {
        if (!(await resetSession(operation))) {
          return;
        }
        for (const command of initialCommands) {
          if (!(await evaluateCommand(command, operation))) {
            if (operationId.current === operation) {
              setReplayError(
                "Shared console history replay stopped. The session was not fully restored.",
              );
            }
            return;
          }
        }
      } catch (error) {
        if (operationId.current === operation) {
          setReplayError(
            error instanceof Error
              ? `Shared console history could not be restored: ${error.message}`
              : "Shared console history could not be restored.",
          );
        }
      } finally {
        finishOperation(operation);
      }
    })();

    return () => {
      ++operationId.current;
    };
  }, [
    initialCommands,
    resetSession,
    evaluateCommand,
    finishOperation,
    setBusy,
  ]);

  async function submit() {
    if (running.current || !input.trim()) {
      return;
    }
    const command = input;
    historyIndex.current = history.length;
    const operation = ++operationId.current;
    running.current = true;
    setBusy(true);
    inputRef.current?.setValue("");
    try {
      await evaluateCommand(command, operation);
    } finally {
      finishOperation(operation);
    }
  }

  async function reset() {
    if (running.current) {
      return;
    }
    const operation = ++operationId.current;
    running.current = true;
    setBusy(true);
    try {
      await resetSession(operation);
    } catch (error) {
      if (operationId.current === operation) {
        setEntries((previous) => [
          ...previous,
          {
            input: "Reset session",
            error:
              error instanceof Error
                ? error.message
                : "Resetting the console failed.",
          },
        ]);
      }
    } finally {
      finishOperation(operation);
    }
  }

  function handleKeyDown(
    event: IKeyboardEvent,
    codeEditor: editor.IStandaloneCodeEditor,
  ) {
    if (event.browserEvent.isComposing || running.current) {
      return;
    }
    if (event.keyCode === KeyCode.Enter && !event.shiftKey) {
      event.preventDefault();
      event.stopPropagation();
      void submit();
      return;
    }

    const selection = codeEditor.getSelection();
    const model = codeEditor.getModel();
    if (
      !selection?.isEmpty() ||
      !model ||
      event.altKey ||
      event.ctrlKey ||
      event.metaKey ||
      event.shiftKey
    ) {
      return;
    }
    const backwards = event.keyCode === KeyCode.UpArrow;
    const forwards = event.keyCode === KeyCode.DownArrow;
    if (
      (!backwards && !forwards) ||
      (backwards && selection.startLineNumber !== 1) ||
      (forwards && selection.endLineNumber !== model.getLineCount())
    ) {
      return;
    }
    const next = historyIndex.current + (backwards ? -1 : 1);
    if (next < 0 || next > history.length) {
      return;
    }
    event.preventDefault();
    event.stopPropagation();
    if (historyIndex.current === history.length) {
      draft.current = input;
    }
    historyIndex.current = next;
    inputRef.current?.setValue(history[next] ?? draft.current);
  }

  return (
    <section
      id="console-panel"
      className="console-panel"
      aria-labelledby="console-title"
      hidden={!open}
    >
      <header className="problems-header">
        <h2 id="console-title" className="problems-title">
          Bicep console
        </h2>
        <span className="layout-spacer" />
        <button
          type="button"
          className="pane-action"
          disabled={busy}
          onClick={() => setEntries([])}
        >
          Clear output
        </button>
        <button
          type="button"
          className="pane-action"
          disabled={busy}
          onClick={() => void reset()}
        >
          Reset session
        </button>
      </header>
      <div
        className="console-transcript"
        ref={transcriptRef}
        role="log"
        aria-label="Console output"
        aria-live="polite"
        aria-relevant="additions text"
      >
        <p className="console-help">
          Evaluate Bicep expressions or declare var, type, and func definitions.
          This session is independent of the editor. Enter runs; Shift+Enter
          adds a line. Up/Down recalls history. Results are JSON. No Azure
          deployment is performed. Copy Link includes all submitted commands,
          even after clearing output. Do not share secrets.
        </p>
        {entries.map((entry, index) => (
          <div className="console-entry" key={index}>
            <pre className="console-input-echo">
              {"> "}
              <HighlightedCode
                content={entry.input}
                language="bicep"
                onError={setHighlightError}
              />
            </pre>
            {entry.result?.output !== null &&
              entry.result?.output !== undefined && (
                <pre>
                  <HighlightedCode
                    content={entry.result.output}
                    language="json"
                    onError={setHighlightError}
                  />
                </pre>
              )}
            {entry.result?.diagnostics.map((diagnostic, diagnosticIndex) => (
              <pre
                key={diagnosticIndex}
                className={
                  diagnostic.level === "Error"
                    ? "console-error"
                    : "console-warning"
                }
              >
                {diagnostic.level} {diagnostic.code}: {diagnostic.message}
              </pre>
            ))}
            {entry.error && <pre className="console-error">{entry.error}</pre>}
          </div>
        ))}
        {highlightError && (
          <div role="alert" className="console-error">
            {highlightError}
          </div>
        )}
        {replayError && (
          <div role="alert" className="console-error">
            {replayError}
          </div>
        )}
      </div>
      <form
        className="console-form"
        onSubmit={(event) => {
          event.preventDefault();
          void submit();
        }}
      >
        <div className="console-input">
          <CodeEditor
            ref={inputRef}
            options={options}
            initialContent=""
            onContentChange={(model) =>
              setInput(model.getValue(editor.EndOfLinePreference.LF))
            }
            onKeyDown={handleKeyDown}
          />
        </div>
        <button
          type="submit"
          className="pane-action"
          disabled={busy || !input.trim()}
        >
          {busy ? "Evaluating..." : "Run"}
        </button>
      </form>
    </section>
  );
};

const HighlightedCode = React.memo(function HighlightedCode({
  content,
  language,
  onError,
}: {
  content: string;
  language: "bicep" | "json";
  onError(message: string): void;
}) {
  const [highlighted, setHighlighted] = useState<{
    content: string;
    html: string;
  }>();
  const colorMode = useColorMode();

  useEffect(() => {
    let disposed = false;
    void editor.colorize(content, language, { tabSize: 4 }).then(
      (html) => {
        if (!disposed) {
          setHighlighted({ content, html });
        }
      },
      (error: unknown) => {
        if (!disposed) {
          onError(
            error instanceof Error
              ? `Console syntax highlighting failed: ${error.message}`
              : "Console syntax highlighting failed.",
          );
        }
      },
    );
    return () => {
      disposed = true;
    };
  }, [content, language, colorMode, onError]);

  if (highlighted?.content !== content) {
    return <>{content}</>;
  }

  // Monaco colorize escapes source text before adding its token markup.
  return <span dangerouslySetInnerHTML={{ __html: highlighted.html }} />;
});
