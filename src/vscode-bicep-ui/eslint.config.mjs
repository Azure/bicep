import eslint from "@eslint/js";
import notice from "eslint-plugin-notice";
import reactPlugin from "eslint-plugin-react";
import reactHooksPlugin from "eslint-plugin-react-hooks";
import reactRefreshPlugin from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";

export default tseslint.config({
  files: ["**/*.{ts,tsx}"],
  ignores: ["node_modules", "dist"],
  plugins: {
    notice,
    "react-refresh": reactRefreshPlugin,
  },
  settings: {
    react: {
      version: "detect",
    },
  },
  extends: [
    eslint.configs.recommended,
    ...tseslint.configs.recommended,
    reactPlugin.configs.flat.recommended,
    reactPlugin.configs.flat["jsx-runtime"],
    {
      plugins: {
        "react-hooks": reactHooksPlugin,
      },
      rules: {
        ...reactHooksPlugin.configs.recommended.rules,
      },
    },
  ],
});
