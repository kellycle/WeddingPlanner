using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class WeddingsController : Controller
    {
        private WeddingPlannerContext db;
        public WeddingsController(WeddingPlannerContext context)
        {
            db = context;
        }

        [HttpGet("dashboard")]
        public IActionResult Success()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if(currentUserId != null)
            {
                // .Include deals with the model of the table queried
                // .ThenInclude deals with the model that was just added from the .Include immediately before it                
                List<Wedding> expiredWeddings = db.Weddings.Where(w => w.Date < DateTime.Now).ToList();
                db.RemoveRange(expiredWeddings);
                db.SaveChanges();

                List<Wedding> allWeddings = db.Weddings
                    .Include(wedding => wedding.CreatedBy)
                    .OrderBy(wedding => wedding.WedderOne)
                    .Include(wedding => wedding.Attending)
                    .ThenInclude(attending => attending.User)
                    .ToList();



                return View("UserDashboard", allWeddings);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet("weddings/new")]
        public IActionResult New()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if(currentUserId != null)
            {
                return View("New");
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("/weddings/create")]
        public IActionResult Create(Wedding newWedding)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!ModelState.IsValid)
            {
                return View("New");
            }
            // Assign foreign key or this error will show up -> "foreign key constraint fails"
            newWedding.UserId = (int)currentUserId; // relate this wedding to current user
            db.Weddings.Add(newWedding);
            db.SaveChanges();

            // When redirecting to a method that has params, pass in a 'new' dictionary: new { paramName = valueForParam }
            return RedirectToAction("Details", new { weddingId = newWedding.WeddingId });
        }

        [HttpPost("/weddings/create2")]
        public IActionResult Create2(Wedding newWedding)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!ModelState.IsValid)
            {
                List<Wedding> allWeddings = db.Weddings
                    .Include(wedding => wedding.CreatedBy)
                    .OrderBy(wedding => wedding.WedderOne)
                    .Include(wedding => wedding.Attending)
                    .ThenInclude(attending => attending.User)
                    .ToList();

                return View("UserDashboard", allWeddings);
            }
            // Assign foreign key or this error will show up -> "foreign key constraint fails"
            newWedding.UserId = (int)currentUserId; // relate this wedding to current user
            db.Weddings.Add(newWedding);
            db.SaveChanges();

            // When redirecting to a method that has params, pass in a 'new' dictionary: new { paramName = valueForParam }
            return RedirectToAction("Details", new { weddingId = newWedding.WeddingId });
        }

        [HttpGet("/weddings/{weddingId}")]
        public IActionResult Details(int weddingId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if(currentUserId != null)
            {
                Wedding wedding = db.Weddings
                    .Include(wedding => wedding.CreatedBy)
                    .Include(wedding => wedding.Attending)
                    .ThenInclude(attending => attending.User)
                    .FirstOrDefault(wedding => wedding.WeddingId == weddingId);

                    if (wedding == null)
                    {
                        return RedirectToAction("Success");
                    }

                return View("Details", wedding);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("/weddings/{weddingId}/delete")]
        public IActionResult Delete(int weddingId)
        {
            Wedding wedding = db.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);

            if (wedding != null)
            {
                db.Weddings.Remove(wedding);
                db.SaveChanges();
            }
            return RedirectToAction("Success");
        }
        
        [HttpGet("/weddings/{weddingId}/edit")]
        public IActionResult Edit(int weddingId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            if(currentUserId != null)
            {
                Wedding wedding = db.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);

                if(wedding == null)
                {
                    return RedirectToAction("Success");
                }

                return View("Edit", wedding);
            } else {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("/weddings/{weddingId}/update")]
        public IActionResult Update(Wedding editedWedding, int weddingId)
        {
            if(ModelState.IsValid == false)
            {
                editedWedding.WeddingId = weddingId;
                return View("Edit", editedWedding);
            } 

            Wedding wedding = db.Weddings.FirstOrDefault(w => w.WeddingId == weddingId);

            wedding.WedderOne = editedWedding.WedderOne;
            wedding.WedderTwo = editedWedding.WedderTwo;
            wedding.Date = editedWedding.Date;
            wedding.Address = editedWedding.Address;
            wedding.UpdatedAt = DateTime.Now;

            db.Weddings.Update(wedding);
            db.SaveChanges();

            return RedirectToAction("Details", new { weddingId = weddingId });
        }

        [HttpPost("/weddings/{weddingId}/attend")]
        public IActionResult Attend(int weddingId)
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserId");
            UserAttendWedding existingRSVP = db.UserAttendWeddings.FirstOrDefault(uaw => uaw.UserId == (int)currentUserId && uaw.WeddingId == weddingId);

            if (existingRSVP != null)
            {
                db.UserAttendWeddings.Remove(existingRSVP);
            } else {
                UserAttendWedding newRSVP = new UserAttendWedding()
                {
                    UserId = (int)currentUserId,
                    WeddingId = weddingId
                };

                db.UserAttendWeddings.Add(newRSVP);
            }
            
            db.SaveChanges();
            return RedirectToAction("Success");
        }
    }
}