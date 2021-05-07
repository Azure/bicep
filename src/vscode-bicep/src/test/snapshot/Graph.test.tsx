// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import renderer from "react-test-renderer";

import { Graph } from "../../visualizer/app/components/Graph";

describe("component Graph", () => {
  it("should render", () => {
    const tree = renderer.create(<Graph elements={[]} />).toJSON();

    expect(tree).toMatchInlineSnapshot(`
      <div
        className="sc-bdvvaa EglMc"
      />
    `);
  });
});
