using Microsoft.AspNetCore.Mvc;
using socialApp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using socialApp.Services;


namespace socialApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly FirebaseAuthService _authService;

        public AuthController(FirebaseAuthService authService)
        {
            _authService = authService;
        }

        // Signup
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
                var result = await _authService.SignUpWithEmailPasswordAsync(model.Email, model.Password);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
                return View(model);
            }
        }

        // Login
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

                // If login is successful, store the user's email in session
                if (result != null)
                {
                    HttpContext.Session.SetString("UserEmail", model.Email);
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

        // Logout
        [HttpPost]
        public IActionResult Logout()
        {
            // Clear the session to log out the user
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
