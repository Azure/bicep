#!/bin/bash

./src/Bicep.Cli.E2eTests/node_modules/.bin/grpc_tools_node_protoc \
  --grpc_out=grpc_js:./src/Bicep.Cli.E2eTests/src/proto \
  --js_out=import_style=commonjs,binary:./src/Bicep.Cli.E2eTests/src/proto \
  src/Bicep.Cli/bicep.proto
  
./src/Bicep.Cli.E2eTests/node_modules/.bin/grpc_tools_node_protoc \
  --plugin=protoc-gen-ts=./src/Bicep.Cli.E2eTests/node_modules/.bin/protoc-gen-ts \
  --ts_out=grpc_js:./src/Bicep.Cli.E2eTests/src/proto \
  src/Bicep.Cli/bicep.proto