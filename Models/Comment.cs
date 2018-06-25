using System;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Comment 
    {
        [Required(ErrorMessage = "Comment can't be blank!")]
        public string CommentContent {get; set;}
        public int MessageID {get; set;}
    }
}