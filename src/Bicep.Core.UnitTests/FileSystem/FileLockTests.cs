// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Azure.Deployments.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Bicep.Core.UnitTests.FileSystem
{
    [TestClass]
    public class FileLockTests
    {
        [NotNull]
        public TestContext? TestContext {  get; set; }

        [TestMethod]
        public async Task ConcurrentRequestsShouldBeSerializedByFileLock()
        {
            var list = new List<int>();
            string lockDir = FileHelper.GetUniqueTestOutputPath(this.TestContext);
            Directory.CreateDirectory(lockDir);
            var fileName = Path.Combine(lockDir, "locktest");

            static void Append(List<int> list, string fileName)
            {
                var @lock = TryAcquireWithTimeout(fileName);
                @lock.Should().NotBeNull();
                using (@lock)
                {
                    if (list.Count == 0)
                    {
                        list.Add(0);
                        return;
                    }

                    list.Add(list.Last() + 1);
                }
            }

            const int ConcurrentTasks = 50;

            var tasks = new List<Task>();
            for(int i = 0; i < ConcurrentTasks; i++)
            {
                tasks.Add(Task.Run(() => Append(list, fileName)));
            }

            await Task.WhenAll(tasks);

            var expectedValues = new int[ConcurrentTasks];
            for(int i = 0; i < ConcurrentTasks; i++)
            {
                expectedValues[i] = i;
            }

            list.Should().Equal(expectedValues);
        }

        [TestMethod]
        public void FileLockShouldNotThrowIfLockFileIsDeleted()
        {
            string lockDir = FileHelper.GetUniqueTestOutputPath(this.TestContext);
            Directory.CreateDirectory(lockDir);
            var fileName = Path.Combine(lockDir, "locktest");

            var @lock = FileLock.TryAcquire(fileName);
            @lock.Should().NotBeNull();
            using (@lock)
            {
#if WINDOWS_BUILD
                FluentActions.Invoking(() => File.Delete(fileName)).Should().Throw<IOException>();
#else
                // delete will succeed on Linux and Mac due to advisory nature of locks there
                File.Delete(fileName);
#endif
            }
        }

        [TestMethod]
        public void CallingDisposeTwiceShouldNotThrow()
        {
            string lockDir = FileHelper.GetUniqueTestOutputPath(this.TestContext);
            Directory.CreateDirectory(lockDir);
            var fileName = Path.Combine(lockDir, "locktest");

            using (var @lock = FileLock.TryAcquire(fileName))
            {
                @lock.Should().NotBeNull();
                // extra call to Dispose() is intentional
                @lock!.Dispose();
            }
        }

        private static FileLock? TryAcquireWithTimeout(string name)
        {
            var sw = Stopwatch.StartNew();

            TimeSpan acquireTimeout = TimeSpan.FromSeconds(3);

            while (sw.Elapsed < acquireTimeout)
            {
                var acquired = FileLock.TryAcquire(name);
                if (acquired is not null)
                {
                    return acquired;
                }
            }

            return null;
        }
    }
}
