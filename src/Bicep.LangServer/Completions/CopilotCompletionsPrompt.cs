// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Static class for providing copilot autocompletion prompt.
    /// </summary>
    public static class CopilotCompletionsPrompt
    {
        /// <summary>
        /// The prompt to be sent to the OpenAI autocompletions model.
        /// TODO: Refine and add few-shot examples.
        /// </summary>
        public static string GetSystemPrompt()
        {
            var prompt = $$"""
You are an assistant that is knowledgeable about Azure Bicep language. You will help users with Bicep related requests.
""";

            return prompt;
        }

        public static string GetUserPrompt(string schemaContent, string bicepContent)
        {
            var prompt = $$"""
Please help me generate the body of a Bicep resource definition by inspecting a partially written Bicep file. The schema of the resource type starts from <Schema Start> and ends at <Schema End>.
Here are a few clarifying instructions:
- The partially written Bicep file starts after <Bicep Start> and ends right before <Bicep End>
- Your response should contain ONLY the body of the resource definition (bicep code) and nothing else. In other words, your response should start with 'resource' and end with '}'.
- Use your trained knowledge to determine resource type aliases as necessary and infer the parameters of the Bicep file.
- The Bicep resource to be completed have the following schema:
<Schema Start>
{{schemaContent}}
<Schema End>
- You should populate properties using the information in the schema.
- You MUST populate all the Required properties. You MUST NOT populate ReadOnly properties. You MUST also populate properties not marked as Required or ReadOnly.
- When populating enum properties, put all possible values as the value of the property. Separate possible values with "|".
- String values should always be enclosed in single quotes.
- The partially written Bicep file is:
<Bicep Start>
{{bicepContent}}
<Bicep End>
""";

            return prompt;
        }
    }
}
