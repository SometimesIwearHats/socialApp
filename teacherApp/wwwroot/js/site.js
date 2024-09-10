// Add comment
function addComment(postId) {
    var commentContent = document.getElementById('comment-input-' + postId).value;

    if (!commentContent) {
        alert('Please enter a comment.');
        return;
    }

    $.ajax({
        url: '/Post/AddComment', 
        type: 'POST',
        data: { postId: postId, commentContent: commentContent },
        success: function (response) {
            if (response.success) {
                // Clear the input
                document.getElementById('comment-input-' + postId).value = '';

                // Append the new comment to the comment section dynamically
                var commentSection = document.querySelector('.comments-section-' + postId);
                var newComment = `<div class="comment">
                                    <strong>${response.comment.UserName}:</strong>
                                    <p>${response.comment.Content}</p>
                                    <small>${new Date(response.comment.Timestamp).toLocaleString()}</small>
                                  </div>`;
                commentSection.innerHTML += newComment;
            } else {
                alert(response.message);
            }
        },
        error: function () {
            alert('Failed to add comment.');
        }
    });
}


// Like button toggle
function toggleLike(postId) {
    $.ajax({
        url: '/Post/ToggleLike',
        type: 'POST',
        data: { postId: postId },
        success: function (response) {
            // Update the Like count dynamically without page reload
            var likeButton = $('#like-button-' + postId);
            var currentLikes = parseInt(likeButton.attr('data-likes'));

            if (response.liked) {
                likeButton.attr('data-likes', currentLikes + 1);
                likeButton.html('Like (' + (currentLikes + 1) + ')');
            } else {
                likeButton.attr('data-likes', currentLikes - 1);
                likeButton.html('Like (' + (currentLikes - 1) + ')');
            }
        },
        error: function () {
            alert('Failed to like the post.');
        }
    });
}



