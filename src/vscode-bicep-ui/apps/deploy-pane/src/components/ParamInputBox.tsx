// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import styled from "styled-components";
import { useEffect, useState, type FC, type PropsWithChildren } from "react";
import type { ParamDefinition, ParameterValue } from "../models";

import {
  VscodeCheckbox,
  VscodeLabel,
  VscodeOption,
  VscodeSingleSelect,
  VscodeTextarea,
  VscodeTextfield,
} from "@vscode-elements/react-elements";
import { ErrorAlert } from "./ErrorAlert";

export type ParameterInputData = {
  inputValue?: unknown;
  value?: ParameterValue;
  isValid: boolean;
}

type InputProps = {
  definition: ParamDefinition;
  disabled: boolean;
  data: ParameterInputData;
  onChangeData: (value: ParameterInputData) => void;
}

const getInputHtmlId = (definition: ParamDefinition) => `param-input-${definition.name.toLowerCase()}`;

const ParamCheckboxInput: FC<InputProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  const { name, defaultValue } = definition;
  const defaultInputValue = defaultValue as boolean;

  const [inputValue, setInputValue] = useState<boolean>(data.inputValue as boolean ?? defaultInputValue);

  useEffect(() => {
    onChangeData({ inputValue, value: inputValue, isValid: true });
  }, [inputValue, onChangeData]);

  return (
    <InputBoxWrapper disabled={disabled}>
      <VscodeCheckbox
        id={getInputHtmlId(definition)}
        checked={inputValue}
        onChange={() => setInputValue(!inputValue)}
        disabled={disabled}
      >
        {name}
      </VscodeCheckbox>
    </InputBoxWrapper>
  );
}

const ParamIntInput: FC<InputProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  const { name, defaultValue } = definition;
  const defaultInputValue = `${defaultValue ?? 0}`;

  const [inputValue, setInputValue] = useState<string>(data.inputValue as string ?? defaultInputValue);
  const [error, setError] = useState<string>();

  useEffect(() => {
    const newValueInt = Number(inputValue);
    if (Number.isInteger(newValueInt)) {
      onChangeData({ inputValue, value: newValueInt, isValid: true });
      setError(undefined);
    } else {
      onChangeData({ inputValue, isValid: false });
      setError("Invalid integer value");
    }
  }, [inputValue, onChangeData]);

  return (
    <InputBoxWrapper disabled={disabled} error={error}>
      <VscodeLabel htmlFor={getInputHtmlId(definition)}>{name}</VscodeLabel>
      <VscodeTextfield
        id={getInputHtmlId(definition)}
        value={inputValue}
        onChange={e => setInputValue((e.currentTarget as HTMLInputElement).value)}
        disabled={disabled}
      />
    </InputBoxWrapper>
  );
}

const ParamStringInput: FC<InputProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  const { name, defaultValue } = definition;
  const defaultInputValue = defaultValue as string;

  const [inputValue, setInputValue] = useState<string>(data.inputValue as string ?? defaultInputValue);

  useEffect(() => {
    onChangeData({ inputValue, value: inputValue, isValid: true });
  }, [inputValue, onChangeData]);

  if (definition.allowedValues) {
    return (
      <InputBoxWrapper disabled={disabled}>
        <VscodeLabel htmlFor={getInputHtmlId(definition)}>{name}</VscodeLabel>
        <VscodeSingleSelect
          id={getInputHtmlId(definition)}
          onChange={e => setInputValue((e.currentTarget as HTMLSelectElement).value)}
          disabled={disabled}
        >
          {definition.allowedValues.map((option) => (
            <VscodeOption key={option} selected={inputValue === option}>
              {option}
            </VscodeOption>
          ))}
        </VscodeSingleSelect>
      </InputBoxWrapper>
    );
  } else {
    return (
      <InputBoxWrapper disabled={disabled}>
        <VscodeLabel htmlFor={getInputHtmlId(definition)}>{name}</VscodeLabel>
        <VscodeTextfield
          id={getInputHtmlId(definition)}
          value={inputValue}
          onChange={e => setInputValue((e.currentTarget as HTMLInputElement).value)}
          disabled={disabled}
        />
      </InputBoxWrapper>
    );
  }
}

const ParamJsonInput: FC<InputProps> = (props) => {
  const { definition, data, disabled, onChangeData } = props;
  const { name, defaultValue } = definition;
  const defaultInputValue = defaultValue ? JSON.stringify(defaultValue, null, 2) : '';

  const [inputValue, setInputValue] = useState<string>(data.inputValue as string ?? defaultInputValue);
  const [error, setError] = useState<string>();

  useEffect(() => {
    try {
      onChangeData({ inputValue, value: inputValue !== '' ? JSON.parse(inputValue) : undefined, isValid: true });
      setError(undefined);
    } catch {
      onChangeData({ inputValue, isValid: false });
      setError("Invalid JSON value");
    }
  }, [inputValue, onChangeData]);

  return (
    <InputBoxWrapper disabled={disabled} error={error}>
      <VscodeLabel htmlFor={getInputHtmlId(definition)}>{name}</VscodeLabel>
      <VscodeTextarea
        id={getInputHtmlId(definition)}
        className="code-textarea-container"
        resize="vertical"
        value={inputValue}
        onChange={e => setInputValue((e.currentTarget as HTMLInputElement).value)}
        disabled={disabled}
      />
    </InputBoxWrapper>
  );
}

type InputBoxWrapperProps = PropsWithChildren<{
  disabled: boolean;
  error?: string;
}>;

const InputRowDiv = styled.div`
  display: grid;
  grid-auto-flow: row;
`;

const InputBoxWrapper: FC<InputBoxWrapperProps> = (props) => {
  const { error, children } = props;
  return (
    <InputRowDiv>
      {children}
      {error && <ErrorAlert message={error} />}
    </InputRowDiv>
  );
};

export const ParamInputBox: FC<InputProps> = (props) => {
  const { definition } = props;
  const { type } = definition;

  switch (type) {
    case "bool":
      return <ParamCheckboxInput {...props} />;
    case "int":
      return <ParamIntInput {...props} />;
    case "string":
      return <ParamStringInput {...props} />;
    default:
      return <ParamJsonInput {...props} />;
  }
};