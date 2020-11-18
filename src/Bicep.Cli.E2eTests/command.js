const path = require("path");
const spawn = require('cross-spawn');

const bicepCliExecutable = path.resolve(
  __dirname,
  process.env.BICEP_CLI_EXECUTABLE || "../Bicep.Cli/bin/Debug/net5.0/bicep"
);

function runBicepCommand(args, expectError = false) {
  const result = spawn.sync(bicepCliExecutable, args, {
    stdio: "pipe",
    encoding: "utf-8",
  });

  if (!expectError && result.stderr.length > 0) {
    console.error(result.stderr);
  }

  return result;
}

module.exports = runBicepCommand;
