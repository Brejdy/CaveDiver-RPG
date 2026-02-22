using CaveDiver.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;

namespace CaveDiver.Dialogue
{
    public class DialogueProvider : IDialogueProvider
    {
        private readonly HttpClient _http = new HttpClient();
        public async Task<string> GetResponseAsync(DialogueContext context)
        {
            var prompt = context.Companion.BuildAIPrompt(context.Player, context.PlayerInput);

            var request = new
            {
                model = "llama3",
                prompt = prompt,
                stream = false
            };

            var response = await _http.PostAsJsonAsync(
                "http://localhost:11434/api/generate",
                request
            );

            var json = await response.Content.ReadFromJsonAsync<OllamaResponse>();

            return json?.response ?? "I remain silent.";
        }

        public class OllamaResponse
        {
            public string response { get; set; } = "";
        }
    }
}
