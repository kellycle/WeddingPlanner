using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingPlannerContext db;
        public HomeController(WeddingPlannerContext context)
        {
            db = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if(currentUserId != null){
                return RedirectToAction("Success", "Weddings");
            }

            return View("Index");
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            // Check initial ModelState
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(db.Users.Any(u => u.Email == newUser.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided error message
                    ModelState.AddModelError("Email", "Email already in use!");

                    // return View("Index");
                }
            }
            if(ModelState.IsValid == false)
            {
                return View("Index");
            }

            // Initializing a PasswordHasher object, providing our User class as its type
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            // Save your user object to the database
            db.Add(newUser);
            db.SaveChanges();

            // Refactor: Instead of saving the entire User into session, just save UserId and User name from newUser

            HttpContext.Session.SetInt32("UserId", newUser.UserId);
            HttpContext.Session.SetString("FirstName", newUser.FirstName);
            HttpContext.Session.SetString("LastName", newUser.LastName);

            return RedirectToAction("Success", "Weddings");
        }

        // [HttpGet("dashboard")]
        // public IActionResult Success()
        // {
        //     int? currentUserId = HttpContext.Session.GetInt32("UserId");
        //     if(currentUserId != null)
        //     {
        //         return View("UserDashboard");
        //     } else {
        //         return RedirectToAction("Index");
        //     }
        // }

        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If initial ModelState is valid, query for a user with provided email
                var userInDb = db.Users.FirstOrDefault(u => u.Email == userSubmission.loginEmail);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("loginEmail", "Email doesn't exist");
                    return View("Index");
                }

                // Initialize hasher object
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();

                // Refactor: Avoid using var, hover over var to find the data type

                // Verify provided password against hash stored in db
                PasswordVerificationResult result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.loginPassword);

                // Result can be compared to 0 for failure
                if(result == 0)
                {
                    // Handle failure (this should be similar to how "existing email" is handled)
                    // Manually add a ModelState error to the Email field, with provided error message
                    ModelState.AddModelError("loginEmail", "Invalid email/password");

                    // You may consider returning to the View at this point
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                HttpContext.Session.SetString("FirstName", userInDb.FirstName);
                HttpContext.Session.SetString("LastName", userInDb.LastName);

                return RedirectToAction("Success", "Weddings");
            } else {
                return View("Index");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}