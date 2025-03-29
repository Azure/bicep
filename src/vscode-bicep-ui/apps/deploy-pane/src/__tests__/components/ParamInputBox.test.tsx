// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import type { ParamDefinition } from "../../models";

import { render } from "@testing-library/react";
import { expect, it, vi } from "vitest";
import { ParamInputBox } from "../../components/ParamInputBox";

// TODO these tests aren't particularly meaningful, as vite doesn't support web components

it("renders a string input", () => {
  const definition: ParamDefinition = {
    name: "myStringParam",
    type: "string",
    defaultValue: "fooValue",
  };
  const onChangeData = vi.fn();

  const { container } = render(
    <ParamInputBox definition={definition} disabled={false} onValueChange={onChangeData} />,
  );

  expect(container).toMatchSnapshot();
});

it("renders an integer input", () => {
  const definition: ParamDefinition = {
    name: "myIntParam",
    type: "int",
    defaultValue: 42,
  };
  const onChangeData = vi.fn();

  const { container } = render(
    <ParamInputBox definition={definition} disabled={false} onValueChange={onChangeData} />,
  );

  expect(container).toMatchSnapshot();
});

it("renders a bool input", () => {
  const definition: ParamDefinition = {
    name: "myBoolParam",
    type: "bool",
    defaultValue: true,
  };
  const onChangeData = vi.fn();

  const { container } = render(
    <ParamInputBox definition={definition} disabled={false} onValueChange={onChangeData} />,
  );

  expect(container).toMatchSnapshot();
});

it("renders a JSON input", () => {
  const definition: ParamDefinition = {
    name: "myJsonParam",
    type: "object",
    defaultValue: { foo: "bar" },
  };
  const onChangeData = vi.fn();

  const { container } = render(
    <ParamInputBox definition={definition} disabled={false} onValueChange={onChangeData} />,
  );

  expect(container).toMatchSnapshot();
});
