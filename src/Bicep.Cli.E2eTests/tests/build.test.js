const path = require("path");
const fs = require("fs");
const runBicepCommand = require("../command");

const exampleDirectory = path.resolve(__dirname, "../examples/101/aks/");
const exampleBicepFile = path.join(exampleDirectory, "main.bicep");
const exampleJsonFile = path.join(exampleDirectory, "main.json");

describe("bicep build", () => {
  it("should build a bicep file", () => {
    const result = runBicepCommand("build", exampleBicepFile);
    expect(result.status).toBe(0);
    expect(fs.existsSync(exampleJsonFile)).toBeTruthy();

    const jsonContents = fs.readFileSync(exampleJsonFile, {
      encoding: "utf-8",
    });
    expect(jsonContents.length).toBeGreaterThan(0);

    // Build with --stdout should emit consistent result.
    const stdoutResult = runBicepCommand("build", "--stdout", exampleBicepFile);
    expect(stdoutResult.status).toBe(0);
    expect(stdoutResult.stdout).toBe(jsonContents);
  });
});
