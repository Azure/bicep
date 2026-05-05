// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Bicep.RpcClient.Helpers;
using FluentAssertions;

namespace Bicep.RpcClient.Tests;

[TestClass]
public class BicepInstallerTests
{
    public TestContext TestContext { get; set; } = null!;

    private string GetTempPath(string fileName) =>
        Path.Combine(TestContext.TestRunDirectory!, $"{TestContext.TestName}_{fileName}");

    [TestMethod]
    public void TryAcquireFileLock_returns_a_stream_and_creates_the_lock_file_when_not_already_locked()
    {
        var lockPath = GetTempPath("lock1");

        using var lockStream = BicepInstaller.TryAcquireFileLock(lockPath);

        lockStream.Should().NotBeNull();
        File.Exists(lockPath).Should().BeTrue();
    }

    [TestMethod]
    public void TryAcquireFileLock_returns_null_when_the_file_is_already_locked()
    {
        var lockPath = GetTempPath("lock2");

        using var first = BicepInstaller.TryAcquireFileLock(lockPath);
        first.Should().NotBeNull();

        using var second = BicepInstaller.TryAcquireFileLock(lockPath);
        second.Should().BeNull();
    }

    [TestMethod]
    public void TryAcquireFileLock_can_reacquire_the_lock_after_the_previous_lock_is_disposed()
    {
        var lockPath = GetTempPath("lock3");

        using (var first = BicepInstaller.TryAcquireFileLock(lockPath))
        {
            first.Should().NotBeNull();
        }

        using var second = BicepInstaller.TryAcquireFileLock(lockPath);
        second.Should().NotBeNull();
    }

    [TestMethod]
    public void MoveFileAtomically_moves_the_file_when_the_destination_does_not_exist()
    {
        var src = GetTempPath("src1.bin");
        var dst = GetTempPath("dst1.bin");
        File.WriteAllText(src, "content");

        BicepInstaller.MoveFileAtomically(src, dst);

        File.Exists(src).Should().BeFalse();
        File.ReadAllText(dst).Should().Be("content");
    }

    [TestMethod]
    public void MoveFileAtomically_replaces_the_destination_file_when_it_already_exists()
    {
        var src = GetTempPath("src2.bin");
        var dst = GetTempPath("dst2.bin");
        File.WriteAllText(src, "new");
        File.WriteAllText(dst, "old");

        BicepInstaller.MoveFileAtomically(src, dst);

        File.Exists(src).Should().BeFalse();
        File.ReadAllText(dst).Should().Be("new");
    }

    [TestMethod]
    public void SetExecutablePermissions_sets_the_executable_bit_for_owner_group_and_others()
    {
        var filePath = GetTempPath("exec_test.sh");
        File.WriteAllText(filePath, "#!/bin/sh\necho hello");

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // this function is a no-op on Windows - verify it doesn't throw, but otherwise skip the rest of the test
            BicepInstaller.SetExecutablePermissions(filePath, CancellationToken.None);
            return;
        }

        // Ensure the file is not executable before the call
        var modeBefore = File.GetUnixFileMode(filePath);
        (modeBefore & UnixFileMode.UserExecute).Should().Be(UnixFileMode.None);

        BicepInstaller.SetExecutablePermissions(filePath, CancellationToken.None);

        var modeAfter = File.GetUnixFileMode(filePath);
        (modeAfter & UnixFileMode.UserExecute).Should().Be(UnixFileMode.UserExecute);
        (modeAfter & UnixFileMode.GroupExecute).Should().Be(UnixFileMode.GroupExecute);
        (modeAfter & UnixFileMode.OtherExecute).Should().Be(UnixFileMode.OtherExecute);
    }

    [TestMethod]
    public void SetExecutablePermissions_throws_operation_canceled_when_the_token_is_already_canceled()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Inconclusive("chmod is not applicable on Windows.");
        }

        var filePath = GetTempPath("exec_cancel.sh");
        File.WriteAllText(filePath, "#!/bin/sh");

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => BicepInstaller.SetExecutablePermissions(filePath, cts.Token);

        act.Should().Throw<OperationCanceledException>();
    }

    [TestMethod]
    public async Task DoWithFileSystemLockAsync_executes_the_action_when_the_lock_is_available()
    {
        var lockPath = GetTempPath("dolock1.lock");
        var executed = false;

        await BicepInstaller.DoWithFileSystemLockAsync(lockPath, () =>
        {
            executed = true;
            return Task.CompletedTask;
        }, CancellationToken.None);

        executed.Should().BeTrue();
    }

    [TestMethod]
    public async Task DoWithFileSystemLockAsync_throws_operation_canceled_when_the_lock_is_held_and_the_token_is_canceled()
    {
        var lockPath = GetTempPath("dolock2.lock");

        // Hold the lock for the duration of the test
        using var heldLock = BicepInstaller.TryAcquireFileLock(lockPath);
        heldLock.Should().NotBeNull();

        var act = async () => await BicepInstaller.DoWithFileSystemLockAsync(
            lockPath,
            () => Task.CompletedTask,
            CancellationToken.None);

        // The default timeout is 30s which is too long for a test — cancel after a short delay instead
        using var cts = new CancellationTokenSource(millisecondsDelay: 1_000);

        var cancelAct = async () => await BicepInstaller.DoWithFileSystemLockAsync(
            lockPath,
            () => Task.CompletedTask,
            cts.Token);

        await cancelAct.Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public async Task DoWithFileSystemLockAsync_throws_operation_canceled_when_the_token_is_already_canceled()
    {
        var lockPath = GetTempPath("dolock3.lock");
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var act = async () => await BicepInstaller.DoWithFileSystemLockAsync(
            lockPath,
            () => Task.CompletedTask,
            cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public async Task DoWithFileSystemLockAsync_releases_the_lock_after_the_action_completes()
    {
        var lockPath = GetTempPath("dolock4.lock");

        await BicepInstaller.DoWithFileSystemLockAsync(lockPath, () => Task.CompletedTask, CancellationToken.None);

        // Lock should be acquirable again after the action completes
        using var lockStream = BicepInstaller.TryAcquireFileLock(lockPath);
        lockStream.Should().NotBeNull();
    }

    [TestMethod]
    public async Task DoWithFileSystemLockAsync_releases_the_lock_when_the_action_throws()
    {
        var lockPath = GetTempPath("dolock5.lock");

        var act = async () => await BicepInstaller.DoWithFileSystemLockAsync(
            lockPath,
            () => throw new InvalidOperationException("boom"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        using var lockStream = BicepInstaller.TryAcquireFileLock(lockPath);
        lockStream.Should().NotBeNull();
    }
}
