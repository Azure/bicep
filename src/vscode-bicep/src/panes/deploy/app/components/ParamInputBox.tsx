// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import {
  VscodeButton,
  VscodeCheckbox,
  VscodeOption,
  VscodeSingleSelect,
  VscodeTextarea,
  VscodeTextfield
} from "@vscode-elements/react-elements";
import { FC } from "react";
import { ParamData, ParamDefinition } from "../../models";

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
    switch (type) {
      case "bool":
        return (
          <VscodeCheckbox checked={!!value} onChange={() => handleValueChange(!value)} disabled={disabled}>
            {name}
          </VscodeCheckbox>
        );
      case "int":
        return (
          <VscodeTextfield
            value={`${value ?? 0}`}
            onChange={(e) => handleValueChange(parseInt(e.currentTarget.value, 10))}
            disabled={disabled}
          >
            {name}
          </VscodeTextfield>
        );
      case "string":
        if (definition.allowedValues) {
          const dropdownHtmlId = `param-input-${name.toLowerCase()}`;
          return (
            <div className="dropdown-container">
              <label htmlFor={dropdownHtmlId}>{name}</label>
              <VscodeSingleSelect
                id={dropdownHtmlId}
                onChange={(e) => handleValueChange(e.currentTarget.value)}
                disabled={disabled}
              >
                {definition.allowedValues.map((option) => (
                  <VscodeOption key={option} selected={value === option}>
                    {option}
                  </VscodeOption>
                ))}
              </VscodeSingleSelect>
            </div>
          );
        } else {
          return (
            <VscodeTextfield
              value={`${value ?? ""}`}
              onChange={(e) => handleValueChange(e.currentTarget.value)}
              disabled={disabled}
            >
              {name}
            </VscodeTextfield>
          );
        }
      default:
        return (
          <VscodeTextarea
            className="code-textarea-container"
            resize="vertical"
            value={value ? JSON.stringify(value, null, 2) : ""}
            onChange={(e) => handleValueChange(JSON.parse(e.currentTarget.value))}
            disabled={disabled}
          >
            {name}
          </VscodeTextarea>
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
