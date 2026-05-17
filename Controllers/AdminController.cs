using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Org.BouncyCastle.Crypto.Generators;
using System.Data;
using TrustPlus.Models;

namespace TrustPlus.Controllers
{
    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly DbManager _db;

        public AdminController(DbManager db)
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
        public IActionResult RequestMgmt()
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

        public IActionResult EmployeeMst()
        {
           // var employee = _db.EmployeeMst.ToList();
            return View();
        }
        [HttpGet]
        public IActionResult GetData()
        {
            var employee = _db.EmployeeMst.ToList();
            return Json(employee);
        }
        [HttpPost]
        public IActionResult AddEmployee([FromForm]EmployeeMst model)
        {
            model.EmployeeId = 0;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                    throw new ArgumentException("Password is required");

                //  Encrypt password before saving
                model.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                _db.EmployeeMst.Add(model);
                _db.SaveChanges();
                return Json(new { success = true, message = "Employee saved successfully!" });
            }
            return Json(new { success = false, message = "Validation failed." });
        }
        [HttpGet]
        public IActionResult GetEmployeeById(int id)
        {
            var employee = _db.EmployeeMst.FirstOrDefault(e => e.EmployeeId == id);
            if (employee != null)
            {
                return Json(employee);
            }
            return Json(new { success = false, message = "Employee  not found" });

        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            var emp = _db.EmployeeMst.FirstOrDefault(e => e.EmployeeId == id);
            if (emp != null)
            {
                _db.EmployeeMst.Remove(emp);
                _db.SaveChanges();
                return Json(new { success = true, message = "Employee deleted successfully!" });
            }
            return Json(new { success = false, message = "Employee not found." });
        }

        [HttpPost]
        public IActionResult UpdateEmployee([FromForm] EmployeeMst model)
        {
            if (ModelState.IsValid)
            {
                var employee = _db.EmployeeMst.FirstOrDefault(e => e.EmployeeId == model.EmployeeId);
                if (employee != null)
                {
                    //update filds
                    employee.Name = model.Name;
                    employee.Email = model.Email;
                    employee.Address = model.Address;
                    employee.Password = model.Password;

                    _db.SaveChanges();

                    return Json(new { success = true, message = "Employee Updated Successfully" });
                }
              
            }
            return Json(new { success = false, message = "Employee Not Updated" });
        }

        public IActionResult AssignedJob()
        {
            var data = _db.Client_Dtl.ToList();

            foreach (var item in data)
            {
                var total = _db.Verification_Status.Count(x => x.ClientId == item.Id);

                var completed = _db.Verification_Status.Count(x =>
                    x.ClientId == item.Id &&
                    x.Status == "Completed");

                item.Progress = total > 0 ? (completed * 100 / total) : 0;

                
                var empName = (from v in _db.Verification_Status
                               join e in _db.EmployeeMst
                               on v.Emp_Id equals e.EmployeeId
                               where v.ClientId == item.Id
                               select e.Name).FirstOrDefault();

               
                item.EmployeeName = empName;
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRequest(VerificationVM model)
        {
            if (ModelState.IsValid)
            {
                string fileName = null;

               
                if (model.DocumentPath != null && model.DocumentPath.Length > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    
                    fileName = Guid.NewGuid().ToString() + "_" + model.DocumentPath.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.DocumentPath.CopyToAsync(stream);
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

                    DocumentPath = fileName,
                    VerifyType = model.VerifyTypes != null
                       ? string.Join(",", model.VerifyTypes)
                       : "",

                    CreatedAt = DateTime.Now,
                    Status = "Pending"
                };

                _db.Client_Dtl.Add(newRequest);
                await _db.SaveChangesAsync();
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }
        [HttpPost]
        public IActionResult SaveEmp(int? id,int clientId )
        {

          
            if (id == 0 || clientId == 0)
            {
                return Json(new { success = false, message = "Invalid Employee or Client Id!" });
            }

            // Get all verification records for this client
            var records = _db.Verification_Status
                             .Where(v => v.ClientId == clientId)
                             .ToList();

            if (records.Any())
            {
                foreach (var record in records)
                {
                    record.Emp_Id = id;          // assign employee to each record
              
                }

                _db.SaveChanges();
                return Json(new { success = true, message = "Employee assigned  for client "});
            }
            return Json(new { success = false, message = "Employee Not Saved!" });
        }

        //[HttpPost]
        //public IActionResult SaveEmpInClient(int? id, int clientId, int companyId)
        //{


        //    if (id == 0 || clientId == 0)
        //    {
        //        return Json(new { success = false, message = "Invalid Employee or Client Id!" });
        //    }

        //    // Get all verification records for this client
        //    var records = _db.Verification_Status
        //                     .Where(v => v.ClientId == clientId)
        //                     .ToList();

        //    if (records.Any())
        //    {
        //        foreach (var record in records)
        //        {
        //            record.Emp_Id = id;          // assign employee to each record

        //        }

        //        _db.SaveChanges();
        //        return Json(new { success = true, message = "Employee assigned  for client " });
        //    }
        //    return Json(new { success = false, message = "Employee Not Saved!" });
        //}
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
