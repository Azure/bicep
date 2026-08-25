// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { editor } from "monaco-editor";
import React from "react";
import { CodeEditor } from "./CodeEditor";

interface JsonEditorProps {
  content: string;
}

const editorOptions: editor.IStandaloneEditorConstructionOptions = {
  ariaLabel: "Generated ARM template editor",
  language: "json",
  scrollBeyondLastLine: false,
  automaticLayout: true,
  minimap: {
    enabled: false,
  },
  readOnly: true,
};

export const JsonEditor: React.FC<JsonEditorProps> = ({ content }) => {
  return <CodeEditor options={editorOptions} initialContent={content} />;
};