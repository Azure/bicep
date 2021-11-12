// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import renderer from "react-test-renderer";
import { ThemeProvider } from "styled-components";
import "jest-styled-components";

import { Graph } from "../../visualizer/app/components/Graph";
import { darkTheme } from "../../visualizer/app/themes";

describe("component Graph", () => {
  it("should render", () => {
    const graph = renderer.create(
      <ThemeProvider theme={darkTheme}>
        <Graph elements={[]} />
      </ThemeProvider>
    );

    expect(graph.toJSON()).toMatchSnapshot();
  });
});
