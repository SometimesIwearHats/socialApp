namespace socialApp.Services
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;

        // Firebase API key
        private readonly string _apiKey = "AIzaSyBYtyQOtUYwSxz8CV5XMzDqvK_-inmCOCM";

        public FirebaseAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Model to capture Firebase authentication response
        public class FirebaseAuthResponse
        {
            [JsonPropertyName("idToken")]
            public string IdToken { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("refreshToken")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("expiresIn")]
            public string ExpiresIn { get; set; }

            [JsonPropertyName("localId")]
            public string LocalId { get; set; }
        }


        // SIGN UP
        // Make this method public so it can be accessed in AuthController
        public async Task<FirebaseAuthResponse> SignUpWithEmailPasswordAsync(string email, string password)
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
            Console.WriteLine("Firebase Response: " + responseContent);

            if (response.IsSuccessStatusCode)
            {
                var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(responseContent);
                // Add this check for null values
                if (result == null || string.IsNullOrEmpty(result.Email) || string.IsNullOrEmpty(result.LocalId))
                {
                    throw new Exception("Firebase returned an invalid or incomplete response.");
                }

                return result;  // Return deserialized response
            }

            throw new Exception($"SignUp failed: {responseContent}");
        }

        // -----------------------------------------------SIGN IN----------------------------------------
        // Make this method public so it can be accessed in AuthController
        public async Task<FirebaseAuthResponse> SignInWithEmailPasswordAsync(string email, string password)
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
            Console.WriteLine("Firebase Response: " + responseContent);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize the response to capture user details
                var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(responseContent);

                // Add this check for null values
                if (result == null || string.IsNullOrEmpty(result.Email) || string.IsNullOrEmpty(result.LocalId))
                {
                    Console.WriteLine("Firebase response has missing fields.");
                    throw new Exception("Firebase returned an invalid or incomplete response.");
                }

                // Print individual fields to verify
                Console.WriteLine($"Firebase Email: {result?.Email}");
                Console.WriteLine($"Firebase LocalId: {result?.LocalId}");
                return result;  // Return deserialized response
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
