// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monaco from 'monaco-editor';
import React, { createRef, useEffect, useState } from 'react';

interface JsonEditorProps {
  content: string;
}

const editorOptions: monaco.editor.IStandaloneEditorConstructionOptions = {
  language: 'json',
  theme: 'vs-dark',
  scrollBeyondLastLine: false,
  automaticLayout: true,
  minimap: {
    enabled: false,
  },
  readOnly: true,
};

export const JsonEditor: React.FC<JsonEditorProps> = ({ content }) => {
  const editorRef = createRef<HTMLDivElement>();
  const [editor, setEditor] = useState<monaco.editor.IStandaloneCodeEditor>();

  useEffect(() => {
    const editor = monaco.editor.create(editorRef.current, editorOptions);

    setEditor(editor);
  }, []);

  useEffect(() => {
    editor?.setValue(content);
  }, [content, editor]);

  return <div ref={editorRef} style={{height: '100%', width: '100%'}} />;
};