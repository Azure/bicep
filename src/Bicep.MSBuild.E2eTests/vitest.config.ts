import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    testTimeout: 150000,
    environment: "node",
  },
});
