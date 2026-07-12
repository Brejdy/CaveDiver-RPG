using CaveDiver.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace CaveDiver.Dialogue
{
    public class DialogueProvider : IDialogueProvider
    {
        private readonly HttpClient _http = new HttpClient();

        public async Task<string> GetResponseAsync(DialogueContext context)
        {
            var response = new StringBuilder();

            await foreach (var chunk in StreamResponseAsync(context))
            {
                response.Append(chunk);
            }

            return response.Length > 0 ? response.ToString() : "I remain silent.";
        }

        public async IAsyncEnumerable<string> StreamResponseAsync(DialogueContext context)
        {
            var prompt = context.Companion.BuildAIPrompt(context.Player, context.PlayerInput);

            var request = new
            {
                model = "llama3",
                prompt = prompt,
                stream = true
            };

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost:11434/api/generate")
            {
                Content = JsonContent.Create(request)
            };

            using var response = await _http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var json = JsonSerializer.Deserialize<OllamaResponse>(line);
                if (!string.IsNullOrEmpty(json?.response))
                {
                    yield return json.response;
                }
            }
        }

        public class OllamaResponse
        {
            public string response { get; set; } = "";
        }
    }
}
