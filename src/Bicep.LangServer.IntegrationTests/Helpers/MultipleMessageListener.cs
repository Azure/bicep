// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Bicep.LangServer.IntegrationTests
{
    // helper class to read messages in order and assert things about them
    [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "Test code does not need to follow this convention.")]
    public class MultipleMessageListener<T>
    {
        private object lockObj = new object();
        private int listenPosition = 0;
        private int writePosition = 0;
        private List<TaskCompletionSource<T>> completionSources = new List<TaskCompletionSource<T>>();

        public async Task<T> WaitNext(int timeout = 10000)
        {
            Task<T> onMessageTask;
            lock (lockObj)
            {
                while (listenPosition >= completionSources.Count)
                {
                    completionSources.Add(new TaskCompletionSource<T>());
                }

                onMessageTask = completionSources[listenPosition].Task;
                listenPosition++;
            }

            return await IntegrationTestHelper.WithTimeoutAsync(onMessageTask, timeout);
        }

        public async Task EnsureNoMessageSent(int timeout = 10000)
        {
            Task<T> onMessageTask;
            lock (lockObj)
            {
                while (listenPosition >= completionSources.Count)
                {
                    completionSources.Add(new TaskCompletionSource<T>());
                }

                onMessageTask = completionSources[listenPosition].Task;
            }

            await IntegrationTestHelper.EnsureTaskDoesntCompleteAsync(onMessageTask, timeout);
        }

        public void AddMessage(T message)
        {
            lock (lockObj)
            {
                while (writePosition >= completionSources.Count)
                {
                    completionSources.Add(new TaskCompletionSource<T>());
                }
                
                completionSources[writePosition].SetResult(message);
                writePosition++;
            }
        }
    }
}
