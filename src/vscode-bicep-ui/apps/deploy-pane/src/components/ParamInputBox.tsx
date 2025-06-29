// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC } from "react";
import type { ParamData, ParamDefinition } from "../models";

import {
  VscodeButton,
  VscodeCheckbox,
  VscodeLabel,
  VscodeOption,
  VscodeSingleSelect,
  VscodeTextarea,
  VscodeTextfield,
} from "@vscode-elements/react-elements";

interface ParamInputBoxProps {
  definition: ParamDefinition;
  data: ParamData;
  disabled: boolean;
  onChangeData: (data: ParamData) => void;
}

export const ParamInputBox: FC<ParamInputBoxProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  const { name, defaultValue, type } = definition;
  const { value } = data;

  function handleValueChange(value: unknown) {
    onChangeData({ ...data, value });
  }

  function handleResetToDefaultClick() {
    handleValueChange(defaultValue);
  }

  function getInputBox() {
    const inputHtmlId = `param-input-${name.toLowerCase()}`;
    switch (type) {
      case "bool":
        return (
          <VscodeCheckbox
            id={inputHtmlId}
            checked={!!value}
            onChange={() => handleValueChange(!value)}
            disabled={disabled}
          >
            {name}
          </VscodeCheckbox>
        );
      case "int":
        return (
          <>
            <VscodeLabel htmlFor={inputHtmlId}>{name}</VscodeLabel>
            <VscodeTextfield
              id={inputHtmlId}
              value={`${value ?? 0}`}
              onChange={(e) => handleValueChange(parseInt((e.currentTarget as HTMLInputElement).value, 10))}
              disabled={disabled}
            />
          </>
        );
      case "string":
        if (definition.allowedValues) {
          return (
            <>
              <VscodeLabel htmlFor={inputHtmlId}>{name}</VscodeLabel>
              <VscodeSingleSelect
                id={inputHtmlId}
                onChange={(e) => handleValueChange((e.currentTarget as HTMLSelectElement).value)}
                disabled={disabled}
              >
                {definition.allowedValues.map((option) => (
                  <VscodeOption key={option} selected={value === option}>
                    {option}
                  </VscodeOption>
                ))}
              </VscodeSingleSelect>
            </>
          );
        } else {
          return (
            <>
              <VscodeLabel htmlFor={inputHtmlId}>{name}</VscodeLabel>
              <VscodeTextfield
                id={inputHtmlId}
                value={`${value ?? ""}`}
                onChange={(e) => handleValueChange((e.currentTarget as HTMLInputElement).value)}
                disabled={disabled}
              />
            </>
          );
        }
      default:
        return (
          <>
            <VscodeLabel htmlFor={inputHtmlId}>{name}</VscodeLabel>
            <VscodeTextarea
              id={inputHtmlId}
              className="code-textarea-container"
              resize="vertical"
              value={value ? JSON.stringify(value, null, 2) : ""}
              onChange={(e) => handleValueChange(JSON.parse((e.currentTarget as HTMLInputElement).value))}
              disabled={disabled}
            />
          </>
        );
    }
  }

  return (
    <span className="input-row">
      {getInputBox()}
      {defaultValue !== undefined && value !== defaultValue && (
        <VscodeButton onClick={handleResetToDefaultClick} disabled={disabled}>
          Reset to default
        </VscodeButton>
      )}
    </span>
  );
};
