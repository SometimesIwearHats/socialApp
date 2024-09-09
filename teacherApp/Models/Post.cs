using System;
using System.Collections.Generic;

namespace socialApp.Models
{
    public class Post
    {
        public string PostId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Likes { get; set; } = 0;
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class Comment
    {
        public string CommentId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
