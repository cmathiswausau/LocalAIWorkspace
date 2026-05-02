// <copyright file="AiService.cs" company="LocalAIWorkspace">
// Copyright (c) LocalAIWorkspace. All rights reserved.
// </copyright>
namespace LocalAIWorkspace.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    /// <summary>
    /// Service for interacting with the local AI API.
    /// </summary>
    public class AiService
    {
        private readonly HttpClient client = new();

        /// <summary>
        /// Sends the specified prompt to the AI model and asynchronously retrieves the generated response.
        /// </summary>
        /// <remarks>The method communicates with a locally hosted AI service using the 'mistral' model.
        /// Ensure the service is running and accessible at the specified endpoint before calling this method.</remarks>
        /// <param name="prompt">The input text or question to send to the AI model for generating a response. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the AI-generated response as a
        /// string. Returns an error message if the request fails.</returns>
        public async Task<string> AskAsync(string prompt)
        {
            var request = new
            {
                model = "mistral",
                prompt = prompt,
                stream = false,
            };

            var json = JsonSerializer.Serialize(request);

            var response = await this.client.PostAsync(
                "http://localhost:11434/api/generate",
                new StringContent(json, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                return "AI request failed";
            }

            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            return doc.RootElement.GetProperty("response").GetString() ?? string.Empty;
        }
    }
}
