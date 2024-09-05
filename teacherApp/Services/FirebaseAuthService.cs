namespace socialApp.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System;

    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;

        //Firebase API key
        private readonly string _apiKey = "AIzaSyBYtyQOtUYwSxz8CV5XMzDqvK_-inmCOCM";

        public FirebaseAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SignUpWithEmailPasswordAsync(string email, string password)
        {
            var requestBody = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={_apiKey}",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Handle the successful sign-up
                return responseContent;
            }

            throw new Exception($"SignUp failed: {responseContent}");
        }

        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            var requestBody = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Handle the successful sign-in 
                return responseContent;
            }

            throw new Exception($"SignIn failed: {responseContent}");
        }

        public async Task ChangeUserPassword(string idToken, string newPassword)
        {
            var requestBody = new
            {
                idToken = idToken,
                password = newPassword,
                returnSecureToken = true
            };

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={_apiKey}",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"ChangePassword failed: {responseContent}");
            }
        }
    }
}
