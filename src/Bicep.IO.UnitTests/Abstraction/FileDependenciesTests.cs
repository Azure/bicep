// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;

namespace Bicep.IO.UnitTests.Abstraction;

[TestClass]
public class FileDependenciesTests
{
    private readonly InMemoryFileExplorer fileExplorer = new();

    [TestMethod]
    public void IsCurrent_NothingChanged_ReturnsTrue()
    {
        var file = this.CreateFile("/config/a.json", "{}");
        var missingFile = this.GetFile("/config/missing.json");

        var sut = Build(builder =>
        {
            builder.AddFile(file);
            builder.AddFile(missingFile);
        });

        sut.IsCurrent().Should().BeTrue();
    }

    [TestMethod]
    public void IsCurrent_RecordedFileRewritten_ReturnsFalse()
    {
        var file = this.CreateFile("/config/a.json", "{}");
        var sut = Build(builder => builder.AddFile(file));

        // Same content: a save still counts as a change.
        file.WriteAllText("{}");

        sut.IsCurrent().Should().BeFalse();
    }

    [TestMethod]
    public void IsCurrent_RecordedFileDeleted_ReturnsFalse()
    {
        var file = this.CreateFile("/config/a.json", "{}");
        var sut = Build(builder => builder.AddFile(file));

        file.Delete();

        sut.IsCurrent().Should().BeFalse();
    }

    [TestMethod]
    public void IsCurrent_MissingFileCreated_ReturnsFalse()
    {
        var file = this.GetFile("/config/a.json");
        var sut = Build(builder => builder.AddFile(file).Should().BeNull());

        file.WriteAllText("{}");

        sut.IsCurrent().Should().BeFalse();
    }

    [TestMethod]
    public void IsCurrent_ExistenceOnlyFileRewritten_ReturnsTrue()
    {
        var file = this.CreateFile("/config/a.json", "{}");
        var sut = Build(builder => builder.AddExistence(file).Should().BeTrue());

        file.WriteAllText("""{ "changed": true }""");

        sut.IsCurrent().Should().BeTrue();
    }

    [TestMethod]
    public void IsCurrent_ExistenceOnlyFileCreatedOrDeleted_ReturnsFalse()
    {
        var created = this.GetFile("/config/created.json");
        var deleted = this.CreateFile("/config/deleted.json", "{}");
        var createdDependencies = Build(builder => builder.AddExistence(created));
        var deletedDependencies = Build(builder => builder.AddExistence(deleted));

        created.WriteAllText("{}");
        deleted.Delete();

        createdDependencies.IsCurrent().Should().BeFalse();
        deletedDependencies.IsCurrent().Should().BeFalse();
    }

    [TestMethod]
    public void IsCurrent_FailedReadRecorded_ReturnsFalse()
    {
        var sut = Build(builder => builder.AddFailedRead());

        sut.IsCurrent().Should().BeFalse();
    }

    private static FileDependencies Build(Action<FileDependencies.Builder> record)
    {
        var builder = new FileDependencies.Builder();
        record(builder);

        return builder.Build();
    }

    private IFileHandle GetFile(string path) => this.fileExplorer.GetFile(new IOUri("file", "", path));

    private IFileHandle CreateFile(string path, string text)
    {
        var file = this.GetFile(path);
        file.WriteAllText(text);

        return file;
    }
}
