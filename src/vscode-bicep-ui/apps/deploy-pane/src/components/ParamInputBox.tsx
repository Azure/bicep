// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { FC, PropsWithChildren } from "react";
import type { ParamDefinition, ParameterValue } from "../models";

import {
  VscodeCheckbox,
  VscodeLabel,
  VscodeOption,
  VscodeSingleSelect,
  VscodeTextarea,
  VscodeTextfield,
} from "@vscode-elements/react-elements";
import { useEffect, useState } from "react";
import styled from "styled-components";
import { ErrorAlert } from "./ErrorAlert";

export type ParameterInputData = {
  value?: ParameterValue;
  isValid: boolean;
};

type InputProps = {
  name: string;
  inputId: string;
  disabled: boolean;
  initialValue?: ParameterValue;
  onValueChange: (value?: ParameterValue, error?: string) => void;
};

const getInputHtmlId = (definition: ParamDefinition) => `param-input-${definition.name.toLowerCase()}`;

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

type ParamInputBoxProps = {
  disabled: boolean;
  definition: ParamDefinition;
  initialValue?: ParameterValue;
  onValueChange: (value?: ParameterValue, error?: string) => void;
};

export const ParamInputBox: FC<ParamInputBoxProps> = (props) => {
  const { definition, initialValue, disabled, onValueChange } = props;
  const { type, name } = definition;
  const [error, setError] = useState<string>();

  function handleValueChange(value: ParameterValue, error?: string) {
    onValueChange(value, error);
    setError(error);
  } 

  switch (type) {
    case "bool":
      return <CheckboxInput 
        name={name}
        inputId={getInputHtmlId(definition)}
        disabled={disabled}
        initialValue={initialValue}
        onValueChange={handleValueChange} />
    case "int":
      return (
        <InputBoxWrapper disabled={props.disabled} error={error}>
          <IntInput
            name={name}
            inputId={getInputHtmlId(definition)}
            disabled={disabled}
            initialValue={initialValue}
            onValueChange={handleValueChange} />
        </InputBoxWrapper>
      );
    case "string":
      if (definition.allowedValues) {
        return (
          <InputBoxWrapper disabled={props.disabled} error={error}>
            <DropdownInput
              name={name}
              inputId={getInputHtmlId(definition)}
              disabled={disabled}
              allowedValues={definition.allowedValues}
              initialValue={initialValue}
              onValueChange={handleValueChange} />
          </InputBoxWrapper>
        );
      }

      return (
        <InputBoxWrapper disabled={props.disabled} error={error}>
          <StringInput
            name={name}
            inputId={getInputHtmlId(definition)}
            disabled={disabled}
            initialValue={initialValue}
            onValueChange={handleValueChange} />
        </InputBoxWrapper>
      );
    default:
      return (
        <InputBoxWrapper disabled={props.disabled} error={error}>
          <JsonInput
            name={name}
            inputId={getInputHtmlId(definition)}
            disabled={disabled}
            initialValue={initialValue}
            onValueChange={handleValueChange} />
        </InputBoxWrapper>
      );
  }
};

const CheckboxInput: FC<InputProps> = (props) => {
  const { name, inputId, disabled, initialValue, onValueChange } = props;
  const [inputValue, setInputValue] = useState<boolean>();

  function handleInputChange(value: boolean) {
    onValueChange(value, undefined);
    setInputValue(value);
  }

  useEffect(() => {
    handleInputChange(initialValue as boolean ?? false);
  }, [initialValue]);

  return (
    <>
      <VscodeCheckbox
        id={inputId}
        checked={inputValue}
        onChange={() => handleInputChange(!inputValue)}
        disabled={disabled}
      >
        {name}
      </VscodeCheckbox>
    </>
  );
};

const IntInput: FC<InputProps> = (props) => {
  const { name, inputId, disabled, initialValue, onValueChange } = props;
  const [inputValue, setInputValue] = useState<string>();

  function handleInputChange(value: string) {
    const newValueInt = Number(value);
    if (Number.isInteger(newValueInt)) {
      onValueChange(newValueInt, undefined);
    } else {
      onValueChange(undefined, "Invalid integer value");
    }
    setInputValue(value);
  }

  useEffect(() => {
    handleInputChange(`${initialValue as number ?? 0}`);
  }, [initialValue]);

  return (
    <>
      <VscodeLabel htmlFor={inputId}>{name}</VscodeLabel>
      <VscodeTextfield
        id={inputId}
        value={inputValue}
        onChange={(e) => handleInputChange((e.currentTarget as HTMLInputElement).value)}
        disabled={disabled}
      />
    </>
  );
};

const JsonInput: FC<InputProps> = (props) => {
  const { name, inputId, disabled, initialValue, onValueChange } = props;
  const [inputValue, setInputValue] = useState<string>();

  function handleInputChange(value: string) {
    try {
      onValueChange(value !== "" ? JSON.parse(value) : undefined, undefined);
    } catch {
      onValueChange(undefined, "Invalid JSON value");
    }
    setInputValue(value);
  }

  useEffect(() => {
    handleInputChange(JSON.stringify(initialValue, null, 2) ?? '');
  }, [initialValue]);

  return (
    <>
      <VscodeLabel htmlFor={inputId}>{name}</VscodeLabel>
      <VscodeTextarea
        id={inputId}
        className="code-textarea-container"
        resize="vertical"
        value={inputValue}
        onChange={(e) => handleInputChange((e.currentTarget as HTMLInputElement).value)}
        disabled={disabled}
      />
    </>
  );
};

const StringInput: FC<InputProps> = (props) => {
  const { name, inputId, disabled, initialValue, onValueChange } = props;
  const [inputValue, setInputValue] = useState<string>();

  function handleInputChange(value: string) {
    onValueChange(value, undefined);
    setInputValue(value);
  }

  useEffect(() => {
    handleInputChange(initialValue as string ?? '');
  }, [initialValue]);

  return (
    <>
      <VscodeLabel htmlFor={inputId}>{name}</VscodeLabel>
      <VscodeTextfield
        id={inputId}
        value={inputValue}
        onChange={(e) => handleInputChange((e.currentTarget as HTMLInputElement).value)}
        disabled={disabled}
      />
    </>
  );
};

const DropdownInput: FC<InputProps & { allowedValues: string[] }> = (props) => {
  const { name, inputId, allowedValues, disabled, initialValue, onValueChange } = props;
  const [inputValue, setInputValue] = useState<string>();

  function handleInputChange(value: string) {
    onValueChange(value, undefined);
    setInputValue(value);
  }

  useEffect(() => {
    handleInputChange(initialValue as string ?? '');
  }, [initialValue]);

  return (
    <>
      <VscodeLabel htmlFor={inputId}>{name}</VscodeLabel>
      <VscodeSingleSelect
        id={inputId}
        onChange={(e) => handleInputChange((e.currentTarget as HTMLSelectElement).value)}
        disabled={disabled}
      >
        {allowedValues.map((option) => (
          <VscodeOption key={option} selected={inputValue === option}>
            {option}
          </VscodeOption>
        ))}
      </VscodeSingleSelect>
    </>
  );
};