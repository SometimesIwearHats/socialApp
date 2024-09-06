using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Database;
using Firebase.Storage;
using FirebaseAdmin;
using Newtonsoft.Json;
using socialApp.Services; // Import the FirebaseAuthService
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// Initialize Firebase
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("Config/teacherapp-fb004-firebase-adminsdk-i85x3-a4daa34515.json"),
});

// Register HttpClient
builder.Services.AddHttpClient(); // Register HttpClient

// Register FirebaseAuthService
builder.Services.AddScoped<FirebaseAuthService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Add session services

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession(); // Enable session middleware

// Custom routes for logged in teachers to view/add students
app.MapControllerRoute(
    name: "allstudents",
    pattern: "School/AllStudents",
    defaults: new { controller = "Students", action = "Index" });

app.MapControllerRoute(
    name: "addstudent",
    pattern: "School/AddStudent",
    defaults: new { controller = "Students", action = "Create" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
