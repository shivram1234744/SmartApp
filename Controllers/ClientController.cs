using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    [Authorize(Roles = "Client")]

    public class ClientController : Controller
    {
        private readonly DbManager _db;

        public ClientController(DbManager db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            var data = _db.Client_Dtl.ToList();

            foreach (var item in data)
            {
                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                var completed = _db.Verification_Status.Count(x =>
                    x.ClientId == item.Id &&
                    x.Status == "Completed");

                item.Progress = total > 0 ? (completed * 100 / total) : 0;
            }

            var pending = _db.Client_Dtl
    .Count(x => x.Status == "Pending");

            var inProgress = _db.Client_Dtl
                .Count(x => x.Status == "In Progress");

            var complete = _db.Client_Dtl
                .Count(x => x.Status == "Completed");

            ViewBag.Pending = pending;
            ViewBag.InProgress = inProgress;
            ViewBag.Completed = complete;
            //var data = _db.Client_Dtl
            //    .OrderByDescending(x => x.Id)
            //    .ToList();

            return View(data);
        }
        public IActionResult MyCases()
        {
            var data = _db.Client_Dtl.ToList();

            foreach (var item in data)
            {
                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                var completed = _db.Verification_Status.Count(x =>
                    x.ClientId == item.Id &&
                    x.Status == "Completed");

                item.Progress = total > 0 ? (completed * 100 / total) : 0;
            }

            var pending = _db.Client_Dtl
    .Count(x => x.Status == "Pending");

            var inProgress = _db.Client_Dtl
                .Count(x => x.Status == "In Progress");

            var complete = _db.Client_Dtl
                .Count(x => x.Status == "Completed");

            ViewBag.Pending = pending;
            ViewBag.InProgress = inProgress;
            ViewBag.Completed = complete;
            //var data = _db.Client_Dtl
            //    .OrderByDescending(x => x.Id)
            //    .ToList();

            return View(data);

        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NewRequest()
        {
            var data = _db.Mst_VerificationChecks.ToList();
            ViewBag.Data = data;
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewRequest(VerificationVM model)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;

                if (model.DocumentPath  != null && model.DocumentPath .Length > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    fileName = Guid.NewGuid().ToString() + "_" + model.DocumentPath .FileName;

                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentPath .CopyToAsync(stream);
                    }
                }

                var newRequest = new ClientDtl
                {
                    Name = model.Name,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    DOB = model.DOB,
                    Position = model.Position,
                    Department = model.Department,
                    Priority = model.Priority,

                    DocumentPath  = fileName,

                    VerifyType = model.VerifyTypes != null
                        ? string.Join(",", model.VerifyTypes)
                        : "",

                    CreatedAt = DateTime.Now,
                    Status = "Pending"
                };

                // Save Client
                _db.Client_Dtl.Add(newRequest);
                await _db.SaveChangesAsync();

                // =====================================
                // INSERT INTO tbl_verification
                // =====================================

                if (model.VerifyTypes != null)
                {
                    foreach (var item in model.VerifyTypes)
                    {
                        Verification verification = new Verification()
                        {
                            ClientId = newRequest.Id,

                            VarId = Convert.ToInt32(item),

                            Status = "Pending",

                            CreatedAt = DateTime.Now
                        };
                        _db.Verification_Status.Add(verification);
                    }

                    await _db.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Verification request submitted successfully.";

                return RedirectToAction("Dashboard");
            }

            return View(model);
        }
        public async Task<IActionResult> Details(int id)
        {
            if (id != 0)
            {
                // 1. Existing Verification Status Data
                ViewBag.VarData = (from s in _db.Verification_Status
                                   join n in _db.Mst_VerificationChecks
                                   on s.VarId equals n.Id into temp
                                   from n in temp.DefaultIfEmpty()
                                   where s.ClientId == id
                                   select new
                                   {
                                       s.Id,
                                       s.ClientId,
                                       s.VarId,
                                       s.Status,
                                       s.CreatedAt,
                                       s.DocumentPath,
                                       s.ExecutorNotes,
                                       Name = n != null ? n.Name : ""
                                   }).ToList();

               
                return View(_db.Client_Dtl.FirstOrDefault(x => x.Id == id));
            }
            else
            {
                return RedirectToAction("Dashboard");
            }
        }
    }
}
