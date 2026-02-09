# Bicep Extension Debugging Guide

Debug Bicep extensions locally using gRPC reflection and HTTP mode.

## Quick Start

```bash
# 1. Set environment & start extension
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --project ./MyExtension.csproj -- --http 5001

# 2. Verify it's running
grpcurl -plaintext localhost:5001 extension.BicepExtension/Ping

# 3. Test a request
grpcurl -plaintext -d '{"type":"MyResource","properties":"{}","config":"{}"}' \
  localhost:5001 extension.BicepExtension/CreateOrUpdate
```

## Prerequisites

| Tool | Install | Description |
|------|---------|-------------|
| .NET 9 SDK | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) | Required |
| grpcurl | `choco install grpcurl` (Win) / `brew install grpcurl` (Mac) | CLI tool |
| grpcui | `choco install grpcui` (Win) / `brew install grpcui` (Mac) | Web UI (optional) |
| Bicep CLI | v0.37.4+ | Required |

---

## IDE Setup

### Visual Studio

Create `Properties/launchSettings.json`:

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "Debug HTTP Mode": {
      "commandName": "Project",
      "commandLineArgs": "--http 5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BICEP_TRACING_ENABLED": "true"
      }
    },
    "Debug Named Pipe": {
      "commandName": "Project",
      "commandLineArgs": "--pipe bicep-ext-debug",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BICEP_TRACING_ENABLED": "true"
      }
    },
    "Debug Unix Socket": {
      "commandName": "Project",
      "commandLineArgs": "--socket /tmp/bicep-ext-debug.sock",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BICEP_TRACING_ENABLED": "true"
      }
    }
  }
}
```

Select profile from dropdown → **F5** to debug.

### VS Code

Create `.vscode/launch.json`:

```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "Debug Extension (HTTP)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/MyExtension.dll",
      "args": ["--http", "5001"],
      "cwd": "${workspaceFolder}",
      "console": "integratedTerminal",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BICEP_TRACING_ENABLED": "true"
      }
    },
    {
      "name": "Debug Extension (Named Pipe)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/MyExtension.dll",
      "args": ["--pipe", "bicep-ext-debug"],
      "cwd": "${workspaceFolder}",
      "console": "integratedTerminal",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BICEP_TRACING_ENABLED": "true"
      }
    },
    {
      "name": "Debug Extension (Unix Socket)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/bin/Debug/net9.0/MyExtension.dll",
      "args": ["--socket", "/tmp/bicep-ext-debug.sock"],
      "cwd": "${workspaceFolder}",
      "console": "integratedTerminal",
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "BICEP_TRACING_ENABLED": "true"
      }
    },
    {
      "name": "Attach to Extension Process",
      "type": "coreclr",
      "request": "attach",
      "processName": "MyExtension"
    }
  ]
}
```

Create `.vscode/tasks.json`:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/MyExtension.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

Open Run and Debug (**Ctrl+Shift+D**) → Select config → **F5**.

### Command Line

```powershell
# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:BICEP_TRACING_ENABLED = $true
dotnet run --project .\MyExtension.csproj -- --http 5001
```

```bash
# macOS/Linux
export ASPNETCORE_ENVIRONMENT=Development
export BICEP_TRACING_ENABLED=true
dotnet run --project ./MyExtension.csproj -- --http 5001
```

### Startup Options

| Option | Description | Example |
|--------|-------------|---------|
| `--http <port>` | HTTP/2 gRPC on TCP port | `--http 5001` |
| `--socket <path>` | Unix domain socket | `--socket /tmp/ext.sock` |
| `--pipe <name>` | Windows named pipe | `--pipe bicep-ext-pipe` |

---

## gRPC Service Contract

### Service Definition

```protobuf
syntax = "proto3";
option csharp_namespace = "Bicep.Local.Rpc";
package extension;

service BicepExtension {
  rpc CreateOrUpdate (ResourceSpecification) returns (LocalExtensibilityOperationResponse);
  rpc Preview (ResourceSpecification) returns (LocalExtensibilityOperationResponse);
  rpc Get (ResourceReference) returns (LocalExtensibilityOperationResponse);
  rpc Delete (ResourceReference) returns (LocalExtensibilityOperationResponse);
  rpc GetTypeFiles(Empty) returns (TypeFilesResponse);
  rpc Ping(Empty) returns (Empty);
}
```

### Message Types

#### ResourceSpecification (CreateOrUpdate, Preview)

```protobuf
message ResourceSpecification {
  optional string config = 1;      // Extension configuration (JSON)
  string type = 2;                 // Resource type name
  optional string apiVersion = 3;  // API version (optional)
  string properties = 4;           // Resource properties (JSON)
}
```

#### ResourceReference (Get, Delete)

```protobuf
message ResourceReference {
  string identifiers = 1;          // Resource identifiers (JSON)
  optional string config = 2;      // Extension configuration (JSON)
  string type = 3;                 // Resource type name
  optional string apiVersion = 4;  // API version (optional)
}
```

#### LocalExtensibilityOperationResponse

```protobuf
message LocalExtensibilityOperationResponse {
  optional Resource resource = 1;  // Successful resource data
  optional ErrorData errorData = 2; // Error information (if failed)
}

message Resource {
  string type = 1;
  optional string apiVersion = 2;
  string identifiers = 3;          // Resource identifiers (JSON)
  string properties = 4;           // Resource properties (JSON)
  optional string status = 5;
}

message ErrorData {
  Error error = 1;
}

message Error {
  string code = 1;
  optional string target = 2;
  string message = 3;
  repeated ErrorDetail details = 4;
  optional string innerError = 5;
}

message ErrorDetail {
  string code = 1;
  optional string target = 2;
  string message = 3;
}
```

#### TypeFilesResponse

```protobuf
message TypeFilesResponse {
  string indexFile = 1;            // Type index content
  map<string, string> typeFiles = 2; // Type definition files
}
```

---

## Testing Tools

### Option 1: grpcui (Web UI)

**Best for**: Users familiar with Postman or prefer a visual interface.

```bash
# Install
choco install grpcui   # Windows
brew install grpcui    # macOS

# Launch (opens browser automatically)
grpcui -plaintext localhost:5001
```

This opens an interactive web interface where you can:

- Browse available services and methods
- Fill in request fields using a form (no JSON escaping needed)
- View formatted responses
- See request/response history

#### Using grpcui

1. **Select service**: Choose `extension.BicepExtension` from the dropdown
2. **Select method**: Pick the RPC method (e.g., `CreateOrUpdate`)
3. **Fill request fields**:
   - `type`: `MyResource`
   - `properties`: `{"name": "test"}` (enter as plain JSON, no escaping)
   - `config`: `{}`
4. **Click Invoke**: View the response in the right panel

#### grpcui vs grpcurl

| Feature | grpcui | grpcurl |
|---------|--------|---------|
| Interface | Web browser | Command line |
| JSON input | Form fields (no escaping) | Requires escape sequences |
| History | Built-in request history | Manual (use shell history) |
| Scriptable | No | Yes |
| Learning curve | Lower (Postman-like) | Higher (CLI syntax) |

### Option 2: grpcurl (CLI)

**Best for**: Scripting, automation, and CI/CD pipelines.

#### Quick Reference

| Action | Command |
|--------|---------|
| List services | `grpcurl -plaintext localhost:5001 list` |
| Describe service | `grpcurl -plaintext localhost:5001 describe extension.BicepExtension` |
| Ping | `grpcurl -plaintext localhost:5001 extension.BicepExtension/Ping` |
| Get types | `grpcurl -plaintext localhost:5001 extension.BicepExtension/GetTypeFiles` |
| CreateOrUpdate | `grpcurl -plaintext -d '{"type":"...", "properties":"...", "config":"{}"}' localhost:5001 extension.BicepExtension/CreateOrUpdate` |
| Preview | `grpcurl -plaintext -d '{"type":"...", "properties":"...", "config":"{}"}' localhost:5001 extension.BicepExtension/Preview` |
| Get | `grpcurl -plaintext -d '{"type":"...", "identifiers":"...", "config":"{}"}' localhost:5001 extension.BicepExtension/Get` |
| Delete | `grpcurl -plaintext -d '{"type":"...", "identifiers":"...", "config":"{}"}' localhost:5001 extension.BicepExtension/Delete` |

#### Endpoint Examples

**Ping**

```bash
grpcurl -plaintext localhost:5001 extension.BicepExtension/Ping
# Output: {}
```

**GetTypeFiles**

```bash
grpcurl -plaintext localhost:5001 extension.BicepExtension/GetTypeFiles
# Output: {}
```

**CreateOrUpdate**

```bash
grpcurl -plaintext -d '{
  "type": "echo",
  "properties": "{\"payload\": \"Hello, World!\"}",
  "config": "{}"
}' localhost:5001 extension.BicepExtension/CreateOrUpdate
```

```json
{
  "resource": {
    "type": "echo",
    "identifiers": "{}",
    "properties": "{\"payload\":\"Hello, World!\"}"
  }
}
```

**Preview**

```bash
grpcurl -plaintext -d '{
  "type": "MyResource",
  "properties": "{\"name\": \"preview-test\"}",
  "config": "{}"
}' localhost:5001 extension.BicepExtension/Preview
```

**Get**

```bash
grpcurl -plaintext -d '{
  "type": "MyResource",
  "identifiers": "{\"name\": \"my-resource\"}",
  "config": "{}"
}' localhost:5001 extension.BicepExtension/Get
```

**Delete**

```bash
grpcurl -plaintext -d '{
  "type": "MyResource",
  "identifiers": "{\"name\": \"resource-to-delete\"}",
  "config": "{}"
}' localhost:5001 extension.BicepExtension/Delete
```

---

## Debugging Workflow

1. **Set breakpoints** in handler methods
2. **Start debugger** (F5) with HTTP profile
3. **Send request** via grpcui or grpcurl
4. **Step through code**:
   - **F10** - Step Over
   - **F11** - Step Into
   - **Shift+F11** - Step Out
   - **F5** - Continue
5. **Inspect variables**:
   - `request.Type` - Resource type
   - `request.Properties` - JSON properties string
   - `request.Config` - Extension configuration
   - `context.CancellationToken` - Timeout handling

### Discovering Services

```bash
# List all available services
grpcurl -plaintext localhost:5001 list

# Output:
# extension.BicepExtension
# grpc.reflection.v1alpha.ServerReflection

# Describe service methods
grpcurl -plaintext localhost:5001 describe extension.BicepExtension

# Describe message types
grpcurl -plaintext localhost:5001 describe extension.ResourceSpecification
```

---

## Test Automation Scripts

### PowerShell (Windows)

```powershell
# test-extension.ps1
$env:ASPNETCORE_ENVIRONMENT = "Development"
$port = 5001

Write-Host "Starting extension..." -ForegroundColor Green
$job = Start-Job { dotnet run --project .\MyExtension.csproj -- --http 5001 }
Start-Sleep -Seconds 3

@(
    @{Cmd="Ping"; Data=$null; Desc="Health Check"}
    @{Cmd="GetTypeFiles"; Data=$null; Desc="Get Types"}
    @{Cmd="CreateOrUpdate"; Data='{"type":"MyResource","properties":"{}","config":"{}"}'; Desc="Create"}
    @{Cmd="Get"; Data='{"type":"MyResource","identifiers":"{}","config":"{}"}'; Desc="Get"}
) | ForEach-Object {
    Write-Host "`n$($_.Desc):" -ForegroundColor Yellow
    if ($_.Data) {
        grpcurl -plaintext -d $_.Data localhost:$port extension.BicepExtension/$($_.Cmd)
    } else {
        grpcurl -plaintext localhost:$port extension.BicepExtension/$($_.Cmd)
    }
}

Write-Host "`nPress any key to stop..." -ForegroundColor Red
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Stop-Job $job; Remove-Job $job
```

### Bash (macOS/Linux)

```bash
#!/bin/bash
# test-extension.sh
export ASPNETCORE_ENVIRONMENT=Development
PORT=5001

echo -e "\033[0;32mStarting extension...\033[0m"
dotnet run --project ./MyExtension.csproj -- --http $PORT &
PID=$!
sleep 3

echo -e "\n\033[1;33mHealth Check:\033[0m"
grpcurl -plaintext localhost:$PORT extension.BicepExtension/Ping

echo -e "\n\033[1;33mGet Types:\033[0m"
grpcurl -plaintext localhost:$PORT extension.BicepExtension/GetTypeFiles

echo -e "\n\033[1;33mCreate Resource:\033[0m"
grpcurl -plaintext -d '{"type":"MyResource","properties":"{}","config":"{}"}' \
  localhost:$PORT extension.BicepExtension/CreateOrUpdate

echo -e "\n\033[1;33mGet Resource:\033[0m"
grpcurl -plaintext -d '{"type":"MyResource","identifiers":"{}","config":"{}"}' \
  localhost:$PORT extension.BicepExtension/Get

echo -e "\n\033[0;31mPress any key to stop...\033[0m"
read -n 1 -s
kill $PID
```

---

## Common Scenarios

### Handler Not Registered

```bash
grpcurl -plaintext -d '{"type":"UnknownType","properties":"{}","config":"{}"}' \
  localhost:5001 extension.BicepExtension/CreateOrUpdate
```

```json
{
  "errorData": {
    "error": {
      "code": "HandlerNotRegistered",
      "message": "No handler registered for type 'UnknownType'."
    }
  }
}
```

### JSON Escaping (grpcurl only)

Properties and identifiers are JSON strings—escape inner quotes:

```bash
grpcurl -plaintext -d '{
  "type": "MyResource",
  "properties": "{\"name\": \"test\", \"nested\": {\"key\": \"value\"}}",
  "config": "{}"
}' localhost:5001 extension.BicepExtension/CreateOrUpdate
```

> **Tip**: Use grpcui to avoid JSON escaping entirely—enter JSON directly in form fields.

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "Failed to dial target host" | Check extension is running on correct port |
| "Unimplemented" reflection error | Set `ASPNETCORE_ENVIRONMENT=Development` |
| "No handler registered" | Verify resource type matches handler registration |
| Malformed JSON | Escape inner quotes: `"{\"key\": \"value\"}"` or use grpcui |
| Connection refused | Check port isn't used by another process |
| Breakpoints not hitting | Use Debug config, not Release |
| launchSettings.json ignored | Must be in `Properties/` folder |
| grpcui won't connect | Ensure extension started with `--http` flag |

---

## Resources

- [Bicep Local Deploy](https://github.com/Azure/bicep/blob/main/docs/experimental/local-deploy.md)
- [.NET Extension Quickstart](https://github.com/Azure/bicep/blob/main/docs/experimental/local-deploy-dotnet-quickstart.md)
- [gRPC Proto Definition](https://github.com/Azure/bicep/blob/main/src/Bicep.Local.Rpc/extension.proto)
- [grpcurl Documentation](https://github.com/fullstorydev/grpcurl#readme)
- [grpcui Documentation](https://github.com/fullstorydev/grpcui#readme)

---

*Questions? File an issue on the [Bicep GitHub repository](https://github.com/Azure/bicep/issues).*
