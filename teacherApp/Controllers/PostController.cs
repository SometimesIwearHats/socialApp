using Microsoft.AspNetCore.Mvc;
using socialApp.Models;
using Firebase.Database;
using Firebase.Storage;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Firebase.Database.Query;
using Microsoft.Extensions.Options;

namespace socialApp.Controllers
{
    public class PostController : Controller
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseStorage _firebaseStorage;
        private readonly string _storageBucket;

        public PostController(IOptions<FirebaseConfig> firebaseConfig)
        {
            _firebaseClient = new FirebaseClient(firebaseConfig.Value.DatabaseURL);
            _firebaseStorage = new FirebaseStorage(firebaseConfig.Value.StorageBucket);
            _storageBucket = firebaseConfig.Value.StorageBucket;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string Content, IFormFile Image)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(Content))
            {
                ModelState.AddModelError("", "Content cannot be empty.");
                return View();
            }

            var post = new Post
            {
                UserId = userId,
                UserName = userName,
                Content = Content,
                Timestamp = DateTime.UtcNow
            };

            if (Image != null)
            {
                var stream = Image.OpenReadStream();
                var imageUrl = await _firebaseStorage
                    .Child("post_images")
                    .Child($"{post.PostId}_{Image.FileName}")
                    .PutAsync(stream);
                post.ImageUrl = imageUrl;
            }

            await _firebaseClient
                .Child("Posts")
                .Child(post.PostId)
                .PutAsync(post);

            return RedirectToAction("ViewFeed");
        }

        [HttpGet]
        public async Task<IActionResult> ViewFeed()
        {
            var posts = await _firebaseClient
                .Child("Posts")
                .OrderBy("Timestamp")
                .OnceAsync<Post>();

            var postsList = posts.Select(p => p.Object).OrderByDescending(p => p.Timestamp).ToList();
            return View(postsList);
        }
    }
}
