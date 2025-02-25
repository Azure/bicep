// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;

namespace Bicep.IO.InMemory
{
    public sealed class InMemoryFileLock : IFileLock
    {
        private readonly Mutex mutex;

        public InMemoryFileLock(string lockFileName)
        {
            this.mutex = new Mutex(false, lockFileName);
            this.mutex.WaitOne();
        }

        public void Dispose()
        {
            this.mutex.ReleaseMutex();
            this.mutex.Dispose();
        }
    }
}
