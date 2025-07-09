import sharedConfig from "../../eslint.config.mjs";
import storybook from "eslint-plugin-storybook";

export default [
  ...sharedConfig,
  ...storybook.configs["flat/recommended"],
]
