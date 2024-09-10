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

        // Create post 
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
                var imageName = $"{post.PostId}_{Image.FileName}";

                var imageUrl = await _firebaseStorage
                    .Child("post_images")
                    .Child(imageName)
                    .PutAsync(stream);

                post.ImageUrl = imageUrl;
            }


            await _firebaseClient
                .Child("Posts")
                .Child(post.PostId)
                .PutAsync(post);

            return RedirectToAction("ViewFeed");
        }

        // View post feed
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

        // Add comment to a post
        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, string commentContent)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Fetch the user's name from UserProfiles
            var userProfile = await _firebaseClient
                .Child("UserProfiles")
                .Child(userId)
                .OnceSingleAsync<UserProfile>();

            if (userProfile == null) return NotFound();

            // Fetch the post
            var post = await _firebaseClient
                .Child("Posts")
                .Child(postId)
                .OnceSingleAsync<Post>();

            if (post == null) return NotFound();

            if (post.Comments == null)
            {
                post.Comments = new List<Comment>();
            }

            // Add the new comment with the user's name
            post.Comments.Add(new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                UserId = userId,
                UserName = userProfile.Name, // Use the user's name
                Content = commentContent,
                Timestamp = DateTime.UtcNow
            });

            // Update the post with the new comment
            await _firebaseClient
                .Child("Posts")
                .Child(postId)
                .PutAsync(post);

            return RedirectToAction("ViewFeed");
        }

        // Like button for posts
        [HttpPost]
        public async Task<IActionResult> ToggleLike(string postId)
        {
            var userId = HttpContext.Session.GetString("UserId");

            var post = await _firebaseClient
                .Child("Posts")
                .Child(postId)
                .OnceSingleAsync<Post>();

            if (post == null)
            {
                return BadRequest();
            }

            bool liked;

            // Check if the user has already liked the post
            if (post.LikesList.Contains(userId))
            {
                post.LikesList.Remove(userId);  // Unlike the post
                liked = false;
            }
            else
            {
                post.LikesList.Add(userId);  // Like the post
                liked = true;
            }

            await _firebaseClient
                .Child("Posts")
                .Child(postId)
                .PutAsync(post);

            // Return JSON with a flag to indicate the like status
            return Json(new { liked = liked, likesCount = post.LikesList.Count });
        }





    }
}
