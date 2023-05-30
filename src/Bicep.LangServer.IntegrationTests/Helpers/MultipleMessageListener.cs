// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bicep.LangServer.IntegrationTests
{
    /// <summary>
    /// Helper class to read messages in order and assert things about them.
    /// </summary>
    public class MultipleMessageListener<T>
    {
        private const int DefaultTimeout = 30000;

        private readonly object lockObj = new();
        private readonly List<TaskCompletionSource<T>> completionSources = new();

        private int listenPosition = 0;
        private int writePosition = 0;

        public async Task<T> WaitNext(int timeout = DefaultTimeout)
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

        public async Task<List<T>> WaitForAll(int timeout = DefaultTimeout)
        {
            List<T> onMessageTasks = new List<T>();

            foreach (var completionSource in completionSources)
            {
                var onMessageTask = completionSource.Task;
                var response = await IntegrationTestHelper.WithTimeoutAsync(onMessageTask, timeout);
                onMessageTasks.Add(response);
            }

            return onMessageTasks;
        }

        public async Task EnsureNoMessageSent(int timeout = DefaultTimeout)
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
