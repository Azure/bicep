// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Buffers.Binary;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Nodes;
using Azure.Deployments.Core.Definitions;
using Azure.Deployments.Extensibility.Core.V2.Models;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Configuration;
using Bicep.Core.UnitTests;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.Core.UnitTests.Utils;
using Bicep.IO.Abstraction;
using Bicep.Local.Deploy.Azure;
using Bicep.Local.Deploy.Extensibility;
using Bicep.Local.Deploy.Helpers;
using Bicep.Local.Deploy.Types;
using LocalExtensionAuthentication = Bicep.Local.Rpc.LocalExtensionAuthentication;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using static Bicep.Core.UnitTests.Utils.CompilationHelper;

namespace Bicep.Local.Deploy.IntegrationTests;

[TestClass]
public class GrpcLocalExtensionSecurityTests : TestBase
{
    private sealed record Endpoint(string? Socket, string? Pipe, int ProcessId, string[]? Arguments = null);

    [TestMethod]
    public void Test_host_fixture_contains_only_the_selected_host_build()
    {
        var fixture = Path.Combine(AppContext.BaseDirectory, "GrpcSecurityTestHost");
        const string Manifest = "Bicep.Local.Deploy.IntegrationTests.TestHost.deps.json";
        var manifests = Directory.GetFiles(fixture, Manifest, SearchOption.AllDirectories);

        manifests.Should().Equal([Path.Combine(fixture, Manifest)],
            "other runtime builds must not be copied into each child fixture");
    }

    [TestMethod]
    [DataRow("sdk-host")]
    [DataRow("sync-host")]
    public async Task Sdk_host_serves_resource_and_type_requests_through_production_factory(string hostName)
    {
        var binaryPath = PrepareHost(hostName);
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        await using var extension = await factory.Start(IOUri.FromFilePath(binaryPath));

        var cancellationToken = TestContext.CancellationTokenSource.Token;
        var specification = new ResourceSpecification("request", "v1", new JsonObject { ["input"] = "SYNTHETIC_TEST_INPUT" }, null, null);
        var created = await extension.CreateOrUpdate(specification, cancellationToken);
        var previewed = await extension.Preview(specification, cancellationToken);
        var retrieved = await extension.Get(new("request", "v1", new JsonObject(), null), cancellationToken);
        var deleted = await extension.Delete(new("request", "v1", new JsonObject(), null), cancellationToken);
        var types = await extension.GetTypeFiles(TestContext.CancellationTokenSource.Token);

        LocalExtensionOperationResponse[] responses = [created, previewed, retrieved, deleted];
        foreach (var response in responses)
        {
            response.ErrorData.Should().BeNull();
            response.Resource.Should().NotBeNull();
            response.Resource?.Properties["value"]?.GetValue<string>().Should().Be("LEGITIMATE_TEST_OUTPUT");
        }
        types.IndexFileContent.Should().Be("""{"resources":{}}""");
        var endpoint = await ReadEndpoint(binaryPath);
        endpoint.Arguments.Should().Equal(
            OperatingSystem.IsWindows() ? "--pipe" : "--socket",
            endpoint.Pipe ?? endpoint.Socket);
    }

    [TestMethod]
    public async Task Synchronous_host_authenticates_with_delayed_bootstrap_without_capturing_the_callers_context()
    {
        using var authentication = LocalExtensionAuthentication.Create();
        await RunWithAuthenticatedChild(authentication, async endpoint =>
        {
            using var channel = CreateUnauthenticatedChannel(endpoint);
            var client = new Rpc.BicepExtension.BicepExtensionClient(channel);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            Func<Task> ping = async () => await client.PingAsync(new(), cancellationToken: timeout.Token);

            await ping.Should().ThrowAsync<RpcException>().Where(exception =>
                exception.StatusCode == StatusCode.Unavailable || exception.StatusCode == StatusCode.Unauthenticated ||
                exception.StatusCode == StatusCode.PermissionDenied);
        }, hostName: "sync-context-host");
    }

    [TestMethod]
    public async Task Bootstrap_shares_only_the_public_client_identity_and_uses_fresh_launch_keys()
    {
        using var first = LocalExtensionAuthentication.Create();
        using var second = LocalExtensionAuthentication.Create();
        using var bootstrap = new MemoryStream();
        await first.WriteBootstrapAsync(bootstrap, TestContext.CancellationTokenSource.Token);
        bootstrap.Position = 0;
        using var child = await LocalExtensionAuthentication.ReadBootstrapAsync(bootstrap, TestContext.CancellationTokenSource.Token);

        child.ServerCertificate.HasPrivateKey.Should().BeTrue();
        child.ClientCertificate.HasPrivateKey.Should().BeFalse();
        first.ClientCertificate.HasPrivateKey.Should().BeTrue();
        LocalExtensionAuthentication.MatchesCertificate(child.ServerCertificate, first.ServerCertificate).Should().BeTrue();
        LocalExtensionAuthentication.MatchesCertificate(child.ClientCertificate, first.ClientCertificate).Should().BeTrue();
        LocalExtensionAuthentication.MatchesCertificate(second.ServerCertificate, first.ServerCertificate).Should().BeFalse();
        LocalExtensionAuthentication.MatchesCertificate(second.ClientCertificate, first.ClientCertificate).Should().BeFalse();
    }

    [TestMethod]
    public async Task Windows_launch_keys_are_deleted_when_the_launch_is_disposed()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("CNG key cleanup is specific to Windows.");
            return;
        }

        var authentication = LocalExtensionAuthentication.Create();
        using var server = authentication.ServerCertificate.GetECDsaPrivateKey();
        using var client = authentication.ClientCertificate.GetECDsaPrivateKey();
        server.Should().BeOfType<ECDsaCng>();
        client.Should().BeOfType<ECDsaCng>();
        var serverName = ((ECDsaCng)server!).Key.KeyName;
        var clientName = ((ECDsaCng)client!).Key.KeyName;

        authentication.Dispose();

        CngKey.Exists(serverName!).Should().BeFalse();
        CngKey.Exists(clientName!).Should().BeFalse();
    }

    [TestMethod]
    [DataRow(0, 1, 1)]
    [DataRow(1, 0, 1)]
    [DataRow(1, 16385, 1)]
    [DataRow(1, 1, 16385)]
    public async Task Bootstrap_rejects_unsupported_versions_and_unbounded_lengths(int version, int serverLength, int clientLength)
    {
        var header = new byte[16];
        BinaryPrimitives.WriteInt32BigEndian(header, version);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), serverLength);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(8), clientLength);
        using var bootstrap = new MemoryStream(header);
        Func<Task> read = async () =>
        {
            using var authentication = await LocalExtensionAuthentication.ReadBootstrapAsync(bootstrap, TestContext.CancellationTokenSource.Token);
        };

        await read.Should().ThrowAsync<InvalidOperationException>().WithMessage("*bootstrap*");
    }

    [TestMethod]
    public async Task Cancelled_bootstrap_clears_server_bytes_written_after_cancellation()
    {
        var header = new byte[16];
        BinaryPrimitives.WriteInt32BigEndian(header, 1);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), 4);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(8), 1);
        using var bootstrap = new DelayedBootstrapStream(header);
        using var cancellation = new CancellationTokenSource();
        using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var read = LocalExtensionAuthentication.ReadBootstrapAsync(bootstrap, cancellation.Token);
        await bootstrap.ServerReadStarted.WaitAsync(deadline.Token);

        try
        {
            await cancellation.CancelAsync();
            Func<Task> complete = async () => await read.WaitAsync(deadline.Token);
            await complete.Should().ThrowAsync<OperationCanceledException>();
            deadline.IsCancellationRequested.Should().BeFalse("startup must return before the pending read finishes");
        }
        finally
        {
            bootstrap.CompleteServerRead();
        }

        while (!bootstrap.ServerBufferCleared && !deadline.IsCancellationRequested)
        {
            await Task.Delay(10);
        }

        bootstrap.ServerBufferCleared.Should().BeTrue("the buffer must be wiped after the late write");
    }

    private sealed class DelayedBootstrapStream(byte[] header) : MemoryStream(header)
    {
        private readonly TaskCompletionSource serverReadStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<int> serverRead = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private Memory<byte> serverBuffer;

        public Task ServerReadStarted => serverReadStarted.Task;
        public bool ServerBufferCleared => serverBuffer.Span.IndexOfAnyExcept((byte)0) < 0;

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (Position < Length)
            {
                return base.ReadAsync(buffer, cancellationToken);
            }

            serverBuffer = buffer;
            serverReadStarted.SetResult();
            return new(serverRead.Task);
        }

        public void CompleteServerRead()
        {
            serverBuffer.Span.Fill(0xA5);
            serverRead.SetResult(serverBuffer.Length);
        }
    }

    [TestMethod]
    public async Task Bootstrap_rejects_trailing_data()
    {
        using var authentication = LocalExtensionAuthentication.Create();
        using var bootstrap = new MemoryStream();
        await authentication.WriteBootstrapAsync(bootstrap, TestContext.CancellationTokenSource.Token);
        bootstrap.WriteByte(1);
        bootstrap.Position = 0;
        Func<Task> read = async () =>
        {
            using var child = await LocalExtensionAuthentication.ReadBootstrapAsync(bootstrap, TestContext.CancellationTokenSource.Token);
        };

        await read.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Unexpected data*");
    }

    [TestMethod]
    [DataRow("missing-client")]
    [DataRow("wrong-client")]
    public async Task Sdk_host_rejects_missing_or_wrong_tls_identities(string identity)
    {
        using var expected = LocalExtensionAuthentication.Create();
        using var unrelated = identity == "wrong-client" ? LocalExtensionAuthentication.Create() : null;
        await RunWithAuthenticatedChild(expected, async endpoint =>
        {
            var handler = CreateTransportHandler(endpoint);
            handler.SslOptions = new()
            {
                TargetHost = "localhost",
                RemoteCertificateValidationCallback = (_, certificate, _, _) =>
                    LocalExtensionAuthentication.MatchesCertificate(certificate, expected.ServerCertificate),
            };
            if (unrelated is not null)
            {
                handler.SslOptions.ClientCertificates = [unrelated.ClientCertificate];
            }
            using var channel = GrpcChannel.ForAddress("https://localhost", new GrpcChannelOptions { HttpHandler = handler, DisposeHttpClient = true });
            var client = new Rpc.BicepExtension.BicepExtensionClient(channel);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            Func<Task> call = async () => await client.PingAsync(new(), cancellationToken: timeout.Token);

            var rejection = await call.Should().ThrowAsync<RpcException>();
            rejection.Which.StatusCode.Should().Be(StatusCode.Unavailable);
            using var goodChannel = CreateAuthenticatedChannel(endpoint, expected);
            using var controlTimeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            controlTimeout.CancelAfter(TimeSpan.FromSeconds(10));
            await new Rpc.BicepExtension.BicepExtensionClient(goodChannel).PingAsync(new(), cancellationToken: controlTimeout.Token);
        });
    }

    [TestMethod]
    [DataRow("startup")]
    [DataRow("single-process-disposal")]
    public async Task Process_exit_is_observed_while_a_descendant_holds_output_handles_open(string phase)
    {
        var binaryPath = PrepareHost(phase == "startup" ? "exit-with-inherited-output-host" : "inherited-output-host");
        var holderPath = Path.ChangeExtension(binaryPath, ".output-holder");
        using var launchCancellation = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        var launch = factory.Start(IOUri.FromFilePath(binaryPath), launchCancellation.Token);
        ILocalExtension? extension = null;
        Process? holder = null;
        Task? disposal = null;
        try
        {
            using var readyTimeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            readyTimeout.CancelAfter(TimeSpan.FromSeconds(10));
            await WaitForFile(holderPath, readyTimeout.Token);
            var holderId = await ReadProcessId(holderPath, readyTimeout.Token);
            holder = Process.GetProcessById(holderId);
            holder.HasExited.Should().BeFalse();

            if (phase == "startup")
            {
                Func<Task> connect = async () => extension = await launch.WaitAsync(TimeSpan.FromSeconds(5));
                await connect.Should().ThrowAsync<InvalidOperationException>()
                    .WithMessage("*exited with code 0 before becoming ready.*");
            }
            else
            {
                extension = await launch.WaitAsync(TimeSpan.FromSeconds(20));
                var endpoint = await ReadEndpoint(binaryPath);
                using var child = Process.GetProcessById(endpoint.ProcessId);
                disposal = extension.DisposeAsync().AsTask();
                await disposal.WaitAsync(TimeSpan.FromSeconds(5));
                child.HasExited.Should().BeTrue();
            }

            holder.HasExited.Should().BeFalse("exit observation and disposal must not depend on the descendant closing stdout/stderr");
        }
        finally
        {
            await launchCancellation.CancelAsync();
            if (holder is null && File.Exists(holderPath))
            {
                var holderId = await ReadProcessId(holderPath, TestContext.CancellationTokenSource.Token);
                holder = Process.GetProcessById(holderId);
            }
            if (holder is not null)
            {
                using (holder)
                {
                    if (!holder.HasExited)
                    {
                        holder.Kill(entireProcessTree: true);
                    }
                    await holder.WaitForExitAsync(TestContext.CancellationTokenSource.Token);
                }
            }
            if (extension is null)
            {
                try
                {
                    extension = await launch;
                }
                catch (Exception exception) when (exception is InvalidOperationException or OperationCanceledException)
                {
                    TestContext.WriteLine($"Expected launch failure during cleanup: {exception.Message}");
                }
            }
            if (disposal is not null)
            {
                await disposal;
            }
            if (extension is not null)
            {
                await extension.DisposeAsync();
            }
        }
    }

    [TestMethod]
    [DataRow("kill-failure")]
    [DataRow("exit-timeout")]
    public async Task Failed_shutdown_releases_resources_without_manufacturing_process_exit(string failure)
    {
        var binaryPath = PrepareHost("stall-host");
        using var child = Process.Start(new ProcessStartInfo
        {
            FileName = binaryPath,
            UseShellExecute = false,
        }) ?? throw new InvalidOperationException("Failed to start the shutdown fixture.");
        using var monitor = Process.GetProcessById(child.Id);
        using var authentication = LocalExtensionAuthentication.Create();
        var exited = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var killAttempted = false;
        var shutdown = GrpcLocalExtension.TerminateProcess(
            IOUri.FromFilePath(binaryPath), child, null, authentication, exited.Task, null,
            () =>
            {
                killAttempted = true;
                if (failure == "kill-failure")
                {
                    throw new System.ComponentModel.Win32Exception("Synthetic termination failure.");
                }
            }, TimeSpan.Zero);
        try
        {
            await shutdown.WaitAsync(TimeSpan.FromSeconds(5));

            killAttempted.Should().BeTrue();
            monitor.HasExited.Should().BeFalse("failed or timed-out termination must not pretend the child exited");
            exited.Task.IsCompleted.Should().BeFalse("a failed kill or exit timeout is not an OS exit notification");
            authentication.ServerCertificate.Handle.Should().Be(IntPtr.Zero);
            authentication.ClientCertificate.Handle.Should().Be(IntPtr.Zero);
            exited.TrySetResult().Should().BeTrue("late exit must remain safe after parent resources are disposed");
            await exited.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally
        {
            exited.TrySetResult();
            if (!monitor.HasExited)
            {
                monitor.Kill();
            }
            await monitor.WaitForExitAsync(TestContext.CancellationTokenSource.Token);
            await shutdown;
        }
    }

    [TestMethod]
    public void Credential_disposal_failure_does_not_skip_other_resources_or_replace_the_original_error()
    {
        var credential = new Mock<IDisposable>(MockBehavior.Strict);
        credential.Setup(instance => instance.Dispose())
            .Throws(new CryptographicException("Synthetic credential cleanup failure."));
        var lifetime = new Mock<IDisposable>(MockBehavior.Strict);
        lifetime.Setup(instance => instance.Dispose());
        var original = new InvalidOperationException("Original authentication failure.");
        Action fail = () =>
        {
            try
            {
                throw original;
            }
            finally
            {
                GrpcLocalExtension.DisposeResources(
                    IOUri.FromFilePath(Path.Combine(Path.GetTempPath(), "synthetic-extension")), credential.Object, lifetime.Object);
            }
        };

        fail.Should().Throw<InvalidOperationException>().Which.Should().BeSameAs(original);
        credential.Verify(instance => instance.Dispose(), Times.Once);
        lifetime.Verify(instance => instance.Dispose(), Times.Once);
    }

    [TestMethod]
    public async Task Disposal_terminates_child_and_closes_its_endpoint()
    {
        var binaryPath = PrepareHost("sdk-host");
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        var extension = await factory.Start(IOUri.FromFilePath(binaryPath));
        var endpoint = await ReadEndpoint(binaryPath);
        using var child = Process.GetProcessById(endpoint.ProcessId);

        await extension.DisposeAsync();
        await extension.DisposeAsync();

        child.HasExited.Should().BeTrue();
        if (endpoint.Socket is { } socket)
        {
            File.Exists(socket).Should().BeFalse();
        }
        Func<Task> call = async () => await extension.GetTypeFiles(TestContext.CancellationTokenSource.Token);
        await call.Should().ThrowAsync<ObjectDisposedException>();
    }

    [TestMethod]
    public async Task Factory_reports_the_cause_when_the_executable_is_missing()
    {
        var binaryPath = Path.Combine(FileHelper.GetUniqueTestOutputPath(TestContext), "missing-extension");
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        Func<Task> connect = async () =>
        {
            await using var extension = await factory.Start(IOUri.FromFilePath(binaryPath));
        };

        var failure = await connect.Should().ThrowAsync<InvalidOperationException>().WithMessage("Failed to start extension *");
        var cause = failure.Which.InnerException.Should().BeOfType<System.ComponentModel.Win32Exception>().Which;
        failure.Which.Message.Should().Contain(cause.Message).And.NotContainEquivalentOf("republish");
    }

    [TestMethod]
    public async Task Factory_fails_promptly_when_child_exits_during_startup()
    {
        var binaryPath = PrepareHost("exit-host");
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        Func<Task> connect = async () =>
        {
            await using var extension = await factory.Start(IOUri.FromFilePath(binaryPath));
        };

        var failure = await connect.Should().ThrowAsync<InvalidOperationException>().WithMessage("*exited with code 0 before becoming ready.*");
        failure.Which.Message.Should().NotContainEquivalentOf("republish");
        var endpoint = await ReadEndpoint(binaryPath);
        Action findChild = () => Process.GetProcessById(endpoint.ProcessId).Dispose();
        findChild.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public async Task Factory_reports_startup_timeout_and_cleans_up_the_child()
    {
        var binaryPath = PrepareHost("stall-host");
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        Func<Task> connect = async () =>
        {
            // Leave headroom beyond the 30-second startup deadline and 15-second cleanup budget.
            await using var extension = await factory.Start(IOUri.FromFilePath(binaryPath))
                .WaitAsync(TimeSpan.FromSeconds(60));
        };

        var failure = await connect.Should().ThrowAsync<InvalidOperationException>().WithMessage("*did not become ready within 30 seconds.*");
        failure.Which.Message.Should().NotContainEquivalentOf("republish");
        var endpoint = await ReadEndpoint(binaryPath);
        Action findChild = () => Process.GetProcessById(endpoint.ProcessId).Dispose();
        findChild.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Cancelled_startup_never_returns_an_extension_and_cleans_up_the_child(bool cancelBeforeLaunch)
    {
        var binaryPath = PrepareHost("stall-host");
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        using var cancelled = new CancellationTokenSource();
        if (cancelBeforeLaunch)
        {
            await cancelled.CancelAsync();
        }
        var launch = factory.Start(IOUri.FromFilePath(binaryPath), cancelled.Token);
        var endpointPath = Path.ChangeExtension(binaryPath, ".endpoint.json");
        if (!cancelBeforeLaunch)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            while (!File.Exists(endpointPath))
            {
                await Task.Delay(50, timeout.Token);
            }
            await cancelled.CancelAsync();
        }
        var rejected = false;
        try
        {
            await launch;
        }
        catch (OperationCanceledException)
        {
            rejected = true;
        }

        rejected.Should().BeTrue("cancelling startup must reject the launch");
        if (cancelBeforeLaunch)
        {
            File.Exists(endpointPath).Should().BeFalse();
        }
        else
        {
            var endpoint = await ReadEndpoint(binaryPath);
            Action findChild = () => Process.GetProcessById(endpoint.ProcessId).Dispose();
            findChild.Should().Throw<ArgumentException>();
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Production_channel_revalidates_launch_identity_on_reconnect(bool replaceServer)
    {
        using var expected = LocalExtensionAuthentication.Create();
        using var different = LocalExtensionAuthentication.Create();
        await RunWithAuthenticatedChild(expected, async endpoint =>
        {
            using var channel = CreateAuthenticatedChannel(endpoint, expected);
            var client = new Rpc.BicepExtension.BicepExtensionClient(channel);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));

            await client.PingAsync(new(), cancellationToken: timeout.Token);
            using var firstChild = Process.GetProcessById(endpoint.ProcessId);
            firstChild.Kill(entireProcessTree: true);
            await firstChild.WaitForExitAsync(timeout.Token);
            if (endpoint.Socket is { } socket && File.Exists(socket))
            {
                File.Delete(socket);
            }

            await RunWithAuthenticatedChild(replaceServer ? different : expected, async _ =>
            {
                using var reconnectTimeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
                reconnectTimeout.CancelAfter(TimeSpan.FromSeconds(10));
                if (replaceServer)
                {
                    Func<Task> call = async () => await client.PingAsync(new(), cancellationToken: reconnectTimeout.Token);
                    var rejection = await call.Should().ThrowAsync<RpcException>();
                    ContainsAuthenticationFailure(rejection.Which).Should().BeTrue();
                }
                else
                {
                    await client.PingAsync(new(), cancellationToken: reconnectTimeout.Token);
                }
            }, endpoint);
        });
    }

    private async Task RunWithAuthenticatedChild(LocalExtensionAuthentication authentication, Func<Endpoint, Task> test, Endpoint? listenEndpoint = null, string hostName = "sdk-host")
    {
        var binaryPath = PrepareHost(hostName);
        var endpoint = listenEndpoint ?? (OperatingSystem.IsWindows()
            ? new Endpoint(null, Guid.NewGuid().ToString("N"), 0)
            : new Endpoint(Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.sock"), null, 0));
        using var child = new Process
        {
            StartInfo = new()
            {
                FileName = binaryPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
        };
        child.StartInfo.ArgumentList.Add(endpoint.Pipe is null ? "--socket" : "--pipe");
        child.StartInfo.ArgumentList.Add(endpoint.Pipe ?? endpoint.Socket ?? throw new InvalidOperationException("Missing endpoint."));
        child.Start().Should().BeTrue();
        var output = child.StandardOutput.ReadToEndAsync();
        var error = child.StandardError.ReadToEndAsync();
        try
        {
            if (hostName == "sync-context-host")
            {
                using var readyTimeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
                readyTimeout.CancelAfter(TimeSpan.FromSeconds(10));
                var readyPath = Path.Combine(Path.GetDirectoryName(binaryPath) ?? throw new InvalidOperationException("Missing test output directory."), $"{hostName}.bootstrap-ready");
                await WaitForFile(readyPath, readyTimeout.Token);
                await Task.Delay(200, readyTimeout.Token);
            }
            await authentication.WriteBootstrapAsync(child.StandardInput.BaseStream, TestContext.CancellationTokenSource.Token);
            child.StandardInput.Close();
            using var channel = CreateAuthenticatedChannel(endpoint, authentication);
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
            timeout.CancelAfter(TimeSpan.FromSeconds(10));
            await GrpcChannelHelper.WaitForConnectionAsync(new(channel), timeout.Token);
            await test(endpoint with { ProcessId = child.Id });
        }
        finally
        {
            if (!child.HasExited)
            {
                child.Kill(entireProcessTree: true);
                await child.WaitForExitAsync();
            }
            TestContext.WriteLine(await output);
            TestContext.WriteLine(await error);
            if (endpoint.Socket is { } socket && File.Exists(socket))
            {
                File.Delete(socket);
            }
        }
    }

    private static GrpcChannel CreateAuthenticatedChannel(Endpoint endpoint, LocalExtensionAuthentication authentication)
        => endpoint.Socket is { } socket
            ? GrpcChannelHelper.CreateDomainSocketChannel(socket, authentication)
            : GrpcChannelHelper.CreateNamedPipeChannel(endpoint.Pipe ?? throw new InvalidOperationException("Missing test pipe."), authentication);

    private static bool ContainsAuthenticationFailure(Exception exception)
        => exception is AuthenticationException ||
            (exception.InnerException is { } inner && ContainsAuthenticationFailure(inner));

    [TestMethod]
    public async Task Factory_rejects_plaintext_extension_with_rebuild_and_republish_guidance()
    {
        var binaryPath = PrepareHost("plaintext-host");
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        Func<Task> connect = async () =>
        {
            await using var extension = await factory.Start(IOUri.FromFilePath(binaryPath));
        };

        var failure = await connect.Should().ThrowAsync<InvalidOperationException>(
            "answering Ping without the launch identity must not authenticate an extension");
        TestContext.WriteLine($"Legacy extension launch error: {failure.Which.Message}");
        VerifyLegacyHostGuidance(failure.Which);
    }

    private static void VerifyLegacyHostGuidance(InvalidOperationException exception)
    {
        exception.Message.Should().Contain("Bicep requires an authenticated extension host.");
        exception.Message.Should().Contain("If this extension was built with an older SDK");
        exception.Message.Should().Contain("rebuild and republish the extension");
        exception.Message.Should().Contain("update your extension reference and restore it");
    }

    [TestMethod]
    [DataRow("sdk-host", false)]
    [DataRow("sync-host", true)]
    public async Task Sdk_host_rejects_unauthenticated_callers(string hostName, bool resourceCall)
    {
        var binaryPath = PrepareHost(hostName);
        ILocalExtensionFactory factory = new GrpcLocalExtensionFactory();
        await using var extension = await factory.Start(IOUri.FromFilePath(binaryPath));
        var endpoint = await ReadEndpoint(binaryPath);
        using var channel = CreateUnauthenticatedChannel(endpoint);
        var client = new Rpc.BicepExtension.BicepExtensionClient(channel);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationTokenSource.Token);
        timeout.CancelAfter(TimeSpan.FromSeconds(10));

        Func<Task> invoke = async () =>
        {
            if (resourceCall)
            {
                await client.CreateOrUpdateAsync(CreateRpcSpecification(), cancellationToken: timeout.Token);
            }
            else
            {
                await client.PingAsync(new(), cancellationToken: timeout.Token);
            }
        };

        await invoke.Should().ThrowAsync<RpcException>().Where(exception =>
            exception.StatusCode == StatusCode.Unavailable ||
            exception.StatusCode == StatusCode.Unauthenticated ||
            exception.StatusCode == StatusCode.PermissionDenied);
    }

    [TestMethod]
    [DataRow("sdk-host", "", false)]
    [DataRow("sdk-host", "not-a-valid-bootstrap-document", false)]
    [DataRow("sync-host", "", true)]
    public async Task Sdk_host_exits_when_bootstrap_is_missing_or_invalid(string hostName, string bootstrap, bool keepInputOpen)
    {
        var binaryPath = PrepareHost(hostName);
        using var process = new Process
        {
            StartInfo = new()
            {
                FileName = binaryPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
        };
        var socketPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.sock");
        process.StartInfo.ArgumentList.Add(OperatingSystem.IsWindows() ? "--pipe" : "--socket");
        process.StartInfo.ArgumentList.Add(OperatingSystem.IsWindows() ? Guid.NewGuid().ToString("N") : socketPath);
        process.Start().Should().BeTrue();
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        var testCancellation = TestContext.CancellationTokenSource.Token;
        var phase = "waiting for the child to start reading bootstrap";

        try
        {
            using var readyTimeout = CancellationTokenSource.CreateLinkedTokenSource(testCancellation);
            readyTimeout.CancelAfter(TimeSpan.FromSeconds(20));
            await WaitForFile(Path.ChangeExtension(binaryPath, ".bootstrap-ready"), readyTimeout.Token);
            phase = "waiting for the child to reject bootstrap";
            using var rejectionTimeout = CancellationTokenSource.CreateLinkedTokenSource(testCancellation);
            rejectionTimeout.CancelAfter(TimeSpan.FromSeconds(45));
            await process.StandardInput.WriteAsync(bootstrap.AsMemory(), rejectionTimeout.Token);
            if (!keepInputOpen)
            {
                process.StandardInput.Close();
            }
            var rejectedPath = Path.ChangeExtension(binaryPath, ".bootstrap-rejected");
            if (keepInputOpen)
            {
                await Task.Delay(TimeSpan.FromSeconds(11), rejectionTimeout.Token);
                File.Exists(rejectedPath).Should().BeFalse("the bootstrap should remain open beyond the previous 10-second timeout");
            }
            await WaitForFile(rejectedPath, rejectionTimeout.Token);
            phase = "waiting for the child to exit after rejecting bootstrap";
            using var exitTimeout = CancellationTokenSource.CreateLinkedTokenSource(testCancellation);
            exitTimeout.CancelAfter(TimeSpan.FromSeconds(15));
            await process.WaitForExitAsync(exitTimeout.Token);
            process.ExitCode.Should().NotBe(0);
            var rejection = await ReadMarker(rejectedPath, testCancellation);
            TestContext.WriteLine(rejection);
            if (keepInputOpen)
            {
                rejection.Should().MatchRegex(@"\A(?:TaskCanceledException|OperationCanceledException)\n");
            }
            else
            {
                var exceptionName = bootstrap.Length == 0 ? nameof(EndOfStreamException) : nameof(InvalidOperationException);
                rejection.Should().StartWith($"{exceptionName}\n");
            }
        }
        catch (OperationCanceledException ex) when (!testCancellation.IsCancellationRequested)
        {
            throw new TimeoutException($"{hostName}: test timed out while {phase}.", ex);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync();
            }

            TestContext.WriteLine(await output);
            TestContext.WriteLine(await error);
            if (!OperatingSystem.IsWindows() && File.Exists(socketPath))
            {
                File.Delete(socketPath);
            }
        }
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task Only_authenticated_output_reaches_nested_arm_deployment(bool authenticated)
    {
        var binaryPath = PrepareHost(authenticated ? "sdk-host" : "plaintext-host");
        var architecture = SupportedArchitectures.TryGetCurrent() ?? throw new InvalidOperationException("Unsupported test architecture.");
        var package = new ExtensionArtifactPackage(
            ExtensionResourceTypeHelper.GetHttpExtensionTypesTgz(),
            [new ExtensionBinary(architecture, BinaryData.FromBytes([]))]);
        var services = await ExtensionTestHelper.GetServiceBuilderWithPublishedExtension(package, new(LocalDeployEnabled: true));
        var compilation = await RestoreAndCompileParams(services,
            ("bicepconfig.json", """
                {
                  "extensions": {
                    "http": "br:example.azurecr.io/extensions/foo:1.2.3"
                  },
                  "experimentalFeaturesEnabled": { "localDeploy": true }
                }
                """),
            ("main.bicep", """
                targetScope = 'local'
                extension http

                @secure()
                param input string

                resource result 'request@v1' = {
                  uri: input
                }

                module cloud './cloud.bicep' = {
                  name: 'test-deployment'
                  scope: resourceGroup('11111111-1111-1111-1111-111111111111', 'test-resource-group')
                  params: {
                    value: string(result.body)
                  }
                }
                """),
            ("cloud.bicep", """
                targetScope = 'resourceGroup'
                param value string
                output value string = value
                """),
            ("parameters.bicepparam", """
                using './main.bicep'
                param input = 'SYNTHETIC_SECURE_TEST_INPUT'
                """));
        compilation.Should().NotHaveAnyDiagnostics();

        ILocalExtensionFactory realFactory = new GrpcLocalExtensionFactory();
        var factory = StrictMock.Of<ILocalExtensionFactory>();
        factory.Setup(instance => instance.Start(It.IsAny<IOUri>(), It.IsAny<CancellationToken>()))
            .Returns<IOUri, CancellationToken>((_, token) => realFactory.Start(IOUri.FromFilePath(binaryPath), token));
        var recordedParameters = new List<string>();
        var arm = StrictMock.Of<IArmDeploymentProvider>();
        arm.Setup(instance => instance.StartDeployment(
                It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<IBicepConfiguration, DeploymentLocator, string, string, CancellationToken>(
                (_, _, _, parameters, _) => recordedParameters.Add(parameters))
            .Returns(Task.CompletedTask);
        arm.Setup(instance => instance.CheckDeployment(
                It.IsAny<IBicepConfiguration>(), It.IsAny<DeploymentLocator>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LocalDeploymentResult(
                new DeploymentContent { Properties = new() { ProvisioningState = ProvisioningState.Succeeded } },
                [],
                []));
        var serviceProvider = services.Build().Construct<IServiceProvider>();
        await using var dispatcher = new LocalExtensionDispatcher(
            serviceProvider.GetRequiredService<IBicepConfigurationManager>(), factory.Object, arm.Object);

        Func<Task> deploy = async () =>
        {
            await dispatcher.InitializeExtensions(compilation.Compilation, TestContext.CancellationTokenSource.Token);
            var result = await dispatcher.Deploy(
                compilation.Compilation.Emitter.Parameters().Template!.Template!,
                compilation.Compilation.Emitter.Parameters().Parameters!,
                _ => { },
                TestContext.CancellationTokenSource.Token);

            result.Deployment.Properties.ProvisioningState.Should().Be(ProvisioningState.Succeeded);
        };

        if (authenticated)
        {
            await deploy();
            recordedParameters.Should().ContainSingle();
            var parameters = JsonNode.Parse(recordedParameters[0]);
            parameters?["parameters"]?["value"]?["value"]?.GetValue<string>().Should().Be("LEGITIMATE_TEST_OUTPUT");
        }
        else
        {
            await deploy.Should().ThrowAsync<InvalidOperationException>();
            recordedParameters.Should().BeEmpty("unverified extension output must not become an authenticated ARM deployment input");
        }
    }

    private static async Task WaitForFile(string path, CancellationToken cancellationToken)
    {
        while (!File.Exists(path))
        {
            await Task.Delay(50, cancellationToken);
        }
    }

    private static async Task<int> ReadProcessId(string path, CancellationToken cancellationToken)
    {
        var contents = await ReadMarker(path, cancellationToken);
        return int.Parse(contents);
    }

    private static async Task<string> ReadMarker(string path, CancellationToken cancellationToken)
    {
        // A published marker can be visible while Windows is still completing its rename.
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private string PrepareHost(string hostName)
    {
        var source = Path.Combine(AppContext.BaseDirectory, "GrpcSecurityTestHost");
        Directory.Exists(source).Should().BeTrue("the test-only child host must be built and copied before running tests");
        var target = FileHelper.GetUniqueTestOutputPath(TestContext);
        Directory.CreateDirectory(target);

        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            var destination = Path.Combine(target, Path.GetRelativePath(source, file));
            Directory.CreateDirectory(Path.GetDirectoryName(destination) ?? throw new InvalidOperationException("Missing test output directory."));
            File.Copy(file, destination);
        }

        var suffix = OperatingSystem.IsWindows() ? ".exe" : "";
        var appHost = Path.Combine(target, $"Bicep.Local.Deploy.IntegrationTests.TestHost{suffix}");
        var binaryPath = Path.Combine(target, $"{hostName}{suffix}");
        File.Copy(appHost, binaryPath);
        if (!OperatingSystem.IsWindows())
        {
            File.SetUnixFileMode(binaryPath, File.GetUnixFileMode(appHost));
        }

        return binaryPath;
    }

    private async Task<Endpoint> ReadEndpoint(string binaryPath)
    {
        var path = Path.ChangeExtension(binaryPath, ".endpoint.json");
        var contents = await ReadMarker(path, TestContext.CancellationTokenSource.Token);
        return JsonSerializer.Deserialize<Endpoint>(contents) ?? throw new InvalidOperationException("Missing test endpoint.");
    }

    private static GrpcChannel CreateUnauthenticatedChannel(Endpoint endpoint)
    {
        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = CreateTransportHandler(endpoint),
            DisposeHttpClient = true,
        });
    }

    private static SocketsHttpHandler CreateTransportHandler(Endpoint endpoint)
        => new()
        {
            ConnectCallback = async (_, cancellationToken) =>
            {
                if (endpoint.Socket is { } socketPath)
                {
                    var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
                    try
                    {
                        await socket.ConnectAsync(new UnixDomainSocketEndPoint(socketPath), cancellationToken);
                        return new NetworkStream(socket, ownsSocket: true);
                    }
                    catch
                    {
                        socket.Dispose();
                        throw;
                    }
                }

                var pipe = new NamedPipeClientStream(".", endpoint.Pipe ?? throw new InvalidOperationException("Missing test pipe."), PipeDirection.InOut, PipeOptions.Asynchronous);
                try
                {
                    await pipe.ConnectAsync(cancellationToken);
                    return pipe;
                }
                catch
                {
                    await pipe.DisposeAsync();
                    throw;
                }
            },
        };

    private static Rpc.ResourceSpecification CreateRpcSpecification()
        => new() { Type = "request", ApiVersion = "v1", Properties = """{"input":"SYNTHETIC_TEST_INPUT"}""" };
}
