using Microsoft.AspNetCore.Mvc;
using socialApp.Models;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Firebase.Database.Query;
using socialApp.Services;
using System;

namespace socialApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseAuthService _authService;

        public ProfileController(FirebaseAuthService authService)
        {
            _firebaseClient = new FirebaseClient("https://teacherapp-fb004-default-rtdb.firebaseio.com/");
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> ViewProfile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Log userEmail to see if it's being set properly
            Console.WriteLine("Session UserEmail: " + userEmail);

            if (string.IsNullOrEmpty(userEmail))
            {
                Console.WriteLine("No user email found in session.");
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Fetch user profile from Firebase Realtime Database
                var userProfile = await _firebaseClient
                    .Child("UserProfiles")
                    .OrderBy("Email")
                    .EqualTo(userEmail)
                    .OnceSingleAsync<UserProfile>();

                if (userProfile != null)
                {
                    Console.WriteLine("Profile fetched from Firebase. Name: " + userProfile.Name);
                }
                else
                {
                    Console.WriteLine("No profile found for email: " + userEmail);
                    return RedirectToAction("EditProfile");
                }

                return View(userProfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching profile: " + ex.Message);
                return RedirectToAction("Login", "Auth");
            }
        }




        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Fetch user profile from Firebase Realtime Database
            var userProfile = await _firebaseClient
                .Child("UserProfiles")
                .OrderBy("Email")
                .EqualTo(userEmail)
                .OnceSingleAsync<UserProfile>();

            if (userProfile == null)
            {
                // Create a new profile if none exists
                userProfile = new UserProfile { Email = userEmail };
            }

            return View(userProfile);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UserProfile model, IFormFile profilePicture)
        {
            if (ModelState.IsValid)
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction("Login", "Auth");
                }

                model.Email = userEmail;

                // Fetch the existing user profile using Email or UserId
                var existingUserProfile = await _firebaseClient
                    .Child("UserProfiles")
                    .OrderBy("Email")
                    .EqualTo(userEmail)
                    .OnceSingleAsync<UserProfile>();

                if (existingUserProfile == null)
                {
                    // If no profile exists, create a new UserId
                    model.UserId = Guid.NewGuid().ToString(); // Generate a new UserId
                }
                else
                {
                    // Keep existing UserId to ensure we're updating the correct profile
                    model.UserId = existingUserProfile.UserId;
                }

                // Upload profile picture to Firebase Storage (if any)
                if (profilePicture != null)
                {
                    var stream = profilePicture.OpenReadStream();

                    // Generate a unique filename using UserId
                    var uniqueFileName = $"{model.UserId}_{DateTime.Now.Ticks}_{profilePicture.FileName}";

                    // Upload the file to Firebase Storage
                    var task = new FirebaseStorage("teacherapp-fb004.appspot.com")
                        .Child("profile_pictures")
                        .Child(uniqueFileName)
                        .PutAsync(stream);

                    // Await the task and get the download URL
                    model.ProfilePictureUrl = await task;

                    // Log the URL for debugging purposes
                    Console.WriteLine("Profile picture URL: " + model.ProfilePictureUrl);
                }
                else if (existingUserProfile != null)
                {
                    // Keep the old profile picture URL if no new one is uploaded
                    model.ProfilePictureUrl = existingUserProfile.ProfilePictureUrl;
                }

                // Update user profile in Firebase Realtime Database
                await _firebaseClient
                    .Child("UserProfiles")
                    .Child(model.UserId) // Update the existing profile by using UserId
                    .PutAsync(model);

                // Log the update for verification
                Console.WriteLine("Profile updated for user: " + model.UserId);

                return RedirectToAction("ViewProfile");
            }

            return View(model);
        }
    }
}
