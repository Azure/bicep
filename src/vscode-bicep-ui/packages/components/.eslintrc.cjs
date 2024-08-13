// @ts-check

/** @type {import("eslint").Linter.LegacyConfig} */
const config = {
  extends: ["../../.eslintrc.cjs", "plugin:storybook/recommended", "plugin:testing-library/react"],
};

module.exports = config;
