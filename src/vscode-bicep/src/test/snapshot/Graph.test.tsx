// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import renderer from "react-test-renderer";
import "jest-styled-components";

import { Graph } from "../../visualizer/app/components/Graph";
import { darkTheme } from "../../visualizer/app/components/themes";

describe("component Graph", () => {
  it("should render", () => {
    const tree = renderer
      .create(<Graph elements={[]} theme={darkTheme} />)
      .toJSON();

    expect(tree).toMatchInlineSnapshot(`
      .c0 {
        position: absolute;
        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;
        overflow: hidden;
        background-color: #111111;
        background-image: radial-gradient(circle at 1px 1px,#3f3f3f 1px,transparent 0);
        background-size: 24px 24px;
        background-position: 12px 12px;
      }

      <div
        className="c0"
      />
    `);
  });
});
