// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;

namespace Bicep.IO.UnitTests.InMemory
{
    [TestClass]
    public class InMemoryFileLockTests
    {
        [TestMethod]
        public void Dispose_AfterLockAcquired_AllowsNewLock()
        {
            // Arrange
            var fileHandle = CreateTestFileHandle("/lockedfile.txt");

            // Act: lock and then dispose
            using (new InMemoryFileLock(fileHandle))
            {
                // still locked here
            }

            // Assert: second lock creation should succeed without blocking
            Action act = () =>
            {
                using var secondLock = new InMemoryFileLock(fileHandle);
            };

            act.Should().NotThrow("because disposing the first lock should free the mutex for a second lock");
        }

        [TestMethod]
        public void SecondLock_OnSameFile_BlocksUntilFirstIsDisposed()
        {
            var fileHandle = CreateTestFileHandle("/lockedfile.txt");
            var firstLock = new InMemoryFileLock(fileHandle);

            var secondLockStarted = new ManualResetEvent(false);
            var secondLockAcquired = new ManualResetEvent(false);

            // Start the second lock attempt on a separate thread
            var lockThread = new Thread(() =>
            {
                secondLockStarted.Set(); // Signal that the thread has started
                using var secondLock = new InMemoryFileLock(fileHandle);
                secondLockAcquired.Set(); // Signal that the lock was acquired
            });

            lockThread.Start();

            // Wait for the second lock thread to start
            secondLockStarted.WaitOne(500);

            // The lock should still be held by the first instance, so second lock shouldn't be acquired yet
            Assert.IsFalse(secondLockAcquired.WaitOne(300), "Second lock should be blocked");

            // Dispose the first lock to allow the second lock to proceed
            firstLock.Dispose();

            // Wait for the second lock to be acquired
            Assert.IsTrue(secondLockAcquired.WaitOne(2000), "Second lock should be acquired after first lock is released");

            // Ensure the second lock thread exits
            lockThread.Join();
        }

        /// <summary>
        /// Creates a simple InMemoryFileHandle for testing by providing a 
        /// minimal FileStore and an "inmemory" URI for the specified file path.
        /// </summary>
        private static InMemoryFileHandle CreateTestFileHandle(string filePath)
        {
            var store = new InMemoryFileExplorer.FileStore();
            var uri = new IOUri("inmemory", "", filePath);
            return new InMemoryFileHandle(store, uri);
        }
    }
}
