// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Json;

namespace Bicep.Core.Configuration
{
    public abstract class ConfigurationSection<T>
    {
        protected ConfigurationSection(T data)
        {
            this.Data = data;
        }

        public T Data { get; }

        public virtual void WriteTo(Utf8JsonWriter writer) => JsonElementFactory.CreateElement(this.Data).WriteTo(writer);
    }
}
