// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
import renderer from "react-test-renderer";
import "jest-styled-components";

import { App } from "../../visualizer/app/components/App";

describe("component App", () => {
  it("should render", () => {
    const tree = renderer.create(<App />).toJSON();

    expect(tree).toMatchSnapshot();
  });
});
