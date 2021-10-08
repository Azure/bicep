// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Json;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public abstract class ConfigurationSection<T>
    {
        protected ConfigurationSection(T data)
        {
            this.Data = data;
        }

        protected T Data { get; }

        public virtual void WriteTo(Utf8JsonWriter writer) => JsonElementFactory.CreateElement(this.Data).WriteTo(writer);
    }
}
