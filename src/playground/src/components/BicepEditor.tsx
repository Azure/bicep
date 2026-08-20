// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor } from "monaco-editor";
import React, { useCallback, useEffect, useRef } from "react";
import { DotnetInterop } from "../utils/interop";
import { CodeEditor } from "./CodeEditor";

const compilationDebounceMs = 200;

interface Props {
  interop: DotnetInterop;
  initialContent: string;
  sourcePath?: string;
  onBicepChange: (bicepContent: string) => void;
  onJsonChange: (jsonContent: string) => void;
  onCompilationError: (message: string | undefined) => void;
}

const editorOptions: editor.IStandaloneEditorConstructionOptions = {
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
  } = props;
  const compilationRequestIdRef = useRef(0);
  const compilationTimeoutRef = useRef<number>(undefined);

  const handleContentChange = useCallback(
    (model: editor.ITextModel, content: string) => {
      onBicepChange(content);
      const requestId = ++compilationRequestIdRef.current;

      if (compilationTimeoutRef.current !== undefined) {
        window.clearTimeout(compilationTimeoutRef.current);
      }

      compilationTimeoutRef.current = window.setTimeout(async () => {
        const modelVersion = model.getVersionId();

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
            return;
          }

          onCompilationError(undefined);
          onJsonChange(template);
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
        }
      }, compilationDebounceMs);
    },
    [interop, onBicepChange, onCompilationError, onJsonChange, sourcePath],
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
