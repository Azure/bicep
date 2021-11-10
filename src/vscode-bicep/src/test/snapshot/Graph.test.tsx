// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import renderer from "react-test-renderer";
import "jest-styled-components";

import { Graph } from "../../visualizer/app/components/Graph";
import { darkTheme } from "../../visualizer/app/themes";

describe("component Graph", () => {
  it("should render", () => {
    const tree = renderer
      .create(<Graph elements={[]} theme={darkTheme} />)
      .toJSON();

    expect(tree).toMatchSnapshot();
  });
});
