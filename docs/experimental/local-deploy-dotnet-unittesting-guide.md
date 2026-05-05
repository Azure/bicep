# Unit Testing Bicep Extensions: A Practical Guide

Building reliable Bicep extensions requires thorough testing. This guide shows you how to write unit tests for extensions built with the `Bicep.Local.Extension` framework, using dependency injection and loose coupling to create testable, maintainable code.

## Prerequisites

- .NET 9 SDK
- Familiarity with MSTest, Moq, and FluentAssertions
- Basic understanding of `TypedResourceHandler<TProperties, TIdentifiers>`

---

## Project Setup

Create a test project alongside your extension:

```xml
<!-- filepath: YourExtension.UnitTests/YourExtension.UnitTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\YourExtension\YourExtension.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="MSTest.TestAdapter" />
    <PackageReference Include="MSTest.TestFramework" />
    <PackageReference Include="Moq" />
    <PackageReference Include="FluentAssertions" />
  </ItemGroup>
</Project>
```

---

## Defining Your Models

First, define the properties and identifiers for your resource. The framework handles JSON serialization automatically.

```csharp
// Models for a file resource handler
public class FileProperties
{
    public string Content { get; set; } = string.Empty;
    public string? Encoding { get; set; }
}

public class FileIdentifiers
{
    public string Path { get; set; } = string.Empty;
}
```

---

## Writing Testable Handlers

### The Problem: Hard-to-Test Code

Handlers that directly access external resources are difficult to test:

```csharp
// ❌ Hard to test - direct file system access
public class FileResourceHandler : TypedResourceHandler<FileProperties, FileIdentifiers>
{
    protected override Task<ExtensibilityOperationSuccessResponse> CreateOrUpdate(
        FileProperties properties,
        FileIdentifiers identifiers,
        CancellationToken cancellationToken)
    {
        // Direct dependency - can't test without real file system
        File.WriteAllText(identifiers.Path, properties.Content);
        
        return Task.FromResult(CreateSuccessResponse(properties, identifiers));
    }
}
```

### The Solution: Dependency Injection

Abstract external dependencies behind interfaces and inject them:

```csharp
// Define the abstraction
public interface IFileSystemService
{
    Task WriteFileAsync(string path, string content, CancellationToken cancellationToken);
    Task<string> ReadFileAsync(string path, CancellationToken cancellationToken);
    Task DeleteFileAsync(string path, CancellationToken cancellationToken);
    bool FileExists(string path);
}

// ✅ Testable - uses injected service
public class FileResourceHandler : TypedResourceHandler<FileProperties, FileIdentifiers>
{
    private readonly IFileSystemService _fileSystem;

    public FileResourceHandler(IFileSystemService fileSystem)
    {
        _fileSystem = fileSystem;
    }

    protected override async Task<ExtensibilityOperationSuccessResponse> CreateOrUpdate(
        FileProperties properties,
        FileIdentifiers identifiers,
        CancellationToken cancellationToken)
    {
        await _fileSystem.WriteFileAsync(identifiers.Path, properties.Content, cancellationToken);
        return CreateSuccessResponse(properties, identifiers);
    }

    protected override async Task<ExtensibilityOperationSuccessResponse> Get(
        FileIdentifiers identifiers,
        CancellationToken cancellationToken)
    {
        if (!_fileSystem.FileExists(identifiers.Path))
        {
            throw new ResourceNotFoundException($"File not found: {identifiers.Path}");
        }

        var content = await _fileSystem.ReadFileAsync(identifiers.Path, cancellationToken);
        var properties = new FileProperties { Content = content };
        
        return CreateSuccessResponse(properties, identifiers);
    }

    protected override async Task<ExtensibilityOperationSuccessResponse> Delete(
        FileIdentifiers identifiers,
        CancellationToken cancellationToken)
    {
        await _fileSystem.DeleteFileAsync(identifiers.Path, cancellationToken);
        return CreateSuccessResponse(new FileProperties(), identifiers);
    }
}
```

---

## Testing Your Handlers

### Test Class Structure

```csharp
[TestClass]
public class FileResourceHandlerTests
{
    private Mock<IFileSystemService> _mockFileSystem = null!;
    private FileResourceHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockFileSystem = new Mock<IFileSystemService>(MockBehavior.Strict);
        _handler = new FileResourceHandler(_mockFileSystem.Object);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _mockFileSystem.VerifyAll();
    }
}
```

### Testing CreateOrUpdate

```csharp
[TestMethod]
public async Task CreateOrUpdate_WritesFileAndReturnsSuccess()
{
    // Arrange
    var properties = new FileProperties { Content = "Hello, World!" };
    var identifiers = new FileIdentifiers { Path = "/tmp/test.txt" };

    _mockFileSystem
        .Setup(fs => fs.WriteFileAsync(identifiers.Path, properties.Content, It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    // Act
    var response = await _handler.CreateOrUpdate(properties, identifiers, CancellationToken.None);

    // Assert
    response.Should().NotBeNull();
    response.Resource.Should().NotBeNull();
}

[TestMethod]
public async Task CreateOrUpdate_WhenWriteFails_ThrowsException()
{
    // Arrange
    var properties = new FileProperties { Content = "content" };
    var identifiers = new FileIdentifiers { Path = "/invalid/path.txt" };

    _mockFileSystem
        .Setup(fs => fs.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ThrowsAsync(new IOException("Access denied"));

    // Act & Assert
    await FluentActions
        .Invoking(() => _handler.CreateOrUpdate(properties, identifiers, CancellationToken.None))
        .Should()
        .ThrowAsync<IOException>()
        .WithMessage("Access denied");
}
```

### Testing Get Operations

```csharp
[TestMethod]
public async Task Get_WhenFileExists_ReturnsContent()
{
    // Arrange
    var identifiers = new FileIdentifiers { Path = "/tmp/existing.txt" };
    var expectedContent = "file content";

    _mockFileSystem.Setup(fs => fs.FileExists(identifiers.Path)).Returns(true);
    _mockFileSystem
        .Setup(fs => fs.ReadFileAsync(identifiers.Path, It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedContent);

    // Act
    var response = await _handler.Get(identifiers, CancellationToken.None);

    // Assert
    response.Should().NotBeNull();
}

[TestMethod]
public async Task Get_WhenFileDoesNotExist_ThrowsNotFoundException()
{
    // Arrange
    var identifiers = new FileIdentifiers { Path = "/tmp/missing.txt" };
    
    _mockFileSystem.Setup(fs => fs.FileExists(identifiers.Path)).Returns(false);

    // Act & Assert
    await FluentActions
        .Invoking(() => _handler.Get(identifiers, CancellationToken.None))
        .Should()
        .ThrowAsync<ResourceNotFoundException>();
}
```

### Testing Delete Operations

```csharp
[TestMethod]
public async Task Delete_RemovesFileSuccessfully()
{
    // Arrange
    var identifiers = new FileIdentifiers { Path = "/tmp/delete-me.txt" };

    _mockFileSystem
        .Setup(fs => fs.DeleteFileAsync(identifiers.Path, It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    // Act
    var response = await _handler.Delete(identifiers, CancellationToken.None);

    // Assert
    response.Should().NotBeNull();
}
```

---

## Testing Your Services

While handlers are tested with mocks, services that interact with external systems need integration-style tests with proper isolation.

### Production Service Implementation

```csharp
public class FileSystemService : IFileSystemService
{
    public async Task WriteFileAsync(string path, string content, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        await File.WriteAllTextAsync(path, content, cancellationToken);
    }

    public async Task<string> ReadFileAsync(string path, CancellationToken cancellationToken)
        => await File.ReadAllTextAsync(path, cancellationToken);

    public Task DeleteFileAsync(string path, CancellationToken cancellationToken)
    {
        File.Delete(path);
        return Task.CompletedTask;
    }

    public bool FileExists(string path) => File.Exists(path);
}
```

### Service Tests with Isolated Directories

```csharp
[TestClass]
public class FileSystemServiceTests
{
    private string _testDirectory = null!;
    private FileSystemService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
        _service = new FileSystemService();
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }

    [TestMethod]
    public async Task WriteFileAsync_CreatesFileWithContent()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "test.txt");
        var content = "Hello, World!";

        // Act
        await _service.WriteFileAsync(filePath, content, CancellationToken.None);

        // Assert
        File.Exists(filePath).Should().BeTrue();
        (await File.ReadAllTextAsync(filePath)).Should().Be(content);
    }

    [TestMethod]
    public async Task WriteFileAsync_CreatesNestedDirectories()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "subdir", "nested", "test.txt");

        // Act
        await _service.WriteFileAsync(filePath, "content", CancellationToken.None);

        // Assert
        File.Exists(filePath).Should().BeTrue();
    }

    [TestMethod]
    public async Task ReadFileAsync_ReturnsFileContent()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "read-test.txt");
        var expectedContent = "Expected content";
        await File.WriteAllTextAsync(filePath, expectedContent);

        // Act
        var result = await _service.ReadFileAsync(filePath, CancellationToken.None);

        // Assert
        result.Should().Be(expectedContent);
    }

    [TestMethod]
    public void FileExists_ReturnsTrueForExistingFile()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "exists.txt");
        File.WriteAllText(filePath, "content");

        // Act & Assert
        _service.FileExists(filePath).Should().BeTrue();
    }

    [TestMethod]
    public void FileExists_ReturnsFalseForMissingFile()
    {
        // Arrange
        var filePath = Path.Combine(_testDirectory, "does-not-exist.txt");

        // Act & Assert
        _service.FileExists(filePath).Should().BeFalse();
    }
}
```

---

## Data-Driven Tests

Use `[DataRow]` to test multiple scenarios efficiently:

```csharp
[TestMethod]
[DataRow("simple.txt", "Hello")]
[DataRow("path/to/nested.txt", "Nested content")]
[DataRow("unicode-файл.txt", "Unicode: 你好世界")]
public async Task CreateOrUpdate_HandlesVariousInputs(string path, string content)
{
    // Arrange
    var properties = new FileProperties { Content = content };
    var identifiers = new FileIdentifiers { Path = path };

    _mockFileSystem
        .Setup(fs => fs.WriteFileAsync(path, content, It.IsAny<CancellationToken>()))
        .Returns(Task.CompletedTask);

    // Act
    var response = await _handler.CreateOrUpdate(properties, identifiers, CancellationToken.None);

    // Assert
    response.Should().NotBeNull();
}
```

---

## Running Tests

```powershell
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test class
dotnet test --filter "FullyQualifiedName~FileResourceHandlerTests"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## Quick Reference

### Testing Strategy by Component

| Component | What to Test | Approach |
|-----------|--------------|----------|
| **Handlers** | Business logic, validation, error handling | Mock all dependencies |
| **Services** | External integrations, I/O operations | Isolated test environments |
| **Validators** | Input validation rules | Direct instantiation |

### Common Patterns

```csharp
// Strict mocking - fails on unexpected calls
var mock = new Mock<IService>(MockBehavior.Strict);

// Verify all setups were invoked
mock.VerifyAll();

// Fluent assertions
result.Should().NotBeNull();
result.Should().BeEquivalentTo(expected);

// Async exception testing
await FluentActions
    .Invoking(() => handler.Method())
    .Should().ThrowAsync<ExpectedException>();
```

---

## Summary

This guide demonstrated how to write testable Bicep extensions:

1. **Use Dependency Injection** — Abstract external dependencies behind interfaces so handlers can be tested in isolation.

2. **Apply Loose Coupling** — Handlers depend on abstractions, allowing you to swap real implementations for test doubles.

3. **Structure Tests Consistently** — Follow Arrange-Act-Assert, use strict mocking, and test one scenario per method.

### Why It Matters

- **Fast Feedback** — Unit tests run in milliseconds, catching bugs before deployment.
- **Safe Refactoring** — Passing tests confirm your changes haven't broken existing behavior.
- **Living Documentation** — Tests describe how handlers should behave, helping new contributors understand the codebase.

### Next Steps

1. **Identify Dependencies** — Review your handlers for direct dependencies on files, networks, or databases.
2. **Create Abstractions** — Define interfaces for each external dependency.
3. **Add Tests Incrementally** — Start with your simplest handler and expand coverage over time.

For more on testing patterns, see [Microsoft's Unit Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices) and [xUnit Patterns](http://xunitpatterns.com/).
