// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Core.Configuration
{
    public abstract class ConfigurationSection<T>
    {
        protected ConfigurationSection(T data)
        {
            this.Data = data;
        }

        protected T Data { get; }
    }
}
