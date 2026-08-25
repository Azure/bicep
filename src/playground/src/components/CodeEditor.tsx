// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from "monaco-editor";
import { forwardRef, useEffect, useImperativeHandle, useRef } from "react";
import {
  CompilerRequestSupersededError,
  DotnetInterop,
} from "../compiler/compiler-client";
import { useColorMode } from "../theme/color-mode";

let editorThemesDefined = false;

interface Props {
  options: monaco.editor.IStandaloneEditorConstructionOptions;
  initialContent: string;
  contentRevision?: number;
  onContentChange?: (model: monaco.editor.ITextModel, content: string) => void;
}

export interface CodeEditorHandle {
  focusAt(lineNumber: number, column: number): void;
}

export const CodeEditor = forwardRef<CodeEditorHandle, Props>((props, ref) => {
  const { options, initialContent, contentRevision, onContentChange } = props;
  const containerRef = useRef<HTMLDivElement>(null);
  const editorRef = useRef<monaco.editor.IStandaloneCodeEditor>(undefined);
  const modelRef = useRef<monaco.editor.ITextModel>(undefined);
  const onContentChangeRef = useRef(onContentChange);
  const initialContentRef = useRef(initialContent);
  const initialOptionsRef = useRef(options);
  const colorMode = useColorMode();

  useImperativeHandle(
    ref,
    () => ({
      focusAt: (lineNumber, column) => {
        const editor = editorRef.current;
        if (!editor) {
          return;
        }

        editor.setPosition({ lineNumber, column });
        editor.revealPositionInCenter({ lineNumber, column });
        editor.focus();
      },
    }),
    [],
  );

  useEffect(() => {
    onContentChangeRef.current = onContentChange;
  }, [onContentChange]);

  useEffect(() => {
    const container = containerRef.current;
    if (!container) {
      throw new Error("The Monaco editor container was not mounted.");
    }

    defineEditorThemes();
    const editor = monaco.editor.create(container, {
      ...initialOptionsRef.current,
      theme: getEditorTheme(colorMode),
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
  }, [contentRevision, initialContent]);

  useEffect(() => {
    monaco.editor.setTheme(getEditorTheme(colorMode));
    editorRef.current?.updateOptions(options);
  }, [colorMode, options]);

  return (
    <div
      ref={containerRef}
      role="region"
      aria-label={options.ariaLabel}
      style={{ height: "100%", width: "100%" }}
    />
  );
});

function defineEditorThemes(): void {
  if (editorThemesDefined) {
    return;
  }

  monaco.editor.defineTheme("bicep-light", {
    base: "vs",
    inherit: true,
    rules: [],
    colors: {
      "editor.background": "#FFFFFF",
      "editorGutter.background": "#FFFFFF",
      "editor.lineHighlightBackground": "#0F172A0A",
      "editorStickyScroll.background": "#FFFFFF",
    },
  });
  monaco.editor.defineTheme("bicep-dark", {
    base: "vs-dark",
    inherit: true,
    rules: [],
    colors: {
      "editor.background": "#101218",
      "editorGutter.background": "#101218",
      "editor.lineHighlightBackground": "#FFFFFF0D",
      "editorStickyScroll.background": "#101218",
    },
  });
  editorThemesDefined = true;
}

function getEditorTheme(colorMode: "dark" | "light"): string {
  return colorMode === "dark" ? "bicep-dark" : "bicep-light";
}

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
      provideDocumentSemanticTokens: async (model) => {
        try {
          return await interop.getSemanticTokens(
            model.getValue(),
            getSourcePath(),
          );
        } catch (error) {
          if (error instanceof CompilerRequestSupersededError) {
            return null;
          }

          throw error;
        }
      },
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
