import React from 'react';
import MonacoEditor from 'react-monaco-editor';

interface JsonEditorProps {
  content: string;
}

export const JsonEditor : React.FC<JsonEditorProps> = props=> {
  const options = {
    automaticLayout: true,
    minimap: {
      enabled: false,
    },
    readOnly: true,
  };

  return <MonacoEditor
    language="json"
    theme="vs-dark"
    value={props.content}
    options={options}
  />
};