# Using the Bicep MCP Server

## What is it

We have built a Bicep MCP server with agentic tools to support Bicep code generation for AI agents in VS Code. To find out more about MCP, see [Use MCP servers in VS Code][00].

### Available Bicep MCP tools

- `get_bicep_best_practices`: Lists up-to-date recommended Bicep best-practices for authoring templates. These practices help improve maintainability, security, and reliability of your Bicep files. This is helpful additional context if you've been asked to generate Bicep code.
- `list_azure_resource_types`: Lists all available Azure resource types and their API versions for a specific Azure resource provider namespace. Data is sourced from Azure Resource Provider APIs.
- `get_azure_resource_type_schema`: Gets the schema for a specific Azure resource type and API version. Data is sourced from Azure Resource Provider APIs.
- `list_extension_resource_types`: Lists all available resource types for a Bicep extension. Accepts a canonical OCI artifact reference (e.g., `br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:1.0.0`).
- `get_extension_resource_type_schema`: Gets the schema for a specific extension resource type. Accepts a canonical OCI artifact reference, resource type, and API version.
- `list_well_known_extensions`: Lists well-known Bicep extensions (e.g., Microsoft Graph) with their dynamically-discovered version tags from MCR. This is not an exhaustive list; other extensions may exist. Use this to discover extensions and their versions for use with the extension resource type tools.
- `list_avm_metadata`: Lists up-to-date metadata for all Azure Verified Modules (AVM). The return value is a newline-separated list of AVM metadata. Each line includes the module name, description, versions, and documentation URI for a specific module.
- `get_bicep_file_diagnostics`: Analyzes a Bicep file (`.bicep`) or Bicep parameters file (`.bicepparam`) and returns all compilation diagnostics including errors, warnings, and informational messages.
- `format_bicep_file`: Formats a Bicep file (`.bicep`) or Bicep parameters file (`.bicepparam`) according to official Bicep formatting standards, respecting `bicepconfig.json` settings.
- `get_file_references`: Analyzes a Bicep or Bicep parameters file and returns a list of all files it references, including modules, parameter files, and other dependencies.
- `decompile_arm_template_file`: Converts an ARM template JSON file into Bicep syntax (`.bicep`). Accepts files with `.json`, `.jsonc`, or `.arm` extensions.
- `decompile_arm_parameters_file`: Converts an ARM template parameters JSON file into Bicep parameters syntax (`.bicepparam`). Accepts files with `.json`, `.jsonc`, or `.arm` extensions.
- `get_deployment_snapshot`: Creates a deployment snapshot from a Bicep parameters file (`.bicepparam`) by compiling and pre-expanding the ARM template, allowing you to preview predicted resources without running a deployment.

## Transport

The Bicep MCP Server uses **stdio** transport, launched locally by a client process (e.g., VS Code, Claude Desktop). This is provided by the `Azure.Bicep.McpServer` NuGet tool.

For remote hosting scenarios, you can build your own MCP server with HTTP transport using the `Azure.Bicep.McpServer.Core` NuGet library (see [Building a Remote MCP Server](#building-a-remote-mcp-server) below).

## Where can I use it?

The Bicep MCP Server can be used directly in VS Code (preferred), but can also be run locally with other AI services such as Claude Desktop and Code, OpenAI Codex CLI, LMStudio, and other MCP-compatible services. 

## How to use the Bicep MCP Server directly in VS Code

### Prerequisites

- Install the latest version of the [Bicep VS Code Extension][01]
- Confirm access to [Copilot in VS Code][02]

### Installing

Ensure you have the latest version of the Bicep extension installed.

### Troubleshooting

The Bicep server may not appear in your list of MCP servers and tools in VS Code until it has been triggered. If you do not see the server, try opening and saving a `.bicep` file and then try providing a Bicep related-prompt in the Copilot chat window in "Agent" mode (as shown in Step #3 of the Viewing and Using Bicep Tools in the Bicep MCP Server section below). You may also need to press the "Refresh" button in the Copilot chat box.

![Refresh copilot tools][05]

If any of the tools are missing from the list of available tools, start/restart the MCP server in VS Code, by hitting `Ctrl + Shift + P`, selecting `MCP: List Servers`, then choosing the Bicep MCP Server and clicking on `Start Server` or `Restart Server`.

### Viewing and Using Bicep MCP Server Tools in VS Code

1. Open the GitHub Copilot extension window and select "Agent Mode".

   ![Agent Mode Selection][06]

1. Click on the tool icon in the GitHub Copilot chat window and search for "Bicep (PREVIEW)".

   ![Bicep MCP Tool Selection][07]

1. Start using Agent Mode to help with your Bicep tasks!

   ![Bicep MCP Usage Example][08]

## How to use the Bicep MCP Server locally with AI Agents
Please refer to [this step by step tutorial](https://github.com/johnlokerse/azure-bicep-mcp-integration-setup) on how to integrate the Bicep MCP Server with Claude Code, Codex, LM Studio, and other AI tools.

This article has all the tools you need to run the Bicep MCP Server locally, with pre-written commands, helper scripts, and client setup guides.

Note: This is contributed by our community member [@johnlokerse](https://github.com/johnlokerse). Thanks John!

## Building a Remote MCP Server

The `Azure.Bicep.McpServer.Core` NuGet library provides the `AddBicepMcpServer()` extension method and all Bicep tool definitions, allowing you to build your own MCP server with any transport (HTTP, stdio, etc.).

Create a new ASP.NET Core project and add the required packages:

```xml
<ItemGroup>
  <FrameworkReference Include="Microsoft.AspNetCore.App" />
  <PackageReference Include="Azure.Bicep.McpServer.Core" />
  <PackageReference Include="ModelContextProtocol.AspNetCore" />
</ItemGroup>
```

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddBicepMcpServer()
    .WithHttpTransport(options => options.Stateless = true);
var app = builder.Build();
app.MapMcp();
await app.RunAsync();
```

### Self-hosting with Docker

Publish your project and create a `Dockerfile`:

```bash
dotnet publish -c Release -o ./publish
```

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY publish/ .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "MyBicepMcpServer.dll"]
```

Build and run:

```bash
docker build -t bicep-mcp-server .
docker run -p 8080:8080 bicep-mcp-server
```

> [!NOTE]
> No authentication is included. If hosting on a network or in the cloud, secure the endpoint using a reverse proxy, VNet integration, or other infrastructure-level controls.

## Limitations

> [!NOTE]
> It is your responsibility to review all code generated by an LLM and **deploy at your own risk**.

These tools provide additional context to help the chosen model generate semantically and syntactically correct Bicep code. These tools are not designed to deploy directly to Azure.

There is no way to definitively guarantee whether the agent orchestrator will use any particular Bicep tool. As a workaround, you can view the available Bicep tools and use specific prompting to guide the agent orchestrator to invoke a tool (e.g. "Create a Bicep file to do X using Bicep best practices")

## Contributing and providing feedback

These tools are early on and we value and welcome feedback to improve them. See [`CONTRIBUTING.md`][09] for guidelines.

In particular, we are looking to crowd source community wisdom on the `get_bicep_best_practices` tool. You can contribute to our forum on bicep best practices on [this Bicep Issue][03].

## Raising bugs or feature requests

Please raise bug reports or feature requests under [Bicep Issues][04] and tag with "story: bicep MCP".

<!-- Link reference definitions -->
[00]: https://code.visualstudio.com/docs/copilot/chat/mcp-servers
[01]: https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-bicep
[02]: https://code.visualstudio.com/docs/copilot/overview
[03]: https://github.com/Azure/bicep/issues/17660
[04]: https://github.com/Azure/bicep/issues
[05]: ../images/refresh-mcp-tools.png
[06]: ../images/mcp-agent-mode.png
[07]: ../images/mcp-tools-selection.png
[08]: ../images/use-agent-mode-with-bicep.png
[09]: ../../CONTRIBUTING.md
