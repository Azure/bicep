// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { render } from "@testing-library/react";

import "jest-styled-components";

import { App } from "../../visualizer/app/components/App";

describe("component App", () => {
  it("should render", () => {
    const { asFragment } = render(<App />);

    expect(asFragment()).toMatchSnapshot();
  });
});
