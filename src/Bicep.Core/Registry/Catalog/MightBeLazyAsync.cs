// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.VisualStudio.Threading;

namespace Bicep.Core.Registry.Catalog;

public class MightBeLazyAsync<T>
{
    private readonly AsyncLazy<T>? lazy;
    private readonly T? value;

    public MightBeLazyAsync(T value)
    {
        this.lazy = null;
        this.value = value;
    }

    public MightBeLazyAsync(Func<Task<T>> initializer)
    {
        this.lazy = new AsyncLazy<T>(initializer, new(new JoinableTaskContext()));
        this.value = default;
    }

    private bool IsLazy => lazy is not null;

    //asdfg public bool HasValue => lazy is null || lazy.IsValueFactoryCompleted;

    public T? TryGetValue()
    {
        if (IsLazy)
        {
            if (lazy!.IsValueFactoryCompleted)
            {
                return lazy.GetValue(); //asdfg can this throw?
            }
            else
            {
                return default;
            }
        }
        else
        {
            return value;
        }
    }

    public async Task<T> GetValueAsync()
    {
        return IsLazy ? await lazy!.GetValueAsync() : value!;
    }
}
