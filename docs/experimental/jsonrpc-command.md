# Using the `jsonrpc` command (Experimental!)

## What is it?
The `jsonrpc` command allows you to run the Bicep CLI in a JSONRPC server mode. This is useful for invoking the CLI programatically to consume structured output, and to avoid cold-start delays when compiling multiple files.

This makes it possible to build libraries which interact with Bicep files programatically in non-.NET languages.

## Usage (named pipe)
`bicep jsonrpc --pipe <named_pipe>`

Runs the JSONRPC server connected to a named pipe.

### Arguments
`<named_pipe>` A named pipe to connect the JSONRPC server to.

### Examples
#### Connecting to a named pipe (OSX/Linux)
`bicep jsonrpc --pipe /tmp/bicep-81375a8084b474fa2eaedda1702a7aa40e2eaa24b3.sock`

#### Connecting to a named pipe (Windows)
`bicep jsonrpc --pipe \\.\pipe\\bicep-81375a8084b474fa2eaedda1702a7aa40e2eaa24b3.sock`

## Usage (TCP socket)
`bicep jsonrpc --socket <tcp_socket>`

Runs the JSONRPC server connected to a TCP socket.

### Arguments
`<tcp_socket>` A socket number to connect the JSONRPC server to.

### Examples
#### Connecting to a TCP socket
`bicep jsonrpc --socket 12345`

## Usage (stdin/stdout)
`bicep jsonrpc --stdio`

Runs the JSONRPC server using stdin & stdout for messages.

## JSONRPC Contract

See [`ICliJsonRpcProtocol.cs`](../../src/Bicep.Cli/Rpc/ICliJsonRpcProtocol.cs) for the available methods & request/response bodies.

See [`jsonrpc.test.ts`](../../src/Bicep.Cli.E2eTests/src/jsonrpc.test.ts) for an example of connecting to the server and interacting with Bicep files programatically using Node.

## Example JSONRPC messages

### `bicep/getVersion`
#### Input
```json
{
  "jsonrpc": "2.0",
  "id": 0,
  "method": "bicep/version",
  "params": {}
}
```
#### Output
```json
{
  "jsonrpc": "2.0",
  "id": 0,
  "result": {
    "version": "0.24.211"
  }
}
```

### `bicep/compile`
#### Input
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "method": "bicep/compile",
  "params": {
    "path": "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/examples/101/aks.prod/main.bicep"
  }
}
```
#### Output
```json
{
  "jsonrpc": "2.0",
  "id": 1,
  "result": {
    "success": true,
    "diagnostics": [],
    "contents": "{\n  \"$schema\": \"https://schema.management.azure.com/schemas/2019-04-01/deploymentTemplate.json#\",\n  \"contentVersion\": \"1.0.0.0\",\n  \"metadata\": {\n    \"_generator\": {\n      \"name\": \"bicep\",\n      \"version\": \"0.24.211.59455\",\n      \"templateHash\": \"16887360739795423296\"\n    }\n  },\n  \"parameters\": {\n    \"dnsPrefix\": {\n      \"type\": \"string\"\n    },\n    \"linuxAdminUsername\": {\n      \"type\": \"string\"\n    },\n    \"sshRSAPublicKey\": {\n      \"type\": \"string\"\n    },\n    \"servicePrincipalClientId\": {\n      \"type\": \"string\"\n    },\n    \"servicePrincipalClientSecret\": {\n      \"type\": \"securestring\"\n    },\n    \"clusterName\": {\n      \"type\": \"string\",\n      \"defaultValue\": \"aks101cluster\"\n    },\n    \"location\": {\n      \"type\": \"string\",\n      \"defaultValue\": \"[resourceGroup().location]\"\n    },\n    \"osDiskSizeGB\": {\n      \"type\": \"int\",\n      \"defaultValue\": 0,\n      \"minValue\": 0,\n      \"maxValue\": 1023\n    },\n    \"agentCount\": {\n      \"type\": \"int\",\n      \"defaultValue\": 3,\n      \"minValue\": 1,\n      \"maxValue\": 50\n    },\n    \"agentVMSize\": {\n      \"type\": \"string\",\n      \"defaultValue\": \"Standard_DS2_v2\"\n    }\n  },\n  \"resources\": [\n    {\n      \"type\": \"Microsoft.ContainerService/managedClusters\",\n      \"apiVersion\": \"2020-09-01\",\n      \"name\": \"[parameters('clusterName')]\",\n      \"location\": \"[parameters('location')]\",\n      \"properties\": {\n        \"dnsPrefix\": \"[parameters('dnsPrefix')]\",\n        \"agentPoolProfiles\": [\n          {\n            \"name\": \"agentpool\",\n            \"osDiskSizeGB\": \"[parameters('osDiskSizeGB')]\",\n            \"count\": \"[parameters('agentCount')]\",\n            \"vmSize\": \"[parameters('agentVMSize')]\",\n            \"osType\": \"Linux\"\n          }\n        ],\n        \"linuxProfile\": {\n          \"adminUsername\": \"[parameters('linuxAdminUsername')]\",\n          \"ssh\": {\n            \"publicKeys\": [\n              {\n                \"keyData\": \"[parameters('sshRSAPublicKey')]\"\n              }\n            ]\n          }\n        },\n        \"servicePrincipalProfile\": {\n          \"clientId\": \"[parameters('servicePrincipalClientId')]\",\n          \"secret\": \"[parameters('servicePrincipalClientSecret')]\"\n        }\n      }\n    }\n  ],\n  \"outputs\": {\n    \"controlPlaneFQDN\": {\n      \"type\": \"string\",\n      \"value\": \"[reference(resourceId('Microsoft.ContainerService/managedClusters', parameters('clusterName')), '2020-09-01').fqdn]\"\n    }\n  }\n}"
  }
}
```

### `bicep/getDeploymentGraph`
#### Input
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "method": "bicep/getDeploymentGraph",
  "params": {
    "path": "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/metadata.bicep"
  }
}
```
#### Output
```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "nodes": [
      {
        "range": {
          "start": {
            "line": 5,
            "char": 4
          },
          "end": {
            "line": 8,
            "char": 5
          }
        },
        "name": "bar",
        "type": "My.Rp/foo",
        "isExisting": true
      },
      {
        "range": {
          "start": {
            "line": 10,
            "char": 4
          },
          "end": {
            "line": 13,
            "char": 5
          }
        },
        "name": "baz",
        "type": "My.Rp/foo",
        "isExisting": false
      },
      {
        "range": {
          "start": {
            "line": 1,
            "char": 4
          },
          "end": {
            "line": 3,
            "char": 5
          }
        },
        "name": "foo",
        "type": "My.Rp/foo",
        "isExisting": false
      }
    ],
    "edges": [
      {
        "source": "bar",
        "target": "foo"
      },
      {
        "source": "baz",
        "target": "bar"
      }
    ]
  }
}
```

### `bicep/getFileReferences`
#### Input
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "method": "bicep/getFileReferences",
  "params": {
    "path": "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/main.bicepparam"
  }
}
```
#### Output
```json
{
  "jsonrpc": "2.0",
  "id": 5,
  "result": {
    "filePaths": [
      "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/bicepconfig.json",
      "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/invalid.txt",
      "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/main.bicep",
      "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/main.bicepparam",
      "/Users/ant/Code/bicep/src/Bicep.Cli.E2eTests/src/temp/jsonrpc/valid.txt"
    ]
  }
}
```

## Raising bugs or feature requests
Please raise bug reports or feature requests under [Bicep Issues](https://github.com/Azure/bicep/issues) as usual.