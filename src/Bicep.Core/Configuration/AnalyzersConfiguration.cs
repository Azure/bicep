// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Json;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class AnalyzersConfiguration : ConfigurationSection<JsonElement>
    {
        public AnalyzersConfiguration(JsonElement data) : base(data) { }

        public static AnalyzersConfiguration Empty => CreateEmptyAnalyzersConfiguration();

        public T GetValue<T>(string path, T defaultValue)
        {
            var element = this.Data.TryGetPropertyByPath(path);

            if (element.HasValue)
            {
                return element.Value.ToNonNullObject<T>();
            }

            return defaultValue;
        }

        public AnalyzersConfiguration SetValue(string path, object value) => new(this.Data.SetPropertyByPath(path, value));

        private static AnalyzersConfiguration CreateEmptyAnalyzersConfiguration() => new(JsonElementFactory.CreateElement("{}"));
    }
}
