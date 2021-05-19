// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import renderer from "react-test-renderer";
import "jest-styled-components";

import { Graph } from "../../visualizer/app/components/Graph";

describe("component Graph", () => {
  it("should render", () => {
    const tree = renderer.create(<Graph elements={[]} />).toJSON();

    expect(tree).toMatchInlineSnapshot(`
      .c0 {
        position: absolute;
        left: 0px;
        top: 0px;
        bottom: 0px;
        right: 0px;
      }

      <div
        className="c0"
      />
    `);
  });
});
