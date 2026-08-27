// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor, MarkerSeverity } from "monaco-editor/editor/editor.api";
import { forwardRef, useCallback, useEffect, useRef } from "react";
import { DotnetInterop } from "../compiler/compiler-client";
import { CodeEditor, CodeEditorHandle } from "./CodeEditor";

const compilationDebounceMs = 200;

export type CompilationStatus =
  | "pending"
  | "compiling"
  | "upToDate"
  | "failed";

interface Props {
  interop: DotnetInterop;
  initialContent: string;
  contentRevision: number;
  sourcePath?: string;
  onBicepChange: (bicepContent: string) => void;
  onJsonChange: (jsonContent: string) => void;
  onDiagnosticsChange: (diagnostics: editor.IMarkerData[]) => void;
  onCompilationError: (message: string | undefined) => void;
  onCompilationStatusChange: (status: CompilationStatus) => void;
  onCompilationDurationChange: (durationMs: number | undefined) => void;
}

const editorOptions: editor.IStandaloneEditorConstructionOptions = {
  ariaLabel: "Bicep editor",
  language: "bicep",
  scrollBeyondLastLine: false,
  automaticLayout: true,
  minimap: {
    enabled: false,
  },
  insertSpaces: true,
  tabSize: 2,
  suggestSelection: "first",
  suggest: {
    snippetsPreventQuickSuggestions: false,
    showWords: false,
  },
  "semanticHighlighting.enabled": true,
};

export const BicepEditor = forwardRef<CodeEditorHandle, Props>((props, ref) => {
  const {
    interop,
    initialContent,
    contentRevision,
    sourcePath,
    onBicepChange,
    onJsonChange,
    onDiagnosticsChange,
    onCompilationError,
    onCompilationStatusChange,
    onCompilationDurationChange,
  } = props;
  const compilationRequestIdRef = useRef(0);
  const compilationTimeoutRef = useRef<number>(undefined);

  const handleContentChange = useCallback(
    (model: editor.ITextModel, content: string) => {
      onBicepChange(content);
      onDiagnosticsChange([]);
      onCompilationError(undefined);
      onCompilationDurationChange(undefined);
      onCompilationStatusChange("pending");
      const requestId = ++compilationRequestIdRef.current;

      if (compilationTimeoutRef.current !== undefined) {
        window.clearTimeout(compilationTimeoutRef.current);
      }

      compilationTimeoutRef.current = window.setTimeout(async () => {
        const modelVersion = model.getVersionId();
        const startedAt = performance.now();
        onCompilationStatusChange("compiling");

        try {
          const { template, diagnostics, error } =
            await interop.compileAndEmitDiagnostics(content, sourcePath);

          if (
            requestId !== compilationRequestIdRef.current ||
            model.isDisposed() ||
            model.getVersionId() !== modelVersion
          ) {
            return;
          }

          editor.setModelMarkers(model, "bicep", diagnostics);
          onDiagnosticsChange(diagnostics);
          onCompilationDurationChange(performance.now() - startedAt);

          if (
            error ||
            diagnostics.some(
              (diagnostic) => diagnostic.severity === MarkerSeverity.Error,
            )
          ) {
            onCompilationError(error ?? undefined);
            onCompilationStatusChange("failed");
            return;
          }

          onCompilationError(undefined);
          onJsonChange(template);
          onCompilationStatusChange("upToDate");
        } catch (error) {
          if (
            requestId !== compilationRequestIdRef.current ||
            model.isDisposed() ||
            model.getVersionId() !== modelVersion
          ) {
            return;
          }

          editor.setModelMarkers(model, "bicep", []);
          onDiagnosticsChange([]);
          onCompilationDurationChange(performance.now() - startedAt);
          onCompilationError(
            error instanceof Error
              ? error.message
              : "Bicep compilation failed.",
          );
          onCompilationStatusChange("failed");
        }
      }, compilationDebounceMs);
    },
    [
      interop,
      onBicepChange,
      onCompilationError,
      onCompilationDurationChange,
      onCompilationStatusChange,
      onDiagnosticsChange,
      onJsonChange,
      sourcePath,
    ],
  );

  useEffect(() => {
    return () => {
      ++compilationRequestIdRef.current;

      if (compilationTimeoutRef.current !== undefined) {
        window.clearTimeout(compilationTimeoutRef.current);
      }
    };
  }, []);

  return (
    <CodeEditor
      ref={ref}
      options={editorOptions}
      initialContent={initialContent}
      contentRevision={contentRevision}
      onContentChange={handleContentChange}
    />
  );
});
