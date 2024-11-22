using System.Net;

namespace aoc_2024.AocClient
{
    public class AocHttpClient : IAocClient, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string? sessionCookie;

        public AocHttpClient()
        {
            this.sessionCookie = GetSessionCookie();

            CookieContainer cookieContainer = new();

            cookieContainer.Add(new Uri(Consts.baseUri),
                new Cookie("session", this.sessionCookie));

            HttpClientHandler httpClientHandler = new()
            {
                CookieContainer = cookieContainer,
            };

            this.httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = new Uri($"{Consts.baseUri}/{Consts.year}/")
            };
        }

        public async Task<ClientResponse> GetPuzzleInput(int dayNumber)
        {
            if (string.IsNullOrEmpty(this.sessionCookie))
            {
                return new ClientResponse
                {
                    ResponseType = ClientResponseType.Failure,
                    Content = $"Session cookie not found."
                };
            }

            HttpResponseMessage response = await this.httpClient
                .GetAsync($"day/{dayNumber}/input");

            string content = await response.Content.ReadAsStringAsync();

            return new ClientResponse
            {
                ResponseType = response.IsSuccessStatusCode ?
                    ClientResponseType.Success :
                    ClientResponseType.Failure,
                Content = content
            };
        }

        private static string? GetSessionCookie()
        {
            string filePath = Path.Combine("ProgramUtils", "session-cookie.txt");

            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllText(filePath);
        }

        public void Dispose()
        {
            this.httpClient.Dispose();
        }
    }
}
