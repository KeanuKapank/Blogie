using Blogie.Admin.Data.Models;
using Blogie.Admin.Data.Repository;
using Blogie.Auth.Models;
using Blogie.Blogger.Data.Models;
using Blogie.Blogger.Data.Repository;
using Blogie.UI.Models.ViewModel;
using Blogie.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace Blogie.UI.Controllers
{
    
    public class AdminController : Controller
    {
        // GET: AdminController
        private readonly IAdminRepository _adminrepo;
        private readonly IBloggerRepository _bloggerrepo;
        private readonly int UserID;
        private readonly IUserInfoService _service;
        public AdminController(IBloggerRepository repo, IUserInfoService service, IAdminRepository adminrepo)
        {
            _adminrepo = adminrepo;
            _bloggerrepo = repo;
            _service = service;
            UserID = _service.GetLogUserId();
        }

        public async Task<ActionResult> Index()
        {
            IEnumerable<DisplayAllPostToAdmin> posts = await _adminrepo.AdminViewAllPost();
            return View(posts);
        }

        [HttpPost]        
        public async Task<ActionResult> Index(string txtSearch)
        {
            if (txtSearch != null || txtSearch != " ")
            {
                IEnumerable<DisplayAllPostToAdmin> posts = await _adminrepo.AdminSearchPost(txtSearch);
                return View(posts);
            }
            else
            {
                IEnumerable<DisplayAllPostToAdmin> posts = await _adminrepo.AdminViewAllPost();
                return View(posts);
            }
        }

        [HttpPost]
        public ActionResult SearchPost(string txtSearch)
        {
            // Store the received message in TempData
            

            // Check if TempData is not null and retrieve the data
            if (txtSearch != null || txtSearch != "")
            {
                
            }
            return RedirectToAction("Index");
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/AddPost
        public async Task<ActionResult> AddPost()
        {
            PostCategory postCategory = new PostCategory()
            {
                Post = new Post(),
                Categories = await _bloggerrepo.GetCategories()
            };
            return View(postCategory);
        }

        // POST: AdminController/Create
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
                await _adminrepo.AdminAddPost(post);
                TempData["MessageSuccess"] = "Post Added Successfully";
                return RedirectToAction("Index");
            }

            PostCategory postCategory = new PostCategory()
            {
                Post = post,
                Categories = await _bloggerrepo.GetCategories()
            };
            return View(postCategory);
        }

        public async Task<ActionResult> EditPost(int id)
        {
            PostCategory postCategory = new PostCategory()
            {
                Post = await _adminrepo.GetPostByID(id),
                Categories = await _bloggerrepo.GetCategories()
            };
            return View(postCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(Post post, IFormFile file)
        {
            Post postToEdit = await _adminrepo.GetPostByID(post.PostID);
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
                post.Picture = postToEdit.Picture;
            }

            post.DatePosted = postToEdit.DatePosted;

            if (ModelState.IsValid)
            {
                await _adminrepo.EditPost(post);
                TempData["MessageSuccess"] = "Post Edited Successfully";
                return RedirectToAction("Index");
            }

            PostCategory postCategory = new PostCategory()
            {
                Post = post,
                Categories = await _bloggerrepo.GetCategories()
            };
            return View(postCategory);
        }
        
        public async Task<ActionResult> AddUser()
        {
            return View();
        }

        public async Task<ActionResult> ManageUsers()
        {
            IEnumerable<User> users = await _adminrepo.ViewAllUsers();
            return View(users);
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(Register register, IFormFile file)
        {
            byte[] fileData = null;
            if (file != null && file.Length != 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileData = ms.ToArray();
                }

                register.Avatar = fileData;
            }
            else
            {
                ModelState.AddModelError("Picture", "Please select an Picture for yourself");
            }

            if (ModelState.IsValid)
            {
                if (register.Password == register.RepeatPassword)
                {
                    await _adminrepo.AdminRegisterUser(register);
                    TempData["MessageSuccess"] = "User Added Successfully";
                    return RedirectToAction("Index");
                }else
                {
                    ModelState.AddModelError("RepeatPassword", "Password does not match");
                }
            }

            return View(register);
        }

        // GET: AdminController/EditUser/5
        public async Task<ActionResult> EditUser(int UserId)
        {
            User user = await _adminrepo.GetUserByID(UserId);
            return View(user);
        }

        // POST: AdminController/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUser(User user, IFormFile file)
        {
            User userToEdit = await _adminrepo.GetUserByID(user.UserId);
            byte[] fileData = null;
            if (file != null && file.Length != 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    fileData = ms.ToArray();
                }

                user.Avatar = fileData;
            }
            else
            {
                user.Avatar = userToEdit.Avatar;
            }

            if (ModelState.IsValid)
            {
                 await _adminrepo.EditUser(user);
                 TempData["MessageSuccess"] = "User Edited Successfully";
                 return RedirectToAction("ManageUsers"); 
            }

            return View(user);
        }

        // GET: AdminController/Delete/5
        public async Task<ActionResult> DeleteUser(int UserId)
        {
            if (UserId == null || UserId == 0)
            {
                return NotFound();
            }
            User user = await _adminrepo.GetUserByID(UserId);

            if (user == null)
            {
                return NotFound();
            }
            else
            {
                TempData["UserId"] = user.UserId;
                TempData["Username"] = user.Username;
                return RedirectToAction("ManageUsers");
            }
        }

        public async Task<ActionResult> DeleteConfirmed(int UserId)
        {
            try
            {
                await _adminrepo.DeleteUser(UserId);
                return Json(new { success = true, message = "User deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // Return a JSON response with the error message
                return Json(new { success = false, message = "An error occurred while deleting the post: " + ex.Message });
            }
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                await _adminrepo.AddCategory(category);
                return RedirectToAction("ManageCategories", "Admin");
            }
            return View();
        }

        public async Task<ActionResult> ManageCategories()
        {
            IEnumerable<Category> categories = await _adminrepo.ViewAllCategories();
            return View(categories);
        }

        // POST: AdminController/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCategory(Category category)
        {
            
            if (ModelState.IsValid)
            {
                await _adminrepo.EditCategory(category);
                TempData["MessageSuccess"] = "Category Edited Successfully";
                return RedirectToAction("ManageCategories");
            }

            return View(category);
        }
        
        public async Task<ActionResult> EditCategory(int id)
        {
            
            Category category = await _adminrepo.GetCategoryByID(id);

            return View(category);
        }

        public async Task<ActionResult> DeleteCategory(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category cat = await _adminrepo.GetCategoryByID(id);

            if (cat == null)
            {
                return NotFound();
            }
            else
            {
                TempData["CategoryId"] = cat.CategoryID;
                TempData["Category"] = cat.Description;
                return RedirectToAction("ManageCategories");
            }
        }

        public async Task<ActionResult> CategoryDeleteConfirmed(int id)
        {
            try
            {
                await _adminrepo.DeleteCategory(id);
                return Json(new { success = true, message = "User deleted successfully!" });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // Return a JSON response with the error message
                return Json(new { success = false, message = "An error occurred while deleting the post: " + ex.Message });
            }
        }

        public async Task<ActionResult> PostAdminDelete(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Post post = await _adminrepo.GetPostByID(id);

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                TempData["PostID"] = post.PostID;
                TempData["PostTitle"] = post.Title;
                return RedirectToAction("Index", "Admin");
            }
        }

        public async Task<ActionResult> AdminPostDeleteConfirmed(int id)
        {
            try
            {
                await _adminrepo.AdminDeletePost(id);
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
