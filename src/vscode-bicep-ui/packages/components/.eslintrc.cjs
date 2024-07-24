// @ts-check

/** @type {import("eslint").Linter.Config} */
const config = {
  extends: ["../../.eslintrc.cjs", "plugin:storybook/recommended", "plugin:testing-library/react"],
};

module.exports = config;
