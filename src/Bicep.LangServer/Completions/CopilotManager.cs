// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using global::Azure;
using global::Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Completions
{
    /// <summary>
    /// Singleton class to manage the Policy Copilot setup.
    /// </summary>
    public class CopilotManager
    {
        private const string DeploymentName = "gpt-35-turbo-16k";
        private static OpenAIClient? client;
        private int numTriesRemaining = 3;

        /// <summary>
        /// Initializes the OpenAIClient.
        /// </summary>
        /// <returns>Whether or not the client was successfully initialized.</returns>
        public bool ConnectToEndpoint()
        {
            if (this.numTriesRemaining > 0 && client == null)
            {
                try
                {
                    Uri modelEndpoint = new("https://americasopenai.azure-api.net");
                    AzureKeyCredential modelCredentials = new("TODO: HERE COMES THE API KEY");
                    client = new OpenAIClient(modelEndpoint, modelCredentials);
                }
                catch (Exception) // ex)
                {
                    // TODO: Make these warnings the user can see.
                    //Console.WriteLine($"An exception occurred with the Policy Copilot while attempting to connect to the OpenAI model endpoint: {ex.Message}");
                    //this.numTriesRemaining--;
                    //if (this.numTriesRemaining == 0)
                    //{
                    //    Console.WriteLine("The Policy Copilot has unsuccessfully attempted to connect to the OpenAI model endpoint multiple times and will, therefore, disable Policy autocompletion.");
                    //}

                    throw;
                }
            }

            //return Task.FromResult(client != null);
            return client != null;
        }

        /// <summary>
        /// Passes the given completionsOptions prompt/settings to the OpenAI model and returns the responses list.
        /// Returns null if any exception occurs while making the API call or if the client has not yet been initialized.
        /// </summary>
        /// <param name="completionsOptions">All of the content/options being sent in the request (including the prompt).</param>
        /// <returns>A list of the model completions for the given completionsOptions.</returns>
        public async Task<IReadOnlyList<ChatChoice>?> GetModelChatCompletionsAsync(ChatCompletionsOptions completionsOptions)
        {
            if (client == null)
            {
                // Programmer error
                //Console.WriteLine("Need to successfully call ConnectToEndpoint() to initialize OpenAIClient before requesting completions.");
                return null;
            }

            try
            {
                Response<ChatCompletions> completionsResponse = await client.GetChatCompletionsAsync(DeploymentName, completionsOptions);
                return completionsResponse.Value.Choices;
            }
            catch (Exception) // ex)
            {
                // TODO: Make this a warning the user can see.
                //Console.WriteLine($"An exception occurred with the Policy Copilot while attempting to make an OpenAI API call: {ex.Message}");
                return null;
            }
        }
    }
}
