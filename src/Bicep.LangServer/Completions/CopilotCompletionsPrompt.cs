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
- Your responses should be informative, visual, logical, actionable and concise with just the Bicep resource body.
- Do not add additional context to the user question or request.
- Use your trained knowledge to determine resource type aliases as necessary and infer the parameter of the Bicep file.
- The Bicep resource to be completed have the following schema:
<Schema Start>
{{schemaContent}}
<Schema End>
- The partially written Bicep file is:
<Bicep Start>
{{bicepContent}}
<Bicep End>
""";

            return prompt;
        }
    }
}
