import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    testTimeout: 500000,
    hookTimeout: 500000,
    environment: "node",
  },
});
