using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Storage;
using Newtonsoft.Json;
using socialApp.Services; // Add this to import the FirebaseAuthService

var builder = WebApplication.CreateBuilder(args);

// Load Firebase configuration from firebaseConfig.json
var firebaseConfig = LoadFirebaseConfig();
Console.WriteLine("API Key: " + firebaseConfig.ApiKey);
Console.WriteLine("Auth Domain: " + firebaseConfig.AuthDomain);

// Configure FirebaseAuthClient
var config = new FirebaseAuthConfig
{
    ApiKey = firebaseConfig.ApiKey.ToString(),
    AuthDomain = firebaseConfig.AuthDomain.ToString(),
    Providers = new FirebaseAuthProvider[]
    {
        new EmailProvider()  // Add and configure providers as needed
    },
    UserRepository = new FileUserRepository("FirebaseSample")  // Persistent user credentials storage
};

var authClient = new FirebaseAuthClient(config);

// Register the FirebaseAuthClient
builder.Services.AddSingleton(authClient);

// Register FirebaseAuthService for Dependency Injection (DI) and HttpClient
builder.Services.AddHttpClient<FirebaseAuthService>();

// Initialize Firebase services
builder.Services.AddSingleton(new FirebaseClient(firebaseConfig.DatabaseURL.ToString()));
builder.Services.AddSingleton(new FirebaseStorage(firebaseConfig.StorageBucket.ToString()));

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();  // Required for session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie accessible only to the server
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();  // Add the session middleware

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Method to load Firebase configuration from firebaseConfig.json
FirebaseConfig LoadFirebaseConfig()
{
    // Define the path to the Firebase config file
    var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "firebaseConfig.json");

    // Check if the configuration file exists
    if (!File.Exists(configFilePath))
    {
        throw new FileNotFoundException("Firebase configuration file not found.");
    }

    // Read the content of the configuration file
    var configJson = File.ReadAllText(configFilePath);

    // Deserialize the JSON content into the FirebaseConfig class
    FirebaseConfig config = JsonConvert.DeserializeObject<FirebaseConfig>(configJson);

    // Check if the deserialization was successful and required fields are not null or empty
    if (config == null || string.IsNullOrEmpty(config.ApiKey) || string.IsNullOrEmpty(config.AuthDomain)
        || string.IsNullOrEmpty(config.DatabaseURL) || string.IsNullOrEmpty(config.ProjectId)
        || string.IsNullOrEmpty(config.StorageBucket) || string.IsNullOrEmpty(config.MessagingSenderId)
        || string.IsNullOrEmpty(config.AppId))
    {
        throw new InvalidOperationException("One or more required Firebase configuration fields are missing.");
    }

    return config;
}
