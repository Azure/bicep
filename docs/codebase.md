# Codebase Overview
This document provides an overview of the Bicep compiler code structure, and design considerations.

## Project Structure
### Bicep.Core
### Bicep.Cli
### Bicep.LangServer
### VSCode Extension
### Peripherals (brief mention)
* Decompiler
* MSBuild
* Highlighter Libraries
* External packages (Deployments, Az Types)
### Pipelines & Build Process

## Compiler Structure
### Parser
* Parser
* Lexer
### Binder
* Binder
### Semantic Analysis
* Type Checker
* Linters
* Emit-specific checks
### Emitter
* Emitter

## Language Server Structure
* List handlers, brief mention how they integrate with compiler structure

## Type Generation
* Some mention of how this works

## Important Design Considerations
* Laziness & caching
* Error Recovery
* Immutability
* Nullability
* Transparent abstraction (warnings rather than errors)

## Testing Considerations
* Easy baseline updates
* Preference for e2e instead of unit tests (both desirable)