// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from "monaco-editor";
import React, { useEffect, useRef } from "react";
import { DotnetInterop } from "../utils/interop";
import { useColorMode } from "../utils/colorModes";

interface Props {
  options: monaco.editor.IStandaloneEditorConstructionOptions;
  initialContent: string;
  onContentChange?: (model: monaco.editor.ITextModel, content: string) => void;
}

export const CodeEditor: React.FC<Props> = (props) => {
  const { options, initialContent, onContentChange } = props;
  const containerRef = useRef<HTMLDivElement>(null);
  const editorRef = useRef<monaco.editor.IStandaloneCodeEditor>(undefined);
  const modelRef = useRef<monaco.editor.ITextModel>(undefined);
  const onContentChangeRef = useRef(onContentChange);
  const initialContentRef = useRef(initialContent);
  const initialOptionsRef = useRef(options);
  const colorMode = useColorMode();

  useEffect(() => {
    onContentChangeRef.current = onContentChange;
  }, [onContentChange]);

  useEffect(() => {
    const container = containerRef.current;
    if (!container) {
      throw new Error("The Monaco editor container was not mounted.");
    }

    const editor = monaco.editor.create(container, {
      ...initialOptionsRef.current,
      theme: colorMode === "dark" ? "vs-dark" : "vs",
      value: initialContentRef.current,
    });
    const model = editor.getModel();
    if (!model) {
      editor.dispose();
      throw new Error("Monaco did not create an editor model.");
    }

    const contentChangeSubscription = editor.onDidChangeModelContent(() => {
      onContentChangeRef.current?.(model, model.getValue());
    });

    editorRef.current = editor;
    modelRef.current = model;
    onContentChangeRef.current?.(model, model.getValue());

    return () => {
      contentChangeSubscription.dispose();
      editor.dispose();
      if (!model.isDisposed()) {
        model.dispose();
      }
      editorRef.current = undefined;
      modelRef.current = undefined;
    };
  }, []);

  useEffect(() => {
    const model = modelRef.current;
    if (model && model.getValue() !== initialContent) {
      model.setValue(initialContent);
    }
  }, [initialContent]);

  useEffect(() => {
    editorRef.current?.updateOptions({
      ...options,
      theme: colorMode === "dark" ? "vs-dark" : "vs",
    });
  }, [colorMode, options]);

  return <div ref={containerRef} style={{ height: "100%", width: "100%" }} />;
};

export function registerBicep(
  interop: DotnetInterop,
  getSourcePath: () => string | undefined,
) {
  monaco.languages.register({
    id: "bicep",
    extensions: [".bicep"],
    aliases: ["bicep"],
  });

  const semanticTokensRegistration =
    monaco.languages.registerDocumentSemanticTokensProvider("bicep", {
      getLegend: () => interop.getSemanticTokensLegend(),
      provideDocumentSemanticTokens: async (model) =>
        await interop.getSemanticTokens(model.getValue(), getSourcePath()),
      releaseDocumentSemanticTokens: () => {
        return;
      },
    });

  return {
    dispose: () => {
      semanticTokensRegistration.dispose();
    },
  };
}
