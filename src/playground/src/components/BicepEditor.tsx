// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor } from "monaco-editor";
import React, { useCallback, useEffect, useRef } from "react";
import { DotnetInterop } from "../compiler/compiler-client";
import { CodeEditor } from "./CodeEditor";

const compilationDebounceMs = 200;

export type CompilationStatus =
  | "pending"
  | "compiling"
  | "upToDate"
  | "failed";

interface Props {
  interop: DotnetInterop;
  initialContent: string;
  sourcePath?: string;
  onBicepChange: (bicepContent: string) => void;
  onJsonChange: (jsonContent: string) => void;
  onCompilationError: (message: string | undefined) => void;
  onCompilationStatusChange: (status: CompilationStatus) => void;
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

export const BicepEditor: React.FC<Props> = (props) => {
  const {
    interop,
    initialContent,
    sourcePath,
    onBicepChange,
    onJsonChange,
    onCompilationError,
    onCompilationStatusChange,
  } = props;
  const compilationRequestIdRef = useRef(0);
  const compilationTimeoutRef = useRef<number>(undefined);

  const handleContentChange = useCallback(
    (model: editor.ITextModel, content: string) => {
      onBicepChange(content);
      onCompilationError(undefined);
      onCompilationStatusChange("pending");
      const requestId = ++compilationRequestIdRef.current;

      if (compilationTimeoutRef.current !== undefined) {
        window.clearTimeout(compilationTimeoutRef.current);
      }

      compilationTimeoutRef.current = window.setTimeout(async () => {
        const modelVersion = model.getVersionId();
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

          if (error) {
            onCompilationError(error);
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
      onCompilationStatusChange,
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
      options={editorOptions}
      initialContent={initialContent}
      onContentChange={handleContentChange}
    />
  );
};
