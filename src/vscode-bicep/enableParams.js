const merger = require("json-merger");
const fs = require("fs");
const result = merger.mergeFiles(["./package.json", "./package.params.json"], { defaultArrayMergeOperation: "concat"});
const formatted = JSON.stringify(result, null, 2);
fs.writeFileSync("./package.json", formatted);