// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Json;

namespace Bicep.Core.Configuration
{
    /// <summary>
    /// Non-generic view of a configuration section that can serialize itself to JSON.
    /// Lets consumers write a section without knowing its concrete data type.
    /// </summary>
    public interface IWritableConfigurationSection
    {
        void WriteTo(Utf8JsonWriter writer);
    }

    public abstract class ConfigurationSection<T> : IWritableConfigurationSection
    {
        protected ConfigurationSection(T data)
        {
            this.Data = data;
        }

        public T Data { get; }

        public virtual void WriteTo(Utf8JsonWriter writer) => JsonElementFactory.CreateElement(this.Data).WriteTo(writer);
    }
}
