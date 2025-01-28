// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Threading;

namespace Bicep.Core.Registry.Catalog.Implementation;

public class MightBeLazyAsync<T> where T : class
{
    private readonly AsyncLazy<T>? lazy;
    private readonly T? value;

    public MightBeLazyAsync(T value)
    {
        lazy = null;
        this.value = value;
    }

    public MightBeLazyAsync(Func<Task<T>> initializer)
    {
        lazy = new AsyncLazy<T>(initializer, new(new JoinableTaskContext()));
        value = default;
    }

    private bool IsLazy => lazy is not null;

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        if (IsLazy)
        {
            if (lazy!.IsValueFactoryCompleted)
            {
                value = lazy.GetValue(); //asdfg can this throw?//asdfg testpoint
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        else
        {
            value = this.value!;
            return true;
        }
    }

    public async Task<T> GetValueAsync()
    {
        return IsLazy ? await lazy!.GetValueAsync() : value!;
    }
}
