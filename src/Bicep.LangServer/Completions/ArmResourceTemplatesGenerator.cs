// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.LanguageServer.Completions
{
    public static class ArmResourceTemplatesGenerator
    {
        public static void GenerateTemplates()
        {
            string content = File.ReadAllText(@"C:\Users\bhsubra\Desktop\armsnippets.jsonc");

            JObject jObject = JObject.Parse(content, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Load,
            });

            foreach (JToken child in jObject.Children())
            {
                if (child.HasValues)
                {
                    JToken value = child.Value<JProperty>().Value;

                    string? context = value.SelectToken("context")?.Value<string>();
                    string? prefix = value.SelectToken("prefix")?.Value<string>();
                    string? description = value.SelectToken("description")?.Value<string>();
                    JToken? bodyToken = value.SelectToken("body");

                    if (context is not null &&
                        context.Equals("resources") &&
                        prefix is not null)
                    {
                        if (bodyToken is not null)
                        {
                            JObject snippet = new JObject()
                            {
                                ["context"] = context,
                                ["prefix"] = prefix,
                                ["description"] = description,
                                ["resources"] = value.SelectToken("body")
                            };

                            JToken parsedJson = JToken.Parse(JsonConvert.SerializeObject(snippet));
                            var formattedJson = parsedJson.ToString(Formatting.Indented);

                            File.WriteAllText(@"C:\Users\bhsubra\Desktop\ArmSnippets\" + prefix + ".jsonc", formattedJson);
                        }
                    }
                }
            }
        }
    }
}

