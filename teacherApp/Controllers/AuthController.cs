using Microsoft.AspNetCore.Mvc;
using socialApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using socialApp.Services;
using System;
using Firebase.Database;
using Firebase.Database.Query;

namespace socialApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly FirebaseAuthService _authService;
        private readonly FirebaseClient _firebaseClient;

        public AuthController(FirebaseAuthService authService)
        {
            _authService = authService;
            _firebaseClient = new FirebaseClient("https://teacherapp-fb004-default-rtdb.firebaseio.com/"); // Initialize Firebase Client
        }

        // ---------------------- Signup ----------------------
        [HttpGet]
        public IActionResult SignUp()
        {
            return View(new SignUpModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Sign up the user
                var result = await _authService.SignUpWithEmailPasswordAsync(model.Email, model.Password);

                // Call SaveUserProfile after successful sign-up
                await SaveUserProfile(result.LocalId, result.Email);

                // Redirect to login after successful sign-up
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
                return View(model);
            }
        }

        // ---------------------- Login ----------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.SignInWithEmailPasswordAsync(model.Email, model.Password);

                // If login is successful, store the user's email and UserId in session
                if (result != null)
                {
                    HttpContext.Session.SetString("UserEmail", result.Email);   // Store Email in session
                    HttpContext.Session.SetString("UserId", result.LocalId);    // Store UserId in session

                    Console.WriteLine($"Session UserId set: {result.LocalId}");
                    Console.WriteLine($"Session UserEmail set: {result.Email}");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    model.ErrorMessage = "Invalid login attempt.";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Invalid login attempt: " + ex.Message;
                return View(model);
            }
        }

        // ---------------------- Logout ----------------------
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session to log out the user
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ---------------------- Save User Profile to Firebase ----------------------
        public async Task SaveUserProfile(string userId, string email)
        {
            // Create a new user profile with default values
            var userProfile = new UserProfile
            {
                UserId = userId,
                Email = email,
                Name = "Default Name",  
                Bio = "Default Bio",    
                ProfilePictureUrl = ""  
            };

            // Save the user profile to Firebase Realtime Database
            await _firebaseClient
                .Child("UserProfiles")        // Navigate to the UserProfiles node
                .Child(userId)                // Create a child with the user's ID
                .PutAsync(userProfile);       // Insert the user profile data

            // Log the successful profile creation
            Console.WriteLine($"Profile saved for UserId: {userId}, Email: {email}");
        }

    }
}
