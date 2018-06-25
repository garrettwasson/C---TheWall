using System;
using System.ComponentModel.DataAnnotations;

namespace TheWall.Models
{
    public class Message 
    {
        [Required(ErrorMessage = "Message can't be blank!")]
        public string MessageContent {get; set;}
    }
}