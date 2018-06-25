using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TheWall.Models;

namespace TheWall.Controllers
{
    public class ContentController : Controller
    {
        private readonly DbConnector _dbConnector;
        public ContentController(DbConnector connect)
        {
            _dbConnector = connect;
        }
        //-----------PRIMARY PAGE LOAD FOR THEWALL!
        [HttpGet("thewall")]
        public IActionResult TheWall()
        {
            // Check if userID is not in session, if true, redirect to home 
            int? userID = HttpContext.Session.GetInt32("userID");
            if(userID == null)
            {
                return RedirectToAction("Welcome", "Main");
            }
            ViewBag.AllMessages = AllMessages();
            ViewBag.AllComments = AllComments();
            ViewBag.User = _dbConnector.Query($"SELECT * FROM users WHERE id = {(int)HttpContext.Session.GetInt32("userID")}");
            ViewBag.UserID = (int)HttpContext.Session.GetInt32("userID");
            return View();
        }
        //------------MESSAGE CREATION!
        [HttpPost("messages/create")]
        public IActionResult CreateMessage(TheWallModels theWall)
        {
            if(ModelState.IsValid)
            {
                string query = $"INSERT INTO messages (user_id, message, created_at, updated_at) VALUES ({(int)HttpContext.Session.GetInt32("userID")}, '{theWall.MessagePost.MessageContent}', NOW(), NOW());";
                _dbConnector.Execute(query);
                return RedirectToAction("TheWall");
            }
            else 
            {
                // Return validation error and store in TempData
                foreach(var modelState in ModelState.Values)
                {
                    foreach(var error in modelState.Errors)
                    {
                        TempData["errors"] = error.ErrorMessage;

                    }
                }
                return RedirectToAction("TheWall");
            }
        }
        //------------COMMENT CREATION!
        [HttpPost("comments/create")]
        public IActionResult CreateComment(TheWallModels theWall)
        {
            if(ModelState.IsValid)
            {
                string query = $"INSERT INTO comments (message_id, user_id, comment, created_at, updated_at) VALUES ('{theWall.CommentPost.MessageID}', {(int)HttpContext.Session.GetInt32("userID")}, '{theWall.CommentPost.CommentContent}', NOW(), NOW());";
                _dbConnector.Execute(query);
                return RedirectToAction("TheWall");
            }
            else 
            {
                // Return validation error and store in TempData
                foreach(var modelState in ModelState.Values)
                {
                    foreach(var error in modelState.Errors)
                    {
                        TempData["errors"] = error.ErrorMessage;

                    }
                }
                return RedirectToAction("TheWall");
            }
        }
        //-------------DELETE MESSAGE!
        [HttpGet("delete/message/{id}")]
        public IActionResult DeleteMessage(int id)
        {
            string query = $"DELETE FROM comments WHERE message_id = {id}; DELETE FROM messages WHERE id = {id};";
            _dbConnector.Execute(query);
            return RedirectToAction("TheWall");
        }



        //------Querying ALL messages
        public List<Dictionary<string,object>> AllMessages()
        {
            string query = "SELECT messages.id AS message_id, messages.message, messages.created_at, users.id AS user_id, users.first_name, users.last_name FROM messages JOIN users ON messages.user_id = users.id ORDER BY created_at DESC;";
            return _dbConnector.Query(query);
        }
        //------Querying ALL comments
        public List<Dictionary<string, object>> AllComments()
        {
            string query = "SELECT comments.id AS comment_id, comments.message_id, comments.comment, comments.created_at, users.id AS user_id,users.first_name, users.last_name FROM comments JOIN messages ON comments.message_id = messages.id JOIN users ON comments.user_id = users.id;";
            return _dbConnector.Query(query);
        }
    }
}