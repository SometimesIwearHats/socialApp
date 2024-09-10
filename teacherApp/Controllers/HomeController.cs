using Microsoft.AspNetCore.Mvc;
using Firebase.Database;
using System.Threading.Tasks;
using socialApp.Models;
using System.Linq;
using Firebase.Database.Query;

namespace socialApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirebaseClient _firebaseClient;

        public HomeController()
        {
            _firebaseClient = new FirebaseClient("https://teacherapp-fb004-default-rtdb.firebaseio.com/");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetch posts from Firebase
            var posts = await _firebaseClient
                .Child("Posts")
                .OrderBy("Timestamp")
                .OnceAsync<Post>();

            var postsList = posts.Select(p => p.Object).OrderByDescending(p => p.Timestamp).ToList();

            return View(postsList); // Pass the list of posts to the view
        }

    }
}
