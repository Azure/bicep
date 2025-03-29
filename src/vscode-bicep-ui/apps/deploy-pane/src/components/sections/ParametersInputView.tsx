// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";
import type { ParamDefinition, ParametersMetadata, TemplateMetadata } from "../../models";
import type { ParameterInputData } from "../ParamInputBox";

import { VscodeButton, VscodeTextfield } from "@vscode-elements/react-elements";
import { useCallback, useEffect, useState } from "react";
import { ParamInputBox } from "../ParamInputBox";
import { FormSection } from "./FormSection";

export type ParametersInputData = Record<string, ParameterInputData>;

interface ParametersInputViewProps {
  template?: TemplateMetadata;
  initialParameters?: ParametersMetadata;
  disabled: boolean;
  onParametersChange: (parameters: ParametersInputData) => void;
  onEnableEditing: () => void;
  onPickParametersFile: () => void;
}

export const ParametersInputView: FC<ParametersInputViewProps> = ({
  template,
  initialParameters,
  disabled,
  onParametersChange,
  onEnableEditing,
  onPickParametersFile,
}) => {
  const definitions = template?.parameterDefinitions;
  const sourceFilePath = initialParameters?.sourceFilePath;
  const [values, setValues] = useState<ParametersInputData>({});

  const handleValueChange = useCallback(
    (name: string, value: ParameterInputData) => {
      const newValues = { ...values, [name]: value };
      setValues(newValues);
      onParametersChange(newValues);
    },
    [values, setValues, onParametersChange],
  );

  useEffect(() => {
    setValues((curValues) => getValuesForDefinitionsUpdate(definitions ?? [], curValues));
  }, [definitions]);

  useEffect(() => {
    if (initialParameters) {
      setValues((curValues) => getValuesForInitialValueUpdate(initialParameters, curValues));
    }
  }, [initialParameters]);

  return (
    <FormSection title="Parameters" hidden={!template || !initialParameters}>
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
        (definitions ?? [])
          .filter((x) => !!values[x.name])
          .map((definition) => (
            <ParamInputBox
              key={definition.name}
              definition={definition}
              disabled={disabled}
              initialValue={initialParameters?.parameters[definition.name]?.value ?? definition.defaultValue}
              onValueChange={(value, error) => handleValueChange(definition.name, { value, isValid: !error })}
            />
          ))}
    </FormSection>
  );
};

function getValuesForDefinitionsUpdate(definitions: ParamDefinition[], prevValues: ParametersInputData): ParametersInputData {
  const values = { ...prevValues };
  let hasUpdates = false;

  for (const definition of definitions) {
    if (!values[definition.name]) {
      hasUpdates = true;
      values[definition.name] = {
        value: definition.defaultValue,
        isValid: true,
      };
    }
  }

  return hasUpdates ? values : prevValues;
}


function getValuesForInitialValueUpdate(metadata: ParametersMetadata, prevValues: ParametersInputData): ParametersInputData {
  const values = { ...prevValues };

  for (const [name, param] of Object.entries(metadata.parameters)) {
    values[name] = {
      value: param.value,
      isValid: true,
    }
  }

  return values;
}
