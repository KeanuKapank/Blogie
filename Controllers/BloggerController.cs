using Blogie.Auth.Models;
using Blogie.Auth.Repository;
using Blogie.Blogger.Data.Models;
using Blogie.Blogger.Data.Repository;
using Blogie.UI.Models.ViewModel;
using Blogie.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Security.Claims;

namespace Blogie.UI.Controllers
{

    
    public class BloggerController : Controller
    {
        // GET: BloggerController

        private readonly IBloggerRepository _repo;
        private readonly int UserID;
        private readonly IUserInfoService _service;
        public BloggerController(IBloggerRepository repo, IUserInfoService service)
        {
            _repo = repo;
            _service = service;
            UserID = _service.GetLogUserId();
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<Post> posts = await _repo.ViewAllPostOfUser(UserID);
            return View(posts);
        }

        
        public ActionResult Details(int id)
        {
            return View();
        }

        
        public async Task<ActionResult> AddPost()
        {
            PostCategory postCategory = new PostCategory()
            {
                Post = new Post(),
                Categories = await _repo.GetCategories()
            };
            return View(postCategory);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPost(Post post, IFormFile file)
        {
            byte[] fileData = null;
            if (file != null && file.Length != 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileData = ms.ToArray();
                }

                post.Picture = fileData;
            }
            else
            {
                ModelState.AddModelError("Picture", "Please select an Picture for yourself");
            }

            post.UserID = UserID;
            post.DatePosted = DateTime.Now;

            if (ModelState.IsValid)
            {
                await _repo.AddPost(post);
                TempData["MessageSuccess"] = "Post Added Successfully";
                return RedirectToAction("Index");
            }

            PostCategory postCategory = new PostCategory()
            {
                Post = post,
                Categories = await _repo.GetCategories()
            };
            return View(postCategory);
        }

        // GET: BloggerController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Post post = await _repo.ViewPostOfUser(UserID, id);

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                PostCategory postCategory = new PostCategory
                {
                    Post = post,
                    Categories = await _repo.GetCategories(),
                };

                return View(postCategory);
            }
            return View();
        }

        // POST: BloggerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Post post, IFormFile file)
        {
            Post ogPost = await _repo.ViewPostOfUser(UserID, post.PostID);
            byte[] fileData = null;
            if (file != null && file.Length != 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileData = ms.ToArray();
                }

                post.Picture = fileData;
            }
            else
            {
                post.Picture = ogPost.Picture;
            }

            post.UserID = ogPost.UserID;
            post.DatePosted = ogPost.DatePosted;
      

            if (ModelState.IsValid)
            {
                await _repo.EditPost(post);
                TempData["MessageSuccess"] = "Post Edited Successfully";
                return RedirectToAction("Index");
            }

            PostCategory postCategory = new PostCategory()
            {
                Post = post,
                Categories = await _repo.GetCategories()
            };
            return View(postCategory);
        }

        
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Post post = await _repo.ViewPostOfUser(UserID, id);

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                TempData["PostID"] = post.PostID;
                TempData["PostTitle"] = post.Title;
                return RedirectToAction("Index");
            }  
        }

        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _repo.DeletePost(id);
                return Json(new { success = true, message = "Post deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // Return a JSON response with the error message
                return Json(new { success = false, message = "An error occurred while deleting the post: " + ex.Message });
            }
        }
    }
}
