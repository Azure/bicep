const runBicepCommand = require("../command");

describe("bicep --help", () => {
  it("should output help information", () => {
    const result = runBicepCommand(["--help"]);
    expect(result.status).toBe(0);
    expect(result.stdout.length).toBeGreaterThan(0);
  });
});
