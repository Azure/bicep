// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import * as monacoEditor from 'monaco-editor';
import React, { useRef } from 'react';
import MonacoEditor from 'react-monaco-editor';

interface JsonEditorProps {
  content: string;
}

export const JsonEditor : React.FC<JsonEditorProps> = props=> {
  const options: monacoEditor.editor.IStandaloneEditorConstructionOptions = {
    scrollBeyondLastLine: false,
    automaticLayout: true,
    minimap: {
      enabled: false,
    },
    readOnly: true,
  };
  
  const monacoRef = useRef<MonacoEditor>();
  
  // clear the selection after rendering completes
  setTimeout(() => monacoRef.current.editor.setSelection({startColumn: 1, startLineNumber: 1, endColumn: 1, endLineNumber: 1}), 0);

  return <MonacoEditor
    ref={monacoRef}
    language="json"
    theme="vs-dark"
    value={props.content}
    options={options}
  />
};