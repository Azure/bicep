// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import { render } from "@testing-library/react";
import { ThemeProvider } from "styled-components";

import "jest-styled-components";

import { Graph } from "../../visualizer/app/components/Graph";
import { darkTheme } from "../../visualizer/app/themes";

describe("component Graph", () => {
  it("should render", () => {
    const { asFragment } = render(
      <ThemeProvider theme={darkTheme}>
        <Graph elements={[]} />
      </ThemeProvider>,
    );

    expect(asFragment()).toMatchSnapshot();
  });
});
