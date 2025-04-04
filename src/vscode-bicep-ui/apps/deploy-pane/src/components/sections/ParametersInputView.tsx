// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";
import type { ParamData, ParamDefinition, ParametersMetadata, TemplateMetadata } from "../../models";

import { VscodeButton, VscodeTextfield } from "@vscode-elements/react-elements";
import { ParamInputBox } from "../ParamInputBox";
import { FormSection } from "./FormSection";

interface ParametersInputViewProps {
  template?: TemplateMetadata;
  parameters?: ParametersMetadata;
  disabled: boolean;
  onValueChange: (name: string, data: ParamData) => void;
  onEnableEditing: () => void;
  onPickParametersFile: () => void;
}

export const ParametersInputView: FC<ParametersInputViewProps> = ({
  template,
  parameters,
  disabled,
  onValueChange,
  onEnableEditing,
  onPickParametersFile,
}) => {
  if (!template || !parameters) {
    return null;
  }

  const { parameterDefinitions } = template;
  const { sourceFilePath } = parameters;

  return (
    <FormSection title="Parameters">
      {sourceFilePath && (
        <VscodeTextfield value={sourceFilePath} disabled={true}>
          File Path
        </VscodeTextfield>
      )}
      {sourceFilePath && !sourceFilePath.endsWith(".bicepparam") && (
        <VscodeButton onClick={onEnableEditing}>Edit Parameters</VscodeButton>
      )}
      {!sourceFilePath && <VscodeButton onClick={onPickParametersFile}>Pick JSON Parameters File</VscodeButton>}
      {!sourceFilePath &&
        parameterDefinitions.map((definition) => (
          <ParamInputBox
            key={definition.name}
            definition={definition}
            data={getParamData(parameters, definition)}
            disabled={disabled}
            onChangeData={(data) => onValueChange(definition.name, data)}
          />
        ))}
    </FormSection>
  );
};

function getParamData(params: ParametersMetadata, definition: ParamDefinition): ParamData {
  return (
    params.parameters[definition.name] ?? {
      value: definition.defaultValue,
    }
  );
}
