// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Newtonsoft.Json;

namespace Bicep.Core.Emit
{
    public interface ITemplateWriter
    {
        void Write(JsonTextWriter writer);
    }
}
