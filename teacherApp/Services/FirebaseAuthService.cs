namespace socialApp.Services
{
    using Microsoft.AspNetCore.Mvc;
    using socialApp.Models;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

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
                return responseContent;
            }

            throw new Exception($"SignIn failed: {responseContent}");
        }


        // Method to fetch the user profile from the Realtime Database
        public async Task<string> GetUserProfileAsync(string userId)
        {
            var response = await _httpClient.GetAsync(
                $"https://teacherapp-fb004-default-rtdb.firebaseio.com/UserProfiles/{userId}.json?auth={_apiKey}"
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                // Return profile data as JSON string
                return responseContent;
            }

            throw new Exception($"Failed to retrieve profile: {responseContent}");
        }

    }
}
